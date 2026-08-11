
using Structural_Automation.AutoCadCommands.Acad;
using Structural_Automation.AutoCadCommands.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Structural_Automation.AutoCadCommands.Plans
{
    /// <summary>
    /// The floor plans a drawing holds, found by their bounding boxes. Every command
    /// that speaks about a floor plan by name asks here, so the way a box and its label
    /// are matched lives in one place.
    /// </summary>
    public class FloorPlans(Drawing drawing)
    {
        /// <summary>
        /// A label further from a corner than this many box widths belongs to another
        /// floor plan, not this one.
        /// </summary>
        private readonly double _labelReach = 1.0;

        private readonly string _unnamed = "unnamed floor plan";

        public List<FloorPlan> All()
        {
            using Transaction transaction = drawing.Start();
            Entities entities = new(transaction);

            List<ObjectId> everything = drawing.ModelSpaceIds(transaction);
            List<ObjectId> labels = entities.OnLayer(SaLayer.SA_FLOOR_PLAN_BBOX_TEXT, everything);

            List<FloorPlan> plans = [];
            foreach (ObjectId box in entities.OnLayer(SaLayer.SA_FLOOR_PLAN_BBOX, everything))
            {
                Extents3d? bounds = entities.ExtentsOf(box);
                if (bounds is null)
                {
                    continue;
                }

                plans.Add(new FloorPlan(box, NameNear(transaction, bounds.Value, labels), bounds.Value));
            }

            transaction.Commit();
            return plans;
        }

        /// <summary>
        /// The plan a point belongs to: the smallest one covering it, so overlapping boxes
        /// never both claim it.
        /// </summary>
        public FloorPlan? Containing(Point3d point)
        {
            List<FloorPlan> all = All();

            foreach (FloorPlan plan in all)
            {
                if (plan.Owns(point, all))
                {
                    return plan;
                }
            }

            return null;
        }

        /// <summary>
        /// The label nearest the box's bottom left corner, which is where the bounding
        /// box command writes it. Nothing ties a label to a box but distance, so a label
        /// further away than the box is wide is taken to belong to another plan.
        /// </summary>
        private string NameNear(Transaction transaction, Extents3d box, List<ObjectId> labels)
        {
            double reach = (box.MaxPoint.X - box.MinPoint.X) * _labelReach;
            double nearest = double.MaxValue;
            string name = _unnamed;

            foreach (ObjectId id in labels)
            {
                DBText label = (DBText)transaction.GetObject(id, OpenMode.ForRead);
                double distance = (label.Position - box.MinPoint).Length;

                if (distance < nearest && distance <= reach)
                {
                    nearest = distance;
                    name = label.TextString;
                }
            }

            return name;
        }
    }
}
