
namespace Structural_Automation.Utils.Geometry
{
    /// <summary>
    /// An axis-independent rectangle in 3D, defined by one corner and the two
    /// perpendicular edge vectors leading away from it. The lengths of those vectors
    /// are the side dimensions, and their cross product defines the plane the
    /// rectangle lies on, so the rectangle is not restricted to the XY plane.
    /// </summary>
    public class Rectangle
    {
        public Point3d Corner { get; private set; }
        public Vector3d U { get; private set; }
        public Vector3d V { get; private set; }

        /// <param name="corner">One corner of the rectangle.</param>
        /// <param name="u">Edge vector leading away from <paramref name="corner"/>.</param>
        /// <param name="v">Edge vector leading away from <paramref name="corner"/>, perpendicular to <paramref name="u"/>.</param>
        public Rectangle(Point3d corner, Vector3d u, Vector3d v)
        {
            if (u.IsZero() || v.IsZero())
            {
                throw new ArgumentException("A rectangle cannot have a zero-length edge.");
            }

            if (!u.IsPerpendicularTo(v))
            {
                throw new ArgumentException("The two edge vectors of a rectangle must be perpendicular.");
            }

            Corner = corner;
            U = u;
            V = v;
        }

        public double GetWidth()
        {
            return U.GetLength();
        }

        public double GetHeight()
        {
            return V.GetLength();
        }

        public double GetArea()
        {
            return GetWidth() * GetHeight();
        }

        /// <summary>
        /// Returns the unit vector normal to the plane the rectangle lies on.
        /// </summary>
        public Vector3d GetNormal()
        {
            return U.Cross(V).Normalized();
        }

        public Point3d GetCenter()
        {
            return U.Add(V).Divide(2).Translate(Corner);
        }

        /// <summary>
        /// Returns the four corners, walking the perimeter in order starting from <see cref="Corner"/>.
        /// </summary>
        public IReadOnlyList<Point3d> GetCorners()
        {
            return
            [
                Corner,
                U.Translate(Corner),
                U.Add(V).Translate(Corner),
                V.Translate(Corner)
            ];
        }

        /// <summary>
        /// Returns true if both rectangles lie on parallel planes.
        /// </summary>
        public bool IsParallelTo(Rectangle other)
        {
            return GetNormal().IsParallelTo(other.GetNormal());
        }

        /// <summary>
        /// Returns true if both rectangles lie on the same plane.
        /// </summary>
        public bool IsCoplanarWith(Rectangle other)
        {
            if (!IsParallelTo(other))
            {
                return false;
            }

            return new LengthTolerance().AreEqual(new Vector3d(Corner, other.Corner).Dot(GetNormal()), 0);
        }

        /// <summary>
        /// Two rectangles are equal when they cover the same region of space, regardless of
        /// which corner each one was built from or the order of its edge vectors.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Rectangle other)
            {
                IReadOnlyList<Point3d> otherCorners = other.GetCorners();

                // A rectangle always has four distinct corners, so matching every corner
                // of this one against the other's four is enough to pair them all up.
                return GetCorners().All(otherCorners.Contains);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Use addition so that the order of the corners doesn't matter.
                return GetCorners().Sum(corner => corner.GetHashCode());
            }
        }
    }
}
