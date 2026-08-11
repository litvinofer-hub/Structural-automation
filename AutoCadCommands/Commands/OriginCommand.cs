
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Structural_Automation.AutoCadCommands.Model;
using Structural_Automation.AutoCadCommands.Plans;

namespace Structural_Automation.AutoCadCommands.Commands
{
    /// <summary>
    /// Asks for one origin circle per floor plan, then insists on exactly one in each: the
    /// plans left empty are asked for again by name, and a plan holding several is settled
    /// by the user saying which to keep.
    /// </summary>
    public class OriginCommand(Session session)
    {
        private readonly Session _session = session;
        private readonly FloorPlanOrigin _origins = new(session.Drawing, session.Layers, session.Plans);

        public void Run()
        {
            List<FloorPlan> all = _session.Plans.All();
            if (all.Count == 0)
            {
                _session.Messages.Say("No floor plans found. Run SA_FLOORPLANBBOX first.");
                return;
            }

            if (!DrawMany())
            {
                return;
            }

            while (true)
            {
                List<FloorPlan> empty = WithOrigins(all, origins => origins == 0);
                List<FloorPlan> crowded = WithOrigins(all, origins => origins > 1);

                if (empty.Count == 0 && crowded.Count == 0)
                {
                    break;
                }

                if (!Thin(crowded) || !Fill(empty))
                {
                    return;
                }
            }

            _session.Messages.Say($"{all.Count} floor plan(s), one origin each.");
        }

        /// <summary>The plans whose origin count is one the caller is looking for.</summary>
        private List<FloorPlan> WithOrigins(List<FloorPlan> plans, Func<int, bool> wanted)
        {
            return plans.Where(plan => wanted(_origins.In(plan).Count)).ToList();
        }

        /// <summary>A circle wherever the user picks, until they say they are done.</summary>
        private bool DrawMany()
        {
            while (true)
            {
                Point3d? centre = _session.Prompts.AskPoint(
                    "\nDraw an origin in each floor plan, or press Enter when finished", allowFinish: true);

                if (centre is null)
                {
                    return true;
                }

                Place(centre.Value, target: null);
            }
        }

        /// <summary>
        /// Drawing another circle cannot fix a plan holding too many, so the user says which
        /// one is the origin and the rest go.
        /// </summary>
        private bool Thin(List<FloorPlan> crowded)
        {
            foreach (FloorPlan plan in crowded)
            {
                List<ObjectId> found = _origins.In(plan);
                _session.Messages.Warn($"'{plan.Name}' has {found.Count} origins, it must have exactly one.");

                ObjectId? keep = _session.Prompts.AskEntityOn(
                    $"\nPick the origin to keep in '{plan.Name}'", SaLayer.SA_FLOOR_PLAN_ORIGIN, typeof(Circle));

                if (keep is null)
                {
                    return false;
                }

                if (!found.Contains(keep.Value))
                {
                    _session.Messages.Say($"That origin is not in '{plan.Name}'.");
                    return false;
                }

                _session.Messages.Say($"{_origins.KeepOnly(keep.Value, found)} origin(s) erased.");
            }

            return true;
        }

        private bool Fill(List<FloorPlan> empty)
        {
            foreach (FloorPlan plan in empty)
            {
                _session.Messages.Warn($"'{plan.Name}' has no origin.");

                Point3d? centre = _session.Prompts.AskPoint($"\nDraw the origin in '{plan.Name}'", allowFinish: false);
                if (centre is null)
                {
                    return false;
                }

                Place(centre.Value, plan);
            }

            return true;
        }

        /// <summary>
        /// Draws one origin, refusing a point that belongs to no floor plan or to one other
        /// than the plan being asked about.
        /// </summary>
        private void Place(Point3d centre, FloorPlan? target)
        {
            FloorPlan? plan = _session.Plans.Containing(centre);

            if (plan is null)
            {
                _session.Messages.Say("That point is not inside any floor plan.");
                return;
            }

            if (target is not null && plan.Box != target.Box)
            {
                _session.Messages.Say($"That point is not inside '{target.Name}'.");
                return;
            }

            _origins.Place(centre, plan);
        }
    }
}
