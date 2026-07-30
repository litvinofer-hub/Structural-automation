
namespace Structural_Automation.Utils.Geometry
{
    public class LengthTolerance(double tolerance = 1e-6)
    {
        public double Tolerance { get; } = tolerance;

        /// <summary>
        /// Returns true if a and b are equal within the tolerance.
        /// </summary>
        public bool AreEqual(double a, double b)
        {
            return Math.Abs(a - b) < Tolerance;
        }

        /// <summary>
        /// Returns true if a is greater than b, beyond the tolerance.
        /// </summary>
        public bool IsGreaterThan(double a, double b)
        {
            return a - b > Tolerance;
        }

        /// <summary>
        /// Returns true if a is less than b, beyond the tolerance.
        /// </summary>
        public bool IsLessThan(double a, double b)
        {
            return b - a > Tolerance;
        }

        /// <summary>
        /// Returns true if a is greater than or equal to b, within the tolerance.
        /// </summary>
        public bool IsGreaterThanOrEqual(double a, double b)
        {
            return a - b > -Tolerance;
        }

        /// <summary>
        /// Returns true if a is less than or equal to b, within the tolerance.
        /// </summary>
        public bool IsLessThanOrEqual(double a, double b)
        {
            return b - a > -Tolerance;
        }

        /// <summary>
        /// Returns true if a and b lie on opposite sides of zero, beyond the tolerance.
        /// A value within the tolerance of zero counts as neither side, so it is false.
        /// </summary>
        public bool AreOppositeSigns(double a, double b)
        {
            return (IsGreaterThan(a, 0) && IsLessThan(b, 0))
                || (IsLessThan(a, 0) && IsGreaterThan(b, 0));
        }
    }
}
