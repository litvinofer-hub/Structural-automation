
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
                if (layerTable.Has(layer.ToString()))
                {
                    kept.Add(layer.ToString());
                    continue;
                }

                Add(transaction, layerTable, layer);
                created.Add(layer.ToString());
            }

            transaction.Commit();
            return new LayerReport(created, kept);
        }

        /// <summary>
        /// Makes sure one layer is there, for a command that is about to draw on it.
        /// True when it had to be created.
        /// </summary>
        public bool Ensure(SaLayer layer)
        {
            using Transaction transaction = database.TransactionManager.StartTransaction();
            LayerTable layerTable = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

            if (layerTable.Has(layer.ToString()))
            {
                transaction.Commit();
                return false;
            }

            Add(transaction, layerTable, layer);
            transaction.Commit();
            return true;
        }

        /// <summary>Adds one layer in our palette to a table already open for read.</summary>
        private void Add(Transaction transaction, LayerTable layerTable, SaLayer layer)
        {
            LayerTableRecord record = new()
            {
                Name = layer.ToString(),
                Color = AcadColor.FromColorIndex(ColorMethod.ByAci, colors.ColorIndexOf(layer))
            };

            layerTable.UpgradeOpen();
            layerTable.Add(record);
            transaction.AddNewlyCreatedDBObject(record, true);
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

            ObjectIdCollection erasable = Purgeable(ours);

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

        /// <summary>
        /// Which of the given ids nothing refers to. Purge narrows the collection it is
        /// handed, and asking it first is what keeps an erase from throwing and losing
        /// the whole transaction.
        /// </summary>
        private ObjectIdCollection Purgeable(ObjectIdCollection ids)
        {
            ObjectIdCollection purgeable = new([.. ids.Cast<ObjectId>()]);
            database.Purge(purgeable);

            return purgeable;
        }
    }
}
