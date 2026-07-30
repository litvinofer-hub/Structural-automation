
using System.Drawing;

namespace Structural_Automation.Utils.Geometry
{
    public class Point3d
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        /// <summary>
        /// Coordinates are snapped to the length tolerance grid, so two points that
        /// compare equal always hold identical values and therefore hash identically.
        /// Without that, coordinates falling either side of a rounding boundary could
        /// compare equal yet land in different hash buckets.
        /// </summary>
        public Point3d(double x, double y, double z)
        {
            double roundFactor = 1.0 / new LengthTolerance().Tolerance;

            X = Math.Round(x * roundFactor) / roundFactor;
            Y = Math.Round(y * roundFactor) / roundFactor;
            Z = Math.Round(z * roundFactor) / roundFactor;
        }

        /// <summary>
        /// Coordinates are already snapped to the tolerance grid, so comparing them
        /// exactly is what applies the tolerance. Comparing with a tolerance again
        /// here would call neighbouring grid values equal while they hash apart.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Point3d other)
            {
                return X == other.X
                    && Y == other.Y
                    && Z == other.Z;
            }

            return false;
        }

        /// <summary>
        /// Hashes the coordinates scaled up to their grid index. Hashing the raw
        /// doubles instead distributes badly, because the bit patterns of small
        /// values differ too little to survive the mixing step.
        /// </summary>
        public override int GetHashCode()
        {
            double roundFactor = 1.0 / new LengthTolerance().Tolerance;
            int hx = (Math.Round(X * roundFactor)).GetHashCode();
            int hy = (Math.Round(Y * roundFactor)).GetHashCode();
            int hz = (Math.Round(Z * roundFactor)).GetHashCode();

            unchecked
            {
                int hash = 17;
                hash = hash * 31 + hx;
                hash = hash * 31 + hy;
                hash = hash * 31 + hz;
                return hash;
            }
        }

        /// <summary>
        /// Returns the distance between this point and another point.
        /// </summary>
        public double Distance(Point3d other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            double dz = Z - other.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
