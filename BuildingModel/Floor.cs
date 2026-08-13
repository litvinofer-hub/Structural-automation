using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    /// <summary>
    /// A horizontal slab, described by the polygon running through the middle of it.
    /// That polygon lies flat, on a plane parallel to the XY plane, and may be any
    /// closed outline. Thickness spreads half above and half below.
    /// </summary>
    public class Floor : IFlattenable
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
        /// Returns the vertices of the mid-polygon, in order around its outline. That
        /// polygon is the floor flattened, one dimension short of the slab itself.
        /// </summary>
        public IEnumerable<Point3d> GetFlatBuildingPoints()
        {
            return MidPolygon.Vertices;
        }

        public override bool Equals(object? obj)
        {
            return obj is Floor other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
