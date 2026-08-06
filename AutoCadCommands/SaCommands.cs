
using Autodesk.AutoCAD.Runtime;
using Structural_Automation.AutoCadCommands.Commands;
using Structural_Automation.AutoCadCommands.Layers;
// Aliased, since WinForms brings in an Application of its own.
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(Structural_Automation.AutoCadCommands.SaCommands))]

namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// Every command AutoCAD offers, declared and nothing more. Each hands off to a class
    /// of its own, so this file stays a list of what the plugin can do.
    /// </summary>
    public class SaCommands
    {
        /// <summary>The palette every command draws in. AutoCAD holds one per document.</summary>
        private readonly SaLayerColors _colors = new();

        /// <summary>The layers holding our own marks rather than the building.</summary>
        private readonly SaAnnotations _annotations = new();

        [CommandMethod("SA_CREATELAYERS")]
        public void CreateLayers()
        {
            new CreateLayersCommand(Start()).Run();
        }

        [CommandMethod("SA_DELETELAYERS")]
        public void DeleteLayers()
        {
            new DeleteLayersCommand(Start()).Run();
        }

        [CommandMethod("SA_FLOORPLANBBOX")]
        public void FloorPlanBoundingBox()
        {
            new FloorPlanBboxCommand(Start()).Run();
        }

        [CommandMethod("SA_ORIGIN")]
        public void Origin()
        {
            new OriginCommand(Start()).Run();
        }

        private Session Start()
        {
            return new Session(AcadApplication.DocumentManager.MdiActiveDocument, _colors, _annotations);
        }
    }
}
