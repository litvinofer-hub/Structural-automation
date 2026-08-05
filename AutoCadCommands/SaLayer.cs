
namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// The layers the plugin draws on. Each name is the AutoCAD layer name verbatim,
    /// so ToString() yields it.
    /// </summary>
    public enum SaLayer
    {
        SA_WALL,
        SA_WALL_TEXT,
        SA_WINDOW,
        SA_WINDOW_TEXT,
        SA_DOOR,
        SA_DOOR_TEXT,
        SA_BEAM,
        SA_BEAM_TEXT,
        SA_COLUMN,
        SA_COLUMN_TEXT,
        SA_ROOM,
        SA_ROOM_TEXT,
        SA_BBOX,
        SA_BBOX_TEXT
    }

    public class SaLayerColors
    {
        /// <summary>
        /// The AutoCAD Color Index of a layer. A layer and its text layer share a color.
        /// </summary>
        public short ColorIndexOf(SaLayer layer)
        {
            const short blue = 5;
            const short yellow = 2;
            const short pink = 6;
            const short red = 1;
            const short brown = 34;
            const short grey = 8;
            const short green = 3;

            return layer switch
            {
                SaLayer.SA_WALL or SaLayer.SA_WALL_TEXT => blue,
                SaLayer.SA_WINDOW or SaLayer.SA_WINDOW_TEXT => yellow,
                SaLayer.SA_DOOR or SaLayer.SA_DOOR_TEXT => pink,
                SaLayer.SA_BEAM or SaLayer.SA_BEAM_TEXT => red,
                SaLayer.SA_COLUMN or SaLayer.SA_COLUMN_TEXT => brown,
                SaLayer.SA_ROOM or SaLayer.SA_ROOM_TEXT => grey,
                SaLayer.SA_BBOX or SaLayer.SA_BBOX_TEXT => green,
                _ => throw new ArgumentOutOfRangeException(nameof(layer))
            };
        }
    }
}
