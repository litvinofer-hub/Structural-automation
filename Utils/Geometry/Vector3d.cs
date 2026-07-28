
namespace Structural_Automation.Utils.Geometry
{
    public class Vector3d(double x, double y, double z)
    {
        public double X { get; private set; } = x;
        public double Y { get; private set; } = y;
        public double Z { get; private set; } = z;

        /// <summary>
        /// Creates the vector leading from <paramref name="start"/> to <paramref name="end"/>.
        /// </summary>
        public Vector3d(Point3d start, Point3d end)
            : this(end.X - start.X, end.Y - start.Y, end.Z - start.Z)
        {
        }

        /// <summary>
        /// Returns the given point translated by this vector.
        /// </summary>
        public Point3d Translate(Point3d point)
        {
            return new Point3d(point.X + X, point.Y + Y, point.Z + Z);
        }

        public double GetLength()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public bool IsZero()
        {
            return new LengthTolerance().AreEqual(GetLength(), 0);
        }

        public override bool Equals(object? obj)
        {
            LengthTolerance lengthTolerance = new();
            if (obj is Vector3d other)
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
        /// Returns a unit vector in the same direction. Throws if this vector has zero length.
        /// </summary>
        public Vector3d Normalized()
        {
            double length = GetLength();

            if (new LengthTolerance().AreEqual(length, 0))
            {
                throw new InvalidOperationException("Cannot normalize a zero-length vector.");
            }

            return new Vector3d(X / length, Y / length, Z / length);
        }

        public double Dot(Vector3d other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public Vector3d Cross(Vector3d other)
        {
            return new Vector3d(
                Y * other.Z - Z * other.Y,
                Z * other.X - X * other.Z,
                X * other.Y - Y * other.X);
        }

        /// <summary>
        /// Returns true if the two vectors are perpendicular.
        /// Both vectors are normalized first, so the tolerance applies to the angle
        /// between them rather than to their magnitudes.
        /// </summary>
        public bool IsPerpendicularTo(Vector3d other)
        {
            if (IsZero() || other.IsZero())
            {
                return false;
            }

            return new LengthTolerance().AreEqual(Normalized().Dot(other.Normalized()), 0);
        }

        /// <summary>
        /// Returns true if the two vectors are parallel, in either direction.
        /// Both vectors are normalized first, so the tolerance applies to the angle
        /// between them rather than to their magnitudes.
        /// </summary>
        public bool IsParallelTo(Vector3d other)
        {
            if (IsZero() || other.IsZero())
            {
                return false;
            }

            return new LengthTolerance().AreEqual(Normalized().Cross(other.Normalized()).GetLength(), 0);
        }

        public Vector3d Add(Vector3d other)
        {
            return new Vector3d(X + other.X, Y + other.Y, Z + other.Z);
        }

        public Vector3d Subtract(Vector3d other)
        {
            return new Vector3d(X - other.X, Y - other.Y, Z - other.Z);
        }

        /// <summary>
        /// Returns a vector of the same length pointing in the opposite direction.
        /// </summary>
        public Vector3d Negate()
        {
            return new Vector3d(-X, -Y, -Z);
        }

        public Vector3d Multiply(double scalar)
        {
            return new Vector3d(X * scalar, Y * scalar, Z * scalar);
        }

        public Vector3d Divide(double scalar)
        {
            if (new LengthTolerance().AreEqual(scalar, 0))
            {
                throw new DivideByZeroException("Cannot divide a vector by zero.");
            }

            return new Vector3d(X / scalar, Y / scalar, Z / scalar);
        }
    }
}
