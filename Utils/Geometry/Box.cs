
namespace Structural_Automation.Utils.Geometry
{
    /// <summary>
    /// A rectangular box in 3D, defined by one corner and the three perpendicular edge
    /// vectors leading away from it. The lengths of those vectors are the three
    /// dimensions, and their directions let the box sit at any angle, so it is not
    /// restricted to the coordinate axes.
    /// </summary>
    public class Box
    {
        public Point3d Corner { get; private set; }
        public Vector3d U { get; private set; }
        public Vector3d V { get; private set; }
        public Vector3d W { get; private set; }

        /// <param name="corner">One corner of the box.</param>
        /// <param name="u">Edge vector leading away from <paramref name="corner"/>.</param>
        /// <param name="v">Edge vector leading away from <paramref name="corner"/>, perpendicular to <paramref name="u"/>.</param>
        /// <param name="w">Edge vector leading away from <paramref name="corner"/>, perpendicular to both others.</param>
        public Box(Point3d corner, Vector3d u, Vector3d v, Vector3d w)
        {
            if (u.IsZero() || v.IsZero() || w.IsZero())
            {
                throw new ArgumentException("A box cannot have a zero-length edge.");
            }

            if (!u.IsPerpendicularTo(v) || !v.IsPerpendicularTo(w) || !w.IsPerpendicularTo(u))
            {
                throw new ArgumentException("The three edge vectors of a box must be perpendicular to each other.");
            }

            Corner = corner;
            U = u;
            V = v;
            W = w;
        }

        public double GetVolume()
        {
            return U.GetLength() * V.GetLength() * W.GetLength();
        }

        public Point3d GetCenter()
        {
            return U.Add(V).Add(W).Divide(2).Translate(Corner);
        }

        /// <summary>
        /// Returns the eight corners of the box.
        /// </summary>
        public IReadOnlyList<Point3d> GetCorners()
        {
            return
            [
                Corner,
                U.Translate(Corner),
                V.Translate(Corner),
                W.Translate(Corner),
                U.Add(V).Translate(Corner),
                U.Add(W).Translate(Corner),
                V.Add(W).Translate(Corner),
                U.Add(V).Add(W).Translate(Corner)
            ];
        }

        /// <summary>
        /// Two boxes are equal when they fill the same space, regardless of which corner
        /// each one was built from or the order of its edge vectors.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Box other)
            {
                IReadOnlyList<Point3d> otherCorners = other.GetCorners();

                // A box always has eight distinct corners, so matching every corner of
                // this one against the other's eight is enough to pair them all up.
                return GetCorners().All(otherCorners.Contains);
            }

            return false;
        }

        public override int GetHashCode()
        {
            // Sort so that the order the corners were built in cannot affect the result,
            // while still combining them with a proper mixing step rather than adding,
            // which collapses into a linear combination.
            int[] cornerHashes = [.. GetCorners().Select(corner => corner.GetHashCode())];
            Array.Sort(cornerHashes);

            unchecked
            {
                int hash = 17;
                foreach (int cornerHash in cornerHashes)
                {
                    hash = hash * 31 + cornerHash;
                }

                return hash;
            }
        }
    }
}
