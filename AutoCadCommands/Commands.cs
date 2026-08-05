
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
// Aliased, since WinForms brings in an Application of its own.
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(Structural_Automation.AutoCadCommands.Commands))]

namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// Every command AutoCAD offers, and nothing else. Each one is a thin entry point:
    /// the drawing work belongs to the class it calls, and all that happens here is
    /// naming the command and saying what it did.
    /// </summary>
    public class Commands
    {
        /// <summary>The palette every command draws in. AutoCAD holds one per document.</summary>
        private readonly SaLayerColors _colors = new();

        [CommandMethod("SA_CREATELAYERS")]
        public void CreateLayers()
        {
            Document document = AcadApplication.DocumentManager.MdiActiveDocument;
            SaLayerTable layers = new(document.Database, _colors);

            LayerReport report = layers.Create();
            Editor editor = document.Editor;

            editor.WriteMessage($"\n{report.Applied.Count} layer(s) created.");

            if (report.Skipped.Count > 0)
            {
                editor.WriteMessage($"\nWarning: {report.Skipped.Count} layer(s) were already in the drawing and "
                    + $"were left as they are, so their colour may not be the one we expect: "
                    + $"{string.Join(", ", report.Skipped)}");
            }
        }

        [CommandMethod("SA_DELETELAYERS")]
        public void DeleteLayers()
        {
            Document document = AcadApplication.DocumentManager.MdiActiveDocument;
            SaLayerTable layers = new(document.Database, _colors);

            LayerReport report = layers.Delete();
            Editor editor = document.Editor;

            editor.WriteMessage($"\n{report.Applied.Count} layer(s) deleted.");

            if (report.Skipped.Count > 0)
            {
                editor.WriteMessage($"\nWarning: {report.Skipped.Count} layer(s) are still in use and were kept - "
                    + $"move or erase what is drawn on them first: {string.Join(", ", report.Skipped)}");
            }
        }
    }
}
