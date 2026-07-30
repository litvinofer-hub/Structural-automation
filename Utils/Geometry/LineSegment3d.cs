
using System.Drawing;
using System.Net;

namespace Structural_Automation.Utils.Geometry
{
    public class LineSegment3d(Point3d start, Point3d end)
    {
        public Point3d Start { get; private set; } = start;
        public Point3d End { get; private set; } = end;

        public override bool Equals(object? obj)
        {
            if (obj is LineSegment3d other)
            {
                bool sameDirection = Start.Equals(other.Start) && End.Equals(other.End);
                bool reversed = Start.Equals(other.End) && End.Equals(other.Start);
                return sameDirection || reversed;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Use addition so that order doesn't matter (A+B == B+A)
                return Start.GetHashCode() + End.GetHashCode();
            }
        }

        public bool IsPointOnSegment(Point3d point)
        {
            double dStartToPoint = Start.Distance(point);
            double dPointToEnd = point.Distance(End);
            double dTotal = Start.Distance(End);

            return new LengthTolerance().AreEqual(dStartToPoint + dPointToEnd, dTotal);
        }
    }
}
