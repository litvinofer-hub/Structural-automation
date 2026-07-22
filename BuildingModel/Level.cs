using Structural_Automation.Utils;
using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    public class Level(double elevation)
    {
        /// <summary>
        /// Global Z coordinate of the level.
        /// </summary>
        public double Elevation { get; private set; } = elevation;

        private readonly List<SubLevel> _subLevels = [];
        public IReadOnlyList<SubLevel> SubLevels => _subLevels.AsReadOnly();

        public override bool Equals(object? obj)
        {
            if (obj is Level other)
            {
                return new LengthTolerance().AreEqual(Elevation, other.Elevation);
            }

            return false;
        }

        public override int GetHashCode()
        {
            double roundFactor = 1.0 / new LengthTolerance().Tolerance;
            return Math.Round(Elevation * roundFactor).GetHashCode();
        }

        /// <summary>
        /// Returns the existing SubLevel with matching offset, or creates and adds a new one.
        /// </summary>
        public SubLevel GetOrAddSubLevel(double offset)
        {
            var existing = _subLevels.FirstOrDefault(s => new LengthTolerance().AreEqual(s.Offset, offset));
            if (existing != null)
                return existing;

            var newSubLevel = new SubLevel(offset);
            _subLevels.Add(newSubLevel);
            return newSubLevel;
        }

        public bool RemoveSubLevel(SubLevel subLevel)
        {
            return _subLevels.Remove(subLevel);
        }
    }
}
