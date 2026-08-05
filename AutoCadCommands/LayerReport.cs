
namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// What a layer operation did: the layers it acted on, and the ones it left alone.
    /// Holding both lets the command say what happened without repeating the work.
    /// </summary>
    public sealed record LayerReport(IReadOnlyList<string> Applied, IReadOnlyList<string> Skipped);
}
