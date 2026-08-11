
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Structural_Automation.AutoCadCommands.Model
{
    /// <summary>
    /// One floor plan: the rectangle around it, what it is called, and the ground it
    /// covers. Carrying the bounds means a caller can test a point without reopening
    /// the rectangle.
    /// </summary>
    public sealed record FloorPlan(ObjectId Box, string Name, Extents3d Bounds)
    {
        /// <summary>
        /// How much ground the plan covers. Where boxes overlap, the smaller one owns what
        /// they share, so a point belongs to one plan and not two.
        /// </summary>
        public double Area =>
            (Bounds.MaxPoint.X - Bounds.MinPoint.X) * (Bounds.MaxPoint.Y - Bounds.MinPoint.Y);

        /// <summary>
        /// Whether the point falls on this floor plan, in X and Y alone. A plan is a plan,
        /// so elevation should not decide what belongs to it.
        /// </summary>
        public bool Covers(Point3d point)
        {
            return point.X >= Bounds.MinPoint.X
                && point.X <= Bounds.MaxPoint.X
                && point.Y >= Bounds.MinPoint.Y
                && point.Y <= Bounds.MaxPoint.Y;
        }

        /// <summary>
        /// Whether the point is this plan's rather than another's. A box reaching over its
        /// neighbour covers the neighbour's ground, but the smaller plan owns it.
        /// </summary>
        public bool Owns(Point3d point, IEnumerable<FloorPlan> others)
        {
            if (!Covers(point))
            {
                return false;
            }

            foreach (FloorPlan other in others)
            {
                if (other.Covers(point) && other.Area < Area)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
