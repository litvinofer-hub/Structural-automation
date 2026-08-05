
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
// Aliased, since WinForms brings in System.Drawing.Color.
using AcadColor = Autodesk.AutoCAD.Colors.Color;

namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// A drawing's layer table, seen through the layers we own. Every command that
    /// touches layers goes through here, so the AutoCAD database work lives in one place.
    /// The palette is given rather than chosen, so the caller decides what the layers
    /// look like.
    /// </summary>
    public class SaLayerTable(Database database, SaLayerColors colors)
    {

        /// <summary>
        /// Creates the layers the drawing lacks. One already there is left untouched and
        /// reported as skipped, since its colour may not be ours.
        /// </summary>
        public LayerReport Create()
        {
            List<string> created = [];
            List<string> kept = [];

            using Transaction transaction = database.TransactionManager.StartTransaction();
            LayerTable layerTable = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

            foreach (SaLayer layer in Enum.GetValues<SaLayer>())
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
                created.Add(name);
            }

            transaction.Commit();
            return new LayerReport(created, kept);
        }

        /// <summary>
        /// Deletes the layers the drawing holds. One still in use is kept and reported as
        /// skipped - AutoCAD cannot erase a layer that owns objects, is current, or is
        /// referenced by a block.
        /// </summary>
        public LayerReport Delete()
        {
            List<string> deleted = [];
            List<string> inUse = [];

            using Transaction transaction = database.TransactionManager.StartTransaction();
            LayerTable layerTable = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

            ObjectIdCollection ours = [];
            foreach (SaLayer layer in Enum.GetValues<SaLayer>())
            {
                string name = layer.ToString();
                if (layerTable.Has(name))
                {
                    ours.Add(layerTable[name]);
                }
            }

            // Purge narrows the collection it is given to what nothing refers to, so asking
            // it first is what keeps the erase below from throwing and losing the lot.
            ObjectIdCollection erasable = new([.. ours.Cast<ObjectId>()]);
            database.Purge(erasable);

            foreach (ObjectId id in ours)
            {
                LayerTableRecord record = (LayerTableRecord)transaction.GetObject(id, OpenMode.ForRead);
                string name = record.Name;

                if (!erasable.Contains(id))
                {
                    inUse.Add(name);
                    continue;
                }

                record.UpgradeOpen();
                record.Erase();
                deleted.Add(name);
            }

            transaction.Commit();
            return new LayerReport(deleted, inUse);
        }
    }
}
