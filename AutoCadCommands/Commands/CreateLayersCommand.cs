
namespace Structural_Automation.AutoCadCommands.Commands
{
    /// <summary>Creates the layers the drawing lacks, and names the ones it kept.</summary>
    public class CreateLayersCommand(Session session)
    {
        public void Run()
        {
            session.Messages.Report(
                session.Layers.Create(),
                "created",
                "were already in the drawing and were left as they are, "
                    + "so their colour may not be the one we expect");
        }
    }
}
