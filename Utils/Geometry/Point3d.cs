
using System.Drawing;

namespace Structural_Automation.Utils.Geometry
{
    public class Point3d(double x, double y, double z)
    {
        public double X { get; private set; } = x;
        public double Y { get; private set; } = y;
        public double Z { get; private set; } = z;

        public override bool Equals(object? obj)
        {
            LengthTolerance lengthTolerance = new();
            if (obj is Point3d other)
            {
                return lengthTolerance.AreEqual(X, other.X)
                    && lengthTolerance.AreEqual(Y, other.Y)
                    && lengthTolerance.AreEqual(Z, other.Z);
            }

            return false;
        }

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
