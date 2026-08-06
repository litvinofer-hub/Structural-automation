
using Autodesk.AutoCAD.EditorInput;
using Structural_Automation.AutoCadCommands.Layers;

namespace Structural_Automation.AutoCadCommands.Acad
{
    /// <summary>
    /// What a command tells the user. Every line is written the same way, so a command
    /// says what happened without repeating the newline and the word Warning.
    /// </summary>
    public class Messages(Editor editor)
    {
        public void Say(string line)
        {
            editor.WriteMessage($"\n{line}");
        }

        public void Warn(string line)
        {
            editor.WriteMessage($"\nWarning: {line}");
        }

        /// <summary>
        /// Says what an operation did to the layers, and warns by name about the ones it
        /// left alone. Both layer commands report the same shape and differ only in words.
        /// </summary>
        public void Report(LayerReport report, string applied, string skipped)
        {
            Say($"{report.Applied.Count} layer(s) {applied}.");

            if (report.Skipped.Count > 0)
            {
                Warn($"{report.Skipped.Count} layer(s) {skipped}: {string.Join(", ", report.Skipped)}");
            }
        }
    }
}
