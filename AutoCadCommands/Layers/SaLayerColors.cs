
namespace Structural_Automation.AutoCadCommands.Layers
{
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
                SaLayer.SA_FLOOR_PLAN_BBOX
                    or SaLayer.SA_FLOOR_PLAN_BBOX_TEXT
                    or SaLayer.SA_FLOOR_PLAN_ORIGIN => _green,
                _ => throw new ArgumentOutOfRangeException(nameof(layer))
            };
        }
    }
}
