
namespace Structural_Automation.Utils.Geometry
{
    /// <summary>
    /// Tolerance for comparing angles between directions.
    /// </summary>
    public class AngleTolerance(double tolerance = 1e-9)
    {
        public double Tolerance { get; } = tolerance;

        /// <summary>
        /// Returns true if the sine or cosine of an angle is close enough to zero to
        /// treat the angle as exactly 0 or 90 degrees.
        /// </summary>
        public bool IsZeroAngle(double sineOrCosine)
        {
            return Math.Abs(sineOrCosine) < Tolerance;
        }
    }
}
