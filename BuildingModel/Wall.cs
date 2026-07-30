using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    /// <summary>
    /// A vertical rectangular wall, described by the rectangle running through the
    /// middle of the box. That rectangle stands vertically, with one edge parallel to
    /// Z giving the height and the other parallel to the XY plane giving the length,
    /// so the wall length may sit at any angle in XY plane. Thickness spreads half to either side.
    /// </summary>
    public class Wall
    {
        public Guid Id { get; private set; }

        /// <summary>
        /// The rectangle through the middle of the wall, halfway through its thickness.
        /// </summary>
        public Rectangle MidSurface { get; private set; }

        public double Thickness { get; private set; }

        public Wall(Rectangle midSurface, double thickness)
        {
            Vector3d up = new(0, 0, 1);
            bool isFirstEdgeVertical = midSurface.U.IsParallelTo(up);

            if (isFirstEdgeVertical == midSurface.V.IsParallelTo(up))
            {
                throw new ArgumentException("A wall's mid-surface must have exactly one edge parallel to Z.");
            }

            Vector3d alongLength = isFirstEdgeVertical ? midSurface.V : midSurface.U;

            if (!alongLength.IsPerpendicularTo(up))
            {
                throw new ArgumentException("A wall's mid-surface must have its other edge parallel to the XY plane.");
            }

            if (!new LengthTolerance().IsGreaterThan(thickness, 0))
            {
                throw new ArgumentException("A wall thickness must be greater than zero.");
            }

            Id = Guid.NewGuid();
            MidSurface = midSurface;
            Thickness = thickness;
        }

        /// <summary>
        /// Returns the mid-surface edge running along the wall, horizontal.
        /// </summary>
        private Vector3d GetLengthVector()
        {
            return MidSurface.U.IsParallelTo(new Vector3d(0, 0, 1)) ? MidSurface.V : MidSurface.U;
        }

        /// <summary>
        /// Returns the mid-surface edge running up the wall, parallel to Z.
        /// </summary>
        private Vector3d GetHeightVector()
        {
            return MidSurface.U.IsParallelTo(new Vector3d(0, 0, 1)) ? MidSurface.U : MidSurface.V;
        }

        public double GetLength()
        {
            return GetLengthVector().GetLength();
        }

        public double GetHeight()
        {
            return GetHeightVector().GetLength();
        }

        public double GetVolume()
        {
            return GetLength() * GetHeight() * Thickness;
        }

        /// <summary>
        /// Returns the unit vector running along the length of the wall, horizontal.
        /// </summary>
        public Vector3d GetDirection()
        {
            return GetLengthVector().Normalized();
        }

        /// <summary>
        /// Returns the unit vector running across the wall, horizontal and
        /// perpendicular to the mid-surface.
        /// </summary>
        public Vector3d GetThicknessDirection()
        {
            return MidSurface.GetNormal();
        }

        /// <summary>
        /// Returns the underside of the wall.
        /// </summary>
        public Rectangle GetBottomRectangle()
        {
            Vector3d height = GetHeightVector();
            Point3d lowestCorner = height.Z > 0 ? MidSurface.Corner : height.Translate(MidSurface.Corner);

            Vector3d across = GetThicknessDirection().Multiply(Thickness);
            Point3d corner = across.Divide(2).Negate().Translate(lowestCorner);

            return new Rectangle(corner, GetLengthVector(), across);
        }

        /// <summary>
        /// Returns the top of the wall, which is the underside raised by the height.
        /// </summary>
        public Rectangle GetTopRectangle()
        {
            Rectangle bottom = GetBottomRectangle();
            Point3d corner = new Vector3d(0, 0, GetHeight()).Translate(bottom.Corner);

            return new Rectangle(corner, bottom.U, bottom.V);
        }

        /// <summary>
        /// Returns the four corners of the mid-surface.
        /// </summary>
        public IEnumerable<Point3d> GetPoints()
        {
            return MidSurface.GetCorners();
        }

        /// <summary>
        /// Two walls are equal when they occupy the same space: same mid-surface,
        /// regardless of which corner it was built from, and same thickness.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Wall other)
            {
                return MidSurface.Equals(other.MidSurface)
                    && new LengthTolerance().AreEqual(Thickness, other.Thickness);
            }

            return false;
        }

        public override int GetHashCode()
        {
            double roundFactor = 1.0 / new LengthTolerance().Tolerance;

            unchecked
            {
                int hash = 17;
                hash = hash * 31 + MidSurface.GetHashCode();
                hash = hash * 31 + Math.Round(Thickness * roundFactor).GetHashCode();
                return hash;
            }
        }
    }
}
