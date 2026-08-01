
namespace Structural_Automation.Utils.SystemParams
{
    public enum LengthUnit
    {
        Inches,
        Meters
    }

    public class Units(LengthUnit unit)
    {
        public LengthUnit Unit { get; private set; } = unit;
    }
}
