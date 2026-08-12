using Structural_Automation.BuildingModel.Params;
using Structural_Automation.Utils.Geometry;
using Structural_Automation.Utils.SystemParams;

namespace Structural_Automation.BuildingModel
{
    /// <summary>
    /// A vertical rectangular wall, described by the rectangle running through the
    /// middle of the box. That rectangle stands vertically, with one edge parallel to
    /// Z giving the height and the other parallel to the XY plane giving the length,
    /// so the wall length may sit at any angle in XY plane. Thickness spreads half to
    /// either side.
    /// <para>
    /// A wall may hold any number of openings, each a void cut through it. The wall is
    /// the one that knows both itself and its openings, so it is the wall that decides
    /// whether an opening fits and that measures an opening against itself. Its borders
    /// are the strips of wall that no opening may cross — three around the edges and one
    /// between one opening and the next — and they start at the defaults for the unit the
    /// model is drawn in, see <see cref="WallBorders"/>.
    /// </para>
    /// </summary>
    public class Wall : VerticalBox, IFlattenable
    {
        public Guid Id { get; private set; }

        /// <summary>
        /// How much wall is kept above every opening.
        /// </summary>
        public double TopBorder { get; private set; }

        /// <summary>
        /// How much wall is kept below every opening.
        /// </summary>
        public double BotBorder { get; private set; }

        /// <summary>
        /// How much wall is kept beside every opening, at either end.
        /// </summary>
        public double SideBorder { get; private set; }

        /// <summary>
        /// How much wall is kept between one opening and the next.
        /// </summary>
        public double MidBorder { get; private set; }

        private readonly List<Opening> _openings = [];
        public IReadOnlyList<Opening> Openings => _openings.AsReadOnly();

        public Wall(Rectangle midSurface, double thickness, Units units)
            : base(midSurface, thickness)
        {
            WallBorders defaults = new(units.Unit);

            Id = Guid.NewGuid();
            TopBorder = defaults.Top;
            BotBorder = defaults.Bottom;
            SideBorder = defaults.Side;
            MidBorder = defaults.Middle;
        }

        /// <summary>
        /// Replaces the borders. The openings already in the wall have to keep to the new
        /// ones, both within the wall and between themselves, so a change that would leave
        /// an opening breaking a border is refused rather than leaving the wall breaking
        /// its own rule.
        /// </summary>
        /// <exception cref="ArgumentException">A border is negative, or an opening already in the wall would cross one.</exception>
        public void SetBorders(double topBorder, double botBorder, double sideBorder, double midBorder)
        {
            LengthTolerance lengthTolerance = new();

            if (lengthTolerance.IsLessThan(topBorder, 0)
                || lengthTolerance.IsLessThan(botBorder, 0)
                || lengthTolerance.IsLessThan(sideBorder, 0)
                || lengthTolerance.IsLessThan(midBorder, 0))
            {
                throw new ArgumentException("A wall border cannot be negative.");
            }

            if (!_openings.All(opening => IsWithinBorders(opening, topBorder, botBorder, sideBorder)))
            {
                throw new ArgumentException("A wall's borders cannot be widened past an opening already in it.");
            }

            for (int i = 0; i < _openings.Count; i++)
            {
                for (int j = i + 1; j < _openings.Count; j++)
                {
                    if (!AreApart(_openings[i], _openings[j], midBorder))
                    {
                        throw new ArgumentException("A wall's middle border cannot be widened past the gap between two openings already in it.");
                    }
                }
            }

            TopBorder = topBorder;
            BotBorder = botBorder;
            SideBorder = sideBorder;
            MidBorder = midBorder;
        }

        /// <summary>
        /// Adds an opening to the wall, after checking that it fits.
        /// </summary>
        /// <exception cref="ArgumentException">The opening does not fit — see <see cref="Contains"/> — or it comes too close to one already there.</exception>
        public void AddOpening(Opening opening)
        {
            if (!MidSurface.IsCoplanarWith(opening.MidSurface))
            {
                throw new ArgumentException("An opening's mid-surface must lie on the wall's mid-surface plane.");
            }

            if (!new LengthTolerance().AreEqual(Thickness, opening.Thickness))
            {
                throw new ArgumentException("An opening must be exactly as thick as the wall it cuts through.");
            }

            if (!Contains(opening))
            {
                throw new ArgumentException("An opening must stay within the wall's borders, so beneath the top border, above the bottom one and clear of the side ones.");
            }

            if (_openings.Any(existing => Overlap(existing, opening)))
            {
                throw new ArgumentException("An opening must not overlap another opening in the same wall.");
            }

            if (!_openings.All(existing => AreApart(existing, opening, MidBorder)))
            {
                throw new ArgumentException("An opening must keep the wall's middle border between itself and every other opening in the same wall.");
            }

            _openings.Add(opening);
        }

        public bool RemoveOpening(Opening opening)
        {
            return _openings.Remove(opening);
        }

        /// <summary>
        /// Returns the four corners of the mid-surface, the wall flattened. The openings
        /// cut through it are elements in their own right and flatten separately. For the
        /// eight corners of the wall itself, see <see cref="Box.GetCorners"/>.
        /// </summary>
        public IEnumerable<Point3d> GetFlatBuildingPoints()
        {
            return MidSurface.GetCorners();
        }

        /// <summary>
        /// Returns true if the opening is a void this wall could hold: it lies on the
        /// wall's mid-surface plane, it is exactly as thick as the wall so that it cuts
        /// right through, and it stays within the wall's borders. An opening may sit right
        /// on a border, so with a bottom border of zero it may reach the foot of the wall,
        /// as a door does.
        /// </summary>
        public bool Contains(Opening opening)
        {
            if (!MidSurface.IsCoplanarWith(opening.MidSurface))
            {
                return false;
            }

            if (!new LengthTolerance().AreEqual(Thickness, opening.Thickness))
            {
                return false;
            }

            return IsWithinBorders(opening, TopBorder, BotBorder, SideBorder);
        }

