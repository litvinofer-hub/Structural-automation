using Structural_Automation.Utils;
using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    public class Building(string name, UnitSystem unitSystem)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = name;
        public UnitSystem UnitSystem { get; private set; } = unitSystem;

        private readonly Dictionary<Point3d, Point3d> _uniquePoints = [];
        private readonly List<Level> _levels = [];

        public IReadOnlyList<Level> Levels => _levels.AsReadOnly();


        /// <summary>
        /// Returns the existing Point with matching coordinates (within tolerance),
        /// or registers and returns the new point.
        /// </summary>
        public Point3d GetOrAddPoint(double x, double y, double z)
        {
            var candidate = new Point3d(x, y, z);

            if (_uniquePoints.TryGetValue(candidate, out var existing))
                return existing;

            _uniquePoints.Add(candidate, candidate);
            return candidate;
        }
    }
}
