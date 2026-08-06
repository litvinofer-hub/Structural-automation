
using Structural_Automation.AutoCadCommands.Acad;
using Structural_Automation.AutoCadCommands.Layers;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Structural_Automation.AutoCadCommands.Plans
{
    /// <summary>
    /// A floor plan's bounding box: the rectangle around it, and the name written at its
    /// corner. One step per method, so a later command can reuse whichever it needs.
    /// </summary>
    public class FloorPlanBbox(Drawing drawing, SaLayerTable layers, SaAnnotations annotations)
    {
        /// <summary>The name sits this many text heights below the box.</summary>
        private readonly double _nameGap = 1.5;

        /// <summary>The text is this fraction of the box width, so it reads at any scale.</summary>
        private readonly double _nameScale = 1.0 / 40.0;

        /// <summary>Draws the box on SA_FLOOR_PLAN_BBOX from two opposite corners.</summary>
        public ObjectId Draw(Point3d corner, Point3d opposite)
        {
            layers.Ensure(SaLayer.SA_FLOOR_PLAN_BBOX);

            using Transaction transaction = drawing.Start();

            Polyline box = new();
            box.AddVertexAt(0, new Point2d(corner.X, corner.Y), 0, 0, 0);
            box.AddVertexAt(1, new Point2d(opposite.X, corner.Y), 0, 0, 0);
            box.AddVertexAt(2, new Point2d(opposite.X, opposite.Y), 0, 0, 0);
            box.AddVertexAt(3, new Point2d(corner.X, opposite.Y), 0, 0, 0);
            box.Closed = true;

            ObjectId id = drawing.Add(transaction, box, SaLayer.SA_FLOOR_PLAN_BBOX);
            transaction.Commit();

            return id;
        }

        /// <summary>
        /// Moves the box, keeping its size, so its centre sits on the centre of everything
        /// completely inside it. Returns how many elements that was.
        /// </summary>
        public int CentreOnContent(ObjectId boxId)
        {
            using Transaction transaction = drawing.Start();
            Entities entities = new(transaction);

            Extents3d? box = entities.ExtentsOf(boxId);
            if (box is null)
            {
                transaction.Commit();
                return 0;
            }

            List<ObjectId> contents = entities.Inside(box.Value, Candidates(transaction));
            Extents3d? content = entities.CombinedExtents(contents);

            if (content is not null)
            {
                Point3d Centre(Extents3d extents) => extents.MinPoint + (extents.MaxPoint - extents.MinPoint) * 0.5;

                entities.MoveBy(boxId, Centre(content.Value) - Centre(box.Value));
            }

            transaction.Commit();
            return contents.Count;
        }

        /// <summary>Writes the floor plan name below the bottom left corner of the box.</summary>
        public void Label(ObjectId boxId, string name)
        {
            layers.Ensure(SaLayer.SA_FLOOR_PLAN_BBOX_TEXT);

            using Transaction transaction = drawing.Start();
            Entities entities = new(transaction);

            Extents3d? box = entities.ExtentsOf(boxId);
            if (box is null)
            {
                transaction.Commit();
                return;
            }

            Point3d corner = box.Value.MinPoint;
            double height = (box.Value.MaxPoint.X - corner.X) * _nameScale;

            DBText text = new()
            {
                TextString = name,
                Height = height,
                Position = new Point3d(corner.X, corner.Y - height * _nameGap, corner.Z)
            };

            drawing.Add(transaction, text, SaLayer.SA_FLOOR_PLAN_BBOX_TEXT);
            transaction.Commit();
        }

        /// <summary>
        /// Everything the box could hold: model space less our own marks, so a second run
        /// does not measure its own label and drift.
        /// </summary>
        private List<ObjectId> Candidates(Transaction transaction)
        {
            List<ObjectId> candidates = [];

            foreach (ObjectId id in drawing.ModelSpaceIds(transaction))
            {
                Entity entity = (Entity)transaction.GetObject(id, OpenMode.ForRead);
                if (annotations.Includes(entity.Layer))
                {
                    continue;
                }

                candidates.Add(id);
            }

            return candidates;
        }
    }
}
