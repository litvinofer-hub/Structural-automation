
namespace Structural_Automation.Utils
{
    public enum LengthUnit
    {
        Inches,
        Meters
    }

    public class UnitSystem(LengthUnit unit)
    {
        public LengthUnit Unit { get; private set; } = unit;
    }
}
