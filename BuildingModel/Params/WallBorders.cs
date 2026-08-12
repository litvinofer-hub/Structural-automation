using Structural_Automation.Utils.SystemParams;

namespace Structural_Automation.BuildingModel.Params
{
    /// <summary>
    /// The default strip of wall to leave around an opening, for the unit the model is
    /// drawn in. A wall starts with these and can be given its own instead.
    /// <para>
    /// These live here rather than in <c>Utils.SystemParams</c> because how much wall has
    /// to be left around a hole is a building question, not a geometric one. Utils knows
    /// what a unit is; only the building model knows what a wall needs. Read them the way
    /// a tolerance is read, by making one where it is needed.
    /// </para>
    /// </summary>
    public class WallBorders(LengthUnit unit)
    {
        /// <summary>
        /// How much wall to leave above an opening.
        /// </summary>
        public double Top => unit switch
        {
            LengthUnit.Inches => 12,
            LengthUnit.Meters => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "No default wall borders for this unit.")
        };

        /// <summary>
        /// How much wall to leave below an opening. Nothing, so that an opening is free to
        /// reach the bottom of the wall, as a door does.
        /// </summary>
        public double Bottom => unit switch
        {
            LengthUnit.Inches => 0,
            LengthUnit.Meters => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "No default wall borders for this unit.")
        };

        /// <summary>
        /// How much wall to leave beside an opening, at either end.
        /// </summary>
        public double Side => unit switch
        {
            LengthUnit.Inches => 12,
            LengthUnit.Meters => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "No default wall borders for this unit.")
        };

        /// <summary>
        /// How much wall to leave between one opening and the next.
        /// </summary>
        public double Middle => unit switch
        {
            LengthUnit.Inches => 12,
            LengthUnit.Meters => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "No default wall borders for this unit.")
        };
    }
}
