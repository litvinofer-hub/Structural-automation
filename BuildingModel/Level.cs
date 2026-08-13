using Structural_Automation.Utils;
using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    public class Level(double elevation)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Global Z coordinate of the level.
        /// </summary>
        public double Elevation { get; private set; } = elevation;

        private readonly List<SubLevel> _subLevels = [];
        public IReadOnlyList<SubLevel> SubLevels => _subLevels.AsReadOnly();

        public override bool Equals(object? obj)
        {
            return obj is Level other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
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
