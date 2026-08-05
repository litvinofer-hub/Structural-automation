
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

    /// <summary>
    /// The palette the layers are drawn in, as AutoCAD Color Indices. Swap an instance
    /// for another and every layer follows.
    /// </summary>
    public class SaLayerColors
    {
        private readonly short _blue = 5;
        private readonly short _yellow = 2;
        private readonly short _pink = 6;
        private readonly short _red = 1;
        private readonly short _brown = 34;
        private readonly short _grey = 8;
        private readonly short _green = 3;

        /// <summary>
        /// The AutoCAD Color Index of a layer. A layer and its text layer share a color.
        /// </summary>
        public short ColorIndexOf(SaLayer layer)
        {
            return layer switch
            {
                SaLayer.SA_WALL or SaLayer.SA_WALL_TEXT => _blue,
                SaLayer.SA_WINDOW or SaLayer.SA_WINDOW_TEXT => _yellow,
                SaLayer.SA_DOOR or SaLayer.SA_DOOR_TEXT => _pink,
                SaLayer.SA_BEAM or SaLayer.SA_BEAM_TEXT => _red,
                SaLayer.SA_COLUMN or SaLayer.SA_COLUMN_TEXT => _brown,
                SaLayer.SA_ROOM or SaLayer.SA_ROOM_TEXT => _grey,
                SaLayer.SA_BBOX or SaLayer.SA_BBOX_TEXT => _green,
                _ => throw new ArgumentOutOfRangeException(nameof(layer))
            };
        }
    }
}
