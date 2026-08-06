
namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// The layers we draw on about a drawing rather than about the building. A command
    /// measuring what a floor plan holds must leave these out, or it measures its own
    /// marks and drifts a little further every run.
    /// </summary>
    public class SaAnnotations
    {
        private readonly SaLayer[] _layers =
        [
            SaLayer.SA_FLOOR_PLAN_BBOX,
            SaLayer.SA_FLOOR_PLAN_BBOX_TEXT,
            SaLayer.SA_FLOOR_PLAN_ORIGIN
        ];

        public bool Includes(SaLayer layer)
        {
            return _layers.Contains(layer);
        }

        /// <summary>True when the layer name is one of ours and one of these.</summary>
        public bool Includes(string layerName)
        {
            return Enum.TryParse(layerName, out SaLayer layer) && Includes(layer);
        }
    }
}
