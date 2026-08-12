using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    public enum OpeningType
    {
        Window,
        Door,
        Void
    }

    /// <summary>
    /// A void cut through a wall, shaped and placed like a small wall of its own: a
    /// vertical mid-surface with a thickness spreading half to either side. Its type says
    /// what the void is for, but every type is the same void geometrically.
    /// <para>
    /// Openings are only ever cut through walls, and the name is short for that. Nothing
    /// here assumes a wall though, and nothing here can reach one: an opening holds global
    /// coordinates and knows nothing of what it is cut into, which is what lets a wall
    /// depend on its openings rather than the other way round. Anything measured against
    /// the wall, such as the sill height or how far along the wall the opening sits, is
    /// therefore asked of the wall — see <see cref="Wall.GetSillHeight"/> and
    /// <see cref="Wall.GetDistanceAlong"/>. Whether the opening actually fits in the wall
    /// is the wall's question too — see <see cref="Wall.Contains"/>.
    /// </para>
    /// </summary>
    public class Opening(Rectangle midSurface, double thickness, OpeningType type)
        : VerticalBox(midSurface, thickness), IFlattenable
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public OpeningType Type { get; private set; } = type;

        /// <summary>
        /// Returns the four corners of the mid-surface, the opening flattened. For the
        /// eight corners of the void itself, see <see cref="Box.GetCorners"/>.
        /// </summary>
        public IEnumerable<Point3d> GetFlatBuildingPoints()
        {
            return MidSurface.GetCorners();
        }

        /// <summary>
        /// Two openings are equal when they fill the same space and are of the same type,
        /// so a door and a window over one space are different openings. The identifier
        /// plays no part, so two openings created separately over the same space are equal.
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is Opening other
                && base.Equals(obj)
                && Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Type);
        }
    }
}
