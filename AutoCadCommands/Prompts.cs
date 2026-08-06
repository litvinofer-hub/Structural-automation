
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
        /// <summary>Asks for a point, allowing the given keywords to be typed instead.</summary>
        public PromptPointResult AskPointOrKeyword(string message, params string[] keywords)
        {
            PromptPointOptions options = new(message);
            foreach (string keyword in keywords)
            {
                options.Keywords.Add(keyword);
            }

            return editor.GetPoint(options);
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

        /// <summary>Asks the user to pick a polyline, and only accepts one on the layer.</summary>
        public ObjectId? AskPolylineOn(string message, SaLayer layer)
        {
            PromptEntityOptions options = new(message);
            options.SetRejectMessage($"\nMust be a rectangle on {layer}.");
            options.AddAllowedClass(typeof(Polyline), exactMatch: false);

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
                editor.WriteMessage($"\nThat rectangle is not on {layer}.");
                return null;
            }

            return result.ObjectId;
        }
    }
}
