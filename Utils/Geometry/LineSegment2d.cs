
namespace Structural_Automation.Utils.Geometry
{
    /// <summary>
    /// A straight segment between two points in a 2D coordinate system. Used for work
    /// that has already been reduced to two dimensions, such as testing an outline
    /// projected onto its own plane, where the third dimension carries no information.
    /// </summary>
    public class LineSegment2d((double X, double Y) start, (double X, double Y) end)
    {
        public (double X, double Y) Start { get; private set; } = start;
        public (double X, double Y) End { get; private set; } = end;

        public double GetLength()
        {
            double runX = End.X - Start.X;
            double runY = End.Y - Start.Y;

            return Math.Sqrt(runX * runX + runY * runY);
        }

        /// <summary>
        /// Returns how far the point sits from the infinite line through this segment,
        /// signed so that the two sides of the line give opposite results. Dividing by
        /// the segment's length keeps the result a length, so the length tolerance
        /// applies to it directly.
        /// </summary>
        public double GetDistanceFromLine((double X, double Y) point)
        {
            double runX = End.X - Start.X;
            double runY = End.Y - Start.Y;

            return (runX * (point.Y - Start.Y) - runY * (point.X - Start.X)) / GetLength();
        }

        /// <summary>
        /// Returns true if the point lies on the segment, ends included.
        /// </summary>
        public bool Contains((double X, double Y) point)
        {
            LengthTolerance lengthTolerance = new();

            if (!lengthTolerance.AreEqual(GetDistanceFromLine(point), 0))
            {
                return false;
            }

            double runX = End.X - Start.X;
            double runY = End.Y - Start.Y;
            double along = ((point.X - Start.X) * runX + (point.Y - Start.Y) * runY) / GetLength();

            return lengthTolerance.IsGreaterThanOrEqual(along, 0)
                && lengthTolerance.IsLessThanOrEqual(along, GetLength());
        }

        /// <summary>
        /// Returns true if the two segments share any point at all, whether they cross
        /// each other or merely meet.
        /// </summary>
        public bool Touches(LineSegment2d other)
        {
            LengthTolerance lengthTolerance = new();

            double otherStartSide = GetDistanceFromLine(other.Start);
            double otherEndSide = GetDistanceFromLine(other.End);
            double startSide = other.GetDistanceFromLine(Start);
            double endSide = other.GetDistanceFromLine(End);

            // Each segment has an end on either side of the other's line, so they cross.
            if (lengthTolerance.AreOppositeSigns(otherStartSide, otherEndSide)
                && lengthTolerance.AreOppositeSigns(startSide, endSide))
            {
                return true;
            }

            // Otherwise they can still meet, with an end of one sitting on the other.
            return Contains(other.Start)
                || Contains(other.End)
                || other.Contains(Start)
                || other.Contains(End);
        }
    }
}
