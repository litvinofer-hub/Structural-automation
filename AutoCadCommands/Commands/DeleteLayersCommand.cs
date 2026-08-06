
namespace Structural_Automation.AutoCadCommands.Commands
{
    /// <summary>Deletes the layers nothing uses, and names the ones still in use.</summary>
    public class DeleteLayersCommand(Session session)
    {
        public void Run()
        {
            session.Messages.Report(
                session.Layers.Delete(),
                "deleted",
                "are still in use and were kept - move or erase what is drawn on them first");
        }
    }
}
