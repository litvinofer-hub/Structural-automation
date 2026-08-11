
using Structural_Automation.AutoCadCommands.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Structural_Automation.AutoCadCommands.Acad
{
    /// <summary>
    /// Reads and moves entities through one transaction. Extents are null rather than
    /// thrown, since an empty text or block has none and that is not an error.
    /// </summary>
    public class Entities(Transaction transaction)
    {
        public Extents3d? ExtentsOf(ObjectId id)
        {
            Entity entity = (Entity)transaction.GetObject(id, OpenMode.ForRead);

            try
            {
                return entity.GeometricExtents;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                return null;
            }
        }

        public Extents3d? CombinedExtents(IEnumerable<ObjectId> ids)
        {
            Extents3d? combined = null;

            foreach (ObjectId id in ids)
            {
                Extents3d? extents = ExtentsOf(id);
                if (extents is null)
                {
                    continue;
                }

                if (combined is null)
                {
                    combined = extents;
                    continue;
                }

                Extents3d grown = combined.Value;
                grown.AddExtents(extents.Value);
                combined = grown;
            }

            return combined;
        }

        /// <summary>The ids lying completely within the given extents.</summary>
        public List<ObjectId> Inside(Extents3d box, IEnumerable<ObjectId> ids)
        {
            List<ObjectId> inside = [];

            foreach (ObjectId id in ids)
            {
                Extents3d? extents = ExtentsOf(id);
                if (extents is null)
                {
                    continue;
                }

                // X and Y only. A floor plan is a plan, so elevation should not decide
                // what belongs to it.
                Point3d min = extents.Value.MinPoint;
                Point3d max = extents.Value.MaxPoint;

                if (min.X < box.MinPoint.X || min.Y < box.MinPoint.Y)
                {
                    continue;
                }

                if (max.X > box.MaxPoint.X || max.Y > box.MaxPoint.Y)
                {
                    continue;
                }

                inside.Add(id);
            }

            return inside;
        }

        public List<ObjectId> OnLayer(SaLayer layer, IEnumerable<ObjectId> ids)
        {
            List<ObjectId> found = [];

            foreach (ObjectId id in ids)
            {
                Entity entity = (Entity)transaction.GetObject(id, OpenMode.ForRead);
                if (entity.Layer == layer.ToString())
                {
                    found.Add(id);
                }
            }

            return found;
        }

        public void Erase(ObjectId id)
        {
            Entity entity = (Entity)transaction.GetObject(id, OpenMode.ForWrite);
            entity.Erase();
        }

        public void MoveBy(ObjectId id, Vector3d offset)
        {
            Entity entity = (Entity)transaction.GetObject(id, OpenMode.ForWrite);
            entity.TransformBy(Matrix3d.Displacement(offset));
        }
    }
}
