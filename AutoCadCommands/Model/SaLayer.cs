
namespace Structural_Automation.AutoCadCommands.Model
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
        SA_FLOOR_PLAN_BBOX,
        SA_FLOOR_PLAN_BBOX_TEXT,
        SA_FLOOR_PLAN_ORIGIN
    }
}
