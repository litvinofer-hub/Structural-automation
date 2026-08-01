
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
// Aliased, since WinForms brings in an Application of its own.
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(Structural_Automation.AutoCadCommands.Commands))]

namespace Structural_Automation.AutoCadCommands
{
    public class Commands
    {
        [CommandMethod("SAHELLO")]
        public void Hello()
        {
            Editor editor = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage("\nStructural Automation is loaded.");
        }
    }
}