        /// <summary>
        /// Returns how far the opening's underside sits above the wall's underside.
        /// </summary>
        public double GetSillHeight(Opening opening)
        {
            return opening.GetBaseElevation() - GetBaseElevation();
        }

        /// <summary>
        /// Returns how far along the wall the opening starts, measured from the wall end
        /// that <see cref="Rectangle.Corner"/> sits on.
        /// </summary>
        public double GetDistanceAlong(Opening opening)
        {
            return GetExtent(opening).MinAlong;
        }

        /// <summary>
        /// Returns the volume of wall left once its openings are taken out.
        /// <see cref="Box.GetVolume"/> gives the solid box and ignores them. Openings cut
        /// right through and never overlap, so subtracting them counts no space twice.
        /// </summary>
        public double GetNetVolume()
        {
            return GetVolume() - _openings.Sum(opening => opening.GetVolume());
        }

        /// <summary>
        /// Returns true if the opening keeps clear of the given borders. The borders are
        /// passed in rather than read off the wall so that <see cref="SetBorders"/> can try
        /// candidate ones against the openings already there before settling on them.
        /// </summary>
        private bool IsWithinBorders(Opening opening, double topBorder, double botBorder, double sideBorder)
        {
            LengthTolerance lengthTolerance = new();
            var (MinAlong, MaxAlong, MinUp, MaxUp) = GetExtent(opening);

            return lengthTolerance.IsGreaterThanOrEqual(MinAlong, sideBorder)
                && lengthTolerance.IsLessThanOrEqual(MaxAlong, GetLength() - sideBorder)
                && lengthTolerance.IsGreaterThanOrEqual(MinUp, botBorder)
                && lengthTolerance.IsLessThanOrEqual(MaxUp, GetHeight() - topBorder);
        }

        /// <summary>
        /// Returns the point's position on the mid-surface plane, as a distance along the
        /// wall from the end <see cref="Rectangle.Corner"/> sits on, and a height above the
        /// foot of the wall. The wall itself then runs from zero to its length, and from
        /// zero to its height.
        /// <para>
        /// Reducing the plane to these two numbers turns questions that would need 3D
        /// maths into ordinary comparisons, and because an opening's mid-surface is
        /// coplanar with the wall's and vertical in the same way, an opening always comes
        /// out as a rectangle square to these two axes.
        /// </para>
        /// <para>
        /// The height is read straight off Z rather than by following the mid-surface's
        /// own vertical edge, because that edge points down whenever the mid-surface was
        /// drawn from a top corner, which would turn the wall upside down and swap the top
        /// border with the bottom one.
        /// </para>
        /// </summary>
        private (double Along, double Up) ProjectOntoMidSurface(Point3d point)
        {
            Vector3d offset = new(MidSurface.Corner, point);

            return (offset.Dot(GetDirection()), point.Z - GetBaseElevation());
        }

        /// <summary>
        /// Returns how far the opening reaches along the wall and up it, as the range it
        /// covers on each of the two mid-surface axes.
        /// </summary>
        private (double MinAlong, double MaxAlong, double MinUp, double MaxUp) GetExtent(Opening opening)
        {
            List<(double Along, double Up)> corners = [.. opening.GetFlatBuildingPoints().Select(ProjectOntoMidSurface)];

            return (
                corners.Min(corner => corner.Along),
                corners.Max(corner => corner.Along),
                corners.Min(corner => corner.Up),
                corners.Max(corner => corner.Up));
        }

        /// <summary>
        /// Returns the clear wall between two openings on each of the mid-surface axes: how
        /// far apart they are along the wall, and how far apart up it. A gap is negative
        /// where the two openings cover common ground on that axis, by as much as they
        /// share.
        /// </summary>
        private (double Along, double Up) GetGaps(Opening first, Opening second)
        {
            var firstExtent = GetExtent(first);
            var secondExtent = GetExtent(second);

            return (
                Math.Max(firstExtent.MinAlong, secondExtent.MinAlong) - Math.Min(firstExtent.MaxAlong, secondExtent.MaxAlong),
                Math.Max(firstExtent.MinUp, secondExtent.MinUp) - Math.Min(firstExtent.MaxUp, secondExtent.MaxUp));
        }

        /// <summary>
        /// Returns true if the two openings share space on the mid-surface, which is when
        /// they cover common ground on both axes at once. Openings that only meet along an
        /// edge share no space.
        /// </summary>
        private bool Overlap(Opening first, Opening second)
        {
            LengthTolerance lengthTolerance = new();
            (double along, double up) = GetGaps(first, second);

            return lengthTolerance.IsLessThan(along, 0) && lengthTolerance.IsLessThan(up, 0);
        }

        /// <summary>
        /// Returns true if the wall between the two openings is at least the given width.
        /// One axis is enough: openings a border apart along the wall have a pier of that
        /// width standing between them whatever their heights, and openings a border apart
        /// up the wall have a course of it whatever their positions.
        /// <para>
        /// Measuring each axis on its own is stricter than measuring corner to corner would
        /// be, and deliberately so. Two openings set diagonally apart are further apart in
        /// a straight line than the wall between them is wide, and it is the width of that
        /// wall that has to carry the load.
        /// </para>
        /// </summary>
        private bool AreApart(Opening first, Opening second, double midBorder)
        {
            LengthTolerance lengthTolerance = new();
            (double along, double up) = GetGaps(first, second);

            return lengthTolerance.IsGreaterThanOrEqual(along, midBorder)
                || lengthTolerance.IsGreaterThanOrEqual(up, midBorder);
        }
    }
}
