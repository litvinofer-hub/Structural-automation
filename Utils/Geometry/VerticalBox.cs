
namespace Structural_Automation.Utils.Geometry
{
    /// <summary>
    /// A <see cref="Box"/> standing upright, described by the rectangle running through
    /// the middle of it. That rectangle has one edge parallel to Z, giving the height, and
    /// the other parallel to the XY plane, giving the length, so the box may still face
    /// any direction. Thickness spreads half to either side of the rectangle.
    /// <para>
    /// The mid-surface's two edges are kept as they were given, in <see cref="Box.U"/> and
    /// <see cref="Box.V"/>, and the third edge <see cref="Box.W"/> is always the thickness.
    /// Which of the first two is the height is a question of which one is parallel to Z,
    /// and it is answered when asked rather than settled here.
    /// </para>
    /// </summary>
    public class VerticalBox : Box
    {
        /// <param name="midSurface">The rectangle through the middle of the box, standing vertically.</param>
        /// <param name="thickness">Spreads half to either side of <paramref name="midSurface"/>.</param>
        public VerticalBox(Rectangle midSurface, double thickness)
            // The mid-surface runs through the middle, so the box starts half a thickness
            // back from it, and its third edge crosses from that face to the other.
            : base(midSurface.GetNormal().Multiply(thickness / 2).Negate().Translate(midSurface.Corner),
                   midSurface.U,
                   midSurface.V,
                   midSurface.GetNormal().Multiply(thickness))
        {
            Vector3d up = new(0, 0, 1);

            if (midSurface.U.IsParallelTo(up) == midSurface.V.IsParallelTo(up))
            {
                throw new ArgumentException("A vertical box's mid-surface must have exactly one edge parallel to Z.");
            }

            // A thickness of zero leaves the box with a zero-length edge, so the base
            // constructor has already turned it away by the time we get here. What is left
            // to catch is a negative one, which would otherwise pass as its own opposite.
            if (!new LengthTolerance().IsGreaterThan(thickness, 0))
            {
                throw new ArgumentException("A vertical box's thickness must be greater than zero.");
            }
        }

        /// <summary>
        /// The rectangle through the middle of the box, halfway through its thickness.
        /// </summary>
        public Rectangle MidSurface => new(W.Divide(2).Translate(Corner), U, V);

        public double Thickness => W.GetLength();

        /// <summary>
        /// Returns the mid-surface edge running along the box, horizontal.
        /// </summary>
        private Vector3d GetLengthVector()
        {
            return U.IsParallelTo(new Vector3d(0, 0, 1)) ? V : U;
        }

        /// <summary>
        /// Returns the mid-surface edge running up the box, parallel to Z. It follows the
        /// mid-surface as it was given, so it points down whenever the mid-surface was
        /// drawn from a top corner.
        /// </summary>
        private Vector3d GetHeightVector()
        {
            return U.IsParallelTo(new Vector3d(0, 0, 1)) ? U : V;
        }

        public double GetLength()
        {
            return GetLengthVector().GetLength();
        }

        public double GetHeight()
        {
            return GetHeightVector().GetLength();
        }

        /// <summary>
        /// Returns the unit vector running along the length of the box, horizontal.
        /// </summary>
        public Vector3d GetDirection()
        {
            return GetLengthVector().Normalized();
        }

        /// <summary>
        /// Returns the unit vector running across the box, horizontal and perpendicular to
        /// the mid-surface.
        /// </summary>
        public Vector3d GetThicknessDirection()
        {
            return W.Normalized();
        }

        /// <summary>
        /// Returns the lower of the two corners the height edge joins.
        /// </summary>
        private Point3d GetLowestCorner()
        {
            Vector3d height = GetHeightVector();

            return height.Z > 0 ? Corner : height.Translate(Corner);
        }

        /// <summary>
        /// Returns the Z coordinate of the underside.
        /// </summary>
        public double GetBaseElevation()
        {
            return GetLowestCorner().Z;
        }

        /// <summary>
        /// Returns the underside of the box.
        /// </summary>
        public Rectangle GetBottomRectangle()
        {
            return new Rectangle(GetLowestCorner(), GetLengthVector(), W);
        }

        /// <summary>
        /// Returns the top of the box, which is the underside raised by the height.
        /// </summary>
        public Rectangle GetTopRectangle()
        {
            Point3d corner = new Vector3d(0, 0, GetHeight()).Translate(GetLowestCorner());

            return new Rectangle(corner, GetLengthVector(), W);
        }
    }
}
