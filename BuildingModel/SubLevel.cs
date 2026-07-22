using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    public class SubLevel(double offset)
    {
        /// <summary>
        /// Offset relative to the parent Level's Elevation.
        /// </summary>
        public double Offset { get; private set; } = offset;

        public override bool Equals(object? obj)
        {
            if (obj is SubLevel other)
            {
                return new LengthTolerance().AreEqual(Offset, other.Offset);
            }

            return false;
        }

        public override int GetHashCode()
        {
            double roundFactor = 1.0 / new LengthTolerance().Tolerance;
            return Math.Round(Offset * roundFactor).GetHashCode();
        }
    }
}
