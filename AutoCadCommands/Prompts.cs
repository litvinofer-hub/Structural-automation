
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// What a command asks the user for. Null means the user cancelled, so a command
    /// reads it as "stop here" without inspecting AutoCAD's prompt statuses.
    /// </summary>
    public class Prompts(Editor editor)
    {
        /// <summary>
        /// Asks the user to choose one of the keywords. AutoCAD renders the list itself,
        /// so the message carries no brackets of its own.
        /// </summary>
        public string? AskKeyword(string message, string[] keywords, string chosenByDefault)
        {
            PromptKeywordOptions options = new(message);
            foreach (string keyword in keywords)
            {
                options.Keywords.Add(keyword);
            }

            options.Keywords.Default = chosenByDefault;
            options.AllowNone = true;

            PromptResult result = editor.GetKeywords(options);

            return result.Status == PromptStatus.OK ? result.StringResult : null;
        }

        /// <summary>
        /// Asks for a point. With <paramref name="allowFinish"/> the user can press Enter
        /// to say they are done, which reads the same as cancelling: null, stop asking.
        /// </summary>
        public Point3d? AskPoint(string message, bool allowFinish)
        {
            PromptPointOptions options = new(message) { AllowNone = allowFinish };
            PromptPointResult result = editor.GetPoint(options);

            return result.Status == PromptStatus.OK ? result.Value : null;
        }

        /// <summary>Asks for the corner opposite <paramref name="from"/>, rubber banded.</summary>
        public Point3d? AskCorner(string message, Point3d from)
        {
            PromptCornerOptions options = new(message, from);
            PromptPointResult result = editor.GetCorner(options);

            return result.Status == PromptStatus.OK ? result.Value : null;
        }

        public string? AskText(string message)
        {
            PromptStringOptions options = new(message) { AllowSpaces = true };
            PromptResult result = editor.GetString(options);

            return result.Status == PromptStatus.OK ? result.StringResult : null;
        }

        /// <summary>Asks the user to pick an entity, and only accepts one on the layer.</summary>
        public ObjectId? AskEntityOn(string message, SaLayer layer, Type allowed)
        {
            PromptEntityOptions options = new(message);
            options.SetRejectMessage($"\nMust be a {allowed.Name} on {layer}.");
            options.AddAllowedClass(allowed, exactMatch: false);

            PromptEntityResult result = editor.GetEntity(options);
            if (result.Status != PromptStatus.OK)
            {
                return null;
            }

            using Transaction transaction = editor.Document.TransactionManager.StartTransaction();
            Entity entity = (Entity)transaction.GetObject(result.ObjectId, OpenMode.ForRead);
            bool ours = entity.Layer == layer.ToString();
            transaction.Commit();

            if (!ours)
            {
                editor.WriteMessage($"\nThat is not on {layer}.");
                return null;
            }

            return result.ObjectId;
        }
    }
}
