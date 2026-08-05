
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
// Aliased, since WinForms brings in an Application of its own.
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
// Aliased, since WinForms brings in System.Drawing.Color.
using AcadColor = Autodesk.AutoCAD.Colors.Color;

[assembly: CommandClass(typeof(Structural_Automation.AutoCadCommands.Commands))]

namespace Structural_Automation.AutoCadCommands
{
    public class Commands
    {
        /// <summary>
        /// Creates every <see cref="SaLayer"/> missing from the drawing. A layer already
        /// there is kept untouched and warned about, since its colour may not be ours.
        /// </summary>
        [CommandMethod("SA_CREATELAYERS")]
        public void CreateLayers()
        {
            Document document = AcadApplication.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            SaLayerColors colors = new();
            SaLayer[] layers = Enum.GetValues<SaLayer>();
            List<string> kept = [];
            int created = 0;

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

                foreach (SaLayer layer in layers)
                {
                    string name = layer.ToString();
                    if (layerTable.Has(name))
                    {
                        kept.Add(name);
                        continue;
                    }

                    LayerTableRecord record = new()
                    {
                        Name = name,
                        Color = AcadColor.FromColorIndex(ColorMethod.ByAci, colors.ColorIndexOf(layer))
                    };

                    layerTable.UpgradeOpen();
                    layerTable.Add(record);
                    transaction.AddNewlyCreatedDBObject(record, true);
                    created++;
                }

                transaction.Commit();
            }

            editor.WriteMessage($"\n{created} layer(s) created.");

            if (kept.Count > 0)
            {
                editor.WriteMessage($"\nWarning: {kept.Count} layer(s) were already in the drawing and were left as they are, "
                    + $"so their colour may not be the one we expect: {string.Join(", ", kept)}");
            }
        }
    }
}
