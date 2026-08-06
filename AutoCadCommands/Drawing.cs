
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// The drawing a command works on. Every command needs a transaction, model space
    /// and somewhere to put an entity, so they ask for those here rather than opening
    /// the block table themselves.
    /// </summary>
    public class Drawing(Document document)
    {
        public Database Database => document.Database;

        public Editor Editor => document.Editor;

        public Transaction Start()
        {
            return document.Database.TransactionManager.StartTransaction();
        }

        public BlockTableRecord ModelSpace(Transaction transaction, OpenMode mode)
        {
            BlockTable blockTable = (BlockTable)transaction.GetObject(document.Database.BlockTableId, OpenMode.ForRead);
            return (BlockTableRecord)transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], mode);
        }

        public List<ObjectId> ModelSpaceIds(Transaction transaction)
        {
            List<ObjectId> ids = [];
            foreach (ObjectId id in ModelSpace(transaction, OpenMode.ForRead))
            {
                ids.Add(id);
            }

            return ids;
        }

        /// <summary>Adds an entity to model space on the given layer.</summary>
        public ObjectId Add(Transaction transaction, Entity entity, SaLayer layer)
        {
            entity.Layer = layer.ToString();

            BlockTableRecord modelSpace = ModelSpace(transaction, OpenMode.ForWrite);
            ObjectId id = modelSpace.AppendEntity(entity);
            transaction.AddNewlyCreatedDBObject(entity, true);

            return id;
        }
    }
}
