
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
            if (obj is Point3d other)
            {
                return new LengthTolerance().AreEqual(X, other.X)
                    && new LengthTolerance().AreEqual(Y, other.Y)
                    && new LengthTolerance().AreEqual(Z, other.Z);
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
        /// Checks whether the list contains any two distinct points with equal coordinates.
        /// Uses a HashSet for O(n) performance.
        /// </summary>
        public static bool HasDuplicates(IEnumerable<Point3d> points)
        {
            var seen = new HashSet<Point3d>();

            foreach (var point in points)
            {
                if (!seen.Add(point))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the 2D distance (X, Y only) between this point and another point.
        /// </summary>
        public double Distance2D(Point3d other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
