
using Structural_Automation.AutoCadCommands.Acad;
using Structural_Automation.AutoCadCommands.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Structural_Automation.AutoCadCommands.Plans
{
    /// <summary>
    /// The circle marking where (0,0) is for a floor plan. Exactly one belongs in each,
    /// which this class can count but only the command can insist on.
    /// </summary>
    public class FloorPlanOrigin(Drawing drawing, SaLayerTable layers, FloorPlans plans)
    {
        /// <summary>The circle is this fraction of the box width, so it reads at any scale.</summary>
        private readonly double _radiusScale = 1.0 / 100.0;

        public ObjectId Place(Point3d centre, FloorPlan plan)
        {
            layers.Ensure(SaLayer.SA_FLOOR_PLAN_ORIGIN);

            using Transaction transaction = drawing.Start();

            double radius = (plan.Bounds.MaxPoint.X - plan.Bounds.MinPoint.X) * _radiusScale;
            Circle circle = new(centre, Vector3d.ZAxis, radius);

            ObjectId id = drawing.Add(transaction, circle, SaLayer.SA_FLOOR_PLAN_ORIGIN);
            transaction.Commit();

            return id;
        }

        /// <summary>
        /// The origins belonging to the floor plan. An origin belongs to one plan only, so
        /// a box large enough to reach into its neighbour does not claim its origin too.
        /// </summary>
        public List<ObjectId> In(FloorPlan plan)
        {
            List<FloorPlan> all = plans.All();

            using Transaction transaction = drawing.Start();
            Entities entities = new(transaction);

            List<ObjectId> origins = [];
            List<ObjectId> candidates = entities.OnLayer(
                SaLayer.SA_FLOOR_PLAN_ORIGIN, drawing.ModelSpaceIds(transaction));

            foreach (ObjectId id in candidates)
            {
                Circle circle = (Circle)transaction.GetObject(id, OpenMode.ForRead);

                if (plan.Owns(circle.Center, all))
                {
                    origins.Add(id);
                }
            }

            transaction.Commit();
            return origins;
        }

        /// <summary>Erases every origin but the one the user chose to keep.</summary>
        public int KeepOnly(ObjectId keep, IEnumerable<ObjectId> origins)
        {
            using Transaction transaction = drawing.Start();
            Entities entities = new(transaction);

            int erased = 0;
            foreach (ObjectId id in origins)
            {
                if (id == keep)
                {
                    continue;
                }

                entities.Erase(id);
                erased++;
            }

            transaction.Commit();
            return erased;
        }
    }
}
