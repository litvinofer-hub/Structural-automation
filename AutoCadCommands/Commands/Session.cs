
using Autodesk.AutoCAD.ApplicationServices;
using Structural_Automation.AutoCadCommands.Acad;
using Structural_Automation.AutoCadCommands.Model;
using Structural_Automation.AutoCadCommands.Plans;

namespace Structural_Automation.AutoCadCommands.Commands
{
    /// <summary>
    /// What every command starts from: the drawing it runs against and the plumbing built
    /// around it, assembled once per run. A command names what it needs rather than how to
    /// wire it, so adding one does not mean repeating this.
    /// </summary>
    public class Session
    {
        public Session(Document document, SaLayerColors colors, SaAnnotations annotations)
        {
            Drawing = new Drawing(document);
            Prompts = new Prompts(Drawing.Editor);
            Messages = new Messages(Drawing.Editor);
            Layers = new SaLayerTable(Drawing.Database, colors);
            Plans = new FloorPlans(Drawing);
            Annotations = annotations;
        }

        public Drawing Drawing { get; }

        public Prompts Prompts { get; }

        public Messages Messages { get; }

        public SaLayerTable Layers { get; }

        public FloorPlans Plans { get; }

        public SaAnnotations Annotations { get; }
    }
}
