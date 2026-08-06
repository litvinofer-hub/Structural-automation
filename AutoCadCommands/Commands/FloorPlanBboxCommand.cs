
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Structural_Automation.AutoCadCommands.Layers;
using Structural_Automation.AutoCadCommands.Plans;

namespace Structural_Automation.AutoCadCommands.Commands
{
    /// <summary>
    /// Boxes a floor plan: a rectangle the user draws or picks, centred on what it holds,
    /// with the floor plan name written at its corner.
    /// </summary>
    public class FloorPlanBboxCommand(Session session)
    {
        private readonly Session _session = session;
        private readonly FloorPlanBbox _box = new(session.Drawing, session.Layers, session.Annotations);

        public void Run()
        {
            ObjectId? box = AskForBox();
            if (box is null)
            {
                return;
            }

            int elements = _box.CentreOnContent(box.Value);

            string? name = _session.Prompts.AskText("\nEnter the floor plan name");
            if (name is null)
            {
                return;
            }

            _box.Label(box.Value, name);
            _session.Messages.Say($"Floor plan '{name}' boxed around {elements} element(s).");
        }

        /// <summary>
        /// The choice comes first, so picking a point only ever means drawing a new box and
        /// is never mistaken for pointing at one that is already there.
        /// </summary>
        private ObjectId? AskForBox()
        {
            string? choice = _session.Prompts.AskKeyword(
                "\nDraw a new floor plan box or select an existing one", ["Draw", "Select"], "Draw");

            if (choice is null)
            {
                return null;
            }

            if (choice == "Select")
            {
                return _session.Prompts.AskEntityOn(
                    "\nSelect the bounding box", SaLayer.SA_FLOOR_PLAN_BBOX, typeof(Polyline));
            }

            Point3d? corner = _session.Prompts.AskPoint("\nSpecify first corner", allowFinish: false);
            if (corner is null)
            {
                return null;
            }

            Point3d? opposite = _session.Prompts.AskCorner("\nSpecify opposite corner", corner.Value);

            return opposite is null ? null : _box.Draw(corner.Value, opposite.Value);
        }
    }
}
