
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
// Aliased, since WinForms brings in an Application of its own.
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(Structural_Automation.AutoCadCommands.Commands))]

namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// Every command AutoCAD offers, and nothing else. Each one is a thin entry point:
    /// the drawing work belongs to the classes it calls, and all that happens here is
    /// naming the command, asking the user, and saying what it did.
    /// </summary>
    public class Commands
    {
        /// <summary>The palette every command draws in. AutoCAD holds one per document.</summary>
        private readonly SaLayerColors _colors = new();

        [CommandMethod("SA_CREATELAYERS")]
        public void CreateLayers()
        {
            Drawing drawing = new(AcadApplication.DocumentManager.MdiActiveDocument);
            LayerReport report = Layers(drawing).Create();

            drawing.Editor.WriteMessage($"\n{report.Applied.Count} layer(s) created.");

            if (report.Skipped.Count > 0)
            {
                drawing.Editor.WriteMessage($"\nWarning: {report.Skipped.Count} layer(s) were already in the drawing and "
                    + $"were left as they are, so their colour may not be the one we expect: "
                    + $"{string.Join(", ", report.Skipped)}");
            }
        }

        [CommandMethod("SA_DELETELAYERS")]
        public void DeleteLayers()
        {
            Drawing drawing = new(AcadApplication.DocumentManager.MdiActiveDocument);
            LayerReport report = Layers(drawing).Delete();

            drawing.Editor.WriteMessage($"\n{report.Applied.Count} layer(s) deleted.");

            if (report.Skipped.Count > 0)
            {
                drawing.Editor.WriteMessage($"\nWarning: {report.Skipped.Count} layer(s) are still in use and were kept - "
                    + $"move or erase what is drawn on them first: {string.Join(", ", report.Skipped)}");
            }
        }

        [CommandMethod("SA_FLOORPLANBBOX")]
        public void FloorPlanBoundingBox()
        {
            Drawing drawing = new(AcadApplication.DocumentManager.MdiActiveDocument);
            Prompts prompts = new(drawing.Editor);
            FloorPlanBbox plan = new(drawing, Layers(drawing));

            ObjectId? box = AskForBox();
            if (box is null)
            {
                return;
            }

            int elements = plan.CentreOnContent(box.Value);

            string? name = prompts.AskText("\nEnter the floor plan name");
            if (name is null)
            {
                return;
            }

            plan.Label(box.Value, name);
            drawing.Editor.WriteMessage($"\nFloor plan '{name}' boxed around {elements} element(s).");

            // Draws a box from two corners, or takes one the user already drew.
            ObjectId? AskForBox()
            {
                PromptPointResult corner = prompts.AskPointOrKeyword(
                    "\nSpecify first corner of the floor plan box or [Select an existing one]", "Select");

                if (corner.Status == PromptStatus.Keyword)
                {
                    return prompts.AskPolylineOn("\nSelect the bounding box", SaLayer.SA_BBOX);
                }

                if (corner.Status != PromptStatus.OK)
                {
                    return null;
                }

                Point3d? opposite = prompts.AskCorner("\nSpecify opposite corner", corner.Value);

                return opposite is null ? null : plan.Draw(corner.Value, opposite.Value);
            }
        }

        private SaLayerTable Layers(Drawing drawing)
        {
            return new SaLayerTable(drawing.Database, _colors);
        }
    }
}
