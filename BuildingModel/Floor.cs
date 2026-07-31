using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    /// <summary>
    /// A horizontal slab, described by the polygon running through the middle of it.
    /// That polygon lies flat, on a plane parallel to the XY plane, and may be any
    /// closed outline. Thickness spreads half above and half below.
    /// </summary>
    public class Floor
    {
        public Guid Id { get; private set; }

        /// <summary>
        /// The polygon through the middle of the floor, halfway through its thickness.
        /// </summary>
        public Polygon MidPolygon { get; private set; }

        public double Thickness { get; private set; }

        public Floor(Polygon midPolygon, double thickness)
        {
            if (!midPolygon.GetNormal().IsParallelTo(new Vector3d(0, 0, 1)))
            {
                throw new ArgumentException("A floor's mid-polygon must be horizontal, so its plane has to be parallel to the XY plane.");
            }

            if (!new LengthTolerance().IsGreaterThan(thickness, 0))
            {
                throw new ArgumentException("A floor thickness must be greater than zero.");
            }

            Id = Guid.NewGuid();
            MidPolygon = midPolygon;
            Thickness = thickness;
        }

        /// <summary>
        /// Returns the vertices of the mid-polygon, in order around its outline.
        /// </summary>
        public IEnumerable<Point3d> GetPoints()
        {
            return MidPolygon.Vertices;
        }

        /// <summary>
        /// Two floors are equal when they occupy the same space: same mid-polygon,
        /// regardless of where its outline starts or which way round it runs, and
        /// same thickness.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Floor other)
            {
                return MidPolygon.Equals(other.MidPolygon)
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
                hash = hash * 31 + MidPolygon.GetHashCode();
                hash = hash * 31 + Math.Round(Thickness * roundFactor).GetHashCode();
                return hash;
            }
        }
    }
}
