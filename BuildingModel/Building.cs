using Structural_Automation.Utils;

namespace Structural_Automation.BuildingModel
{
    public class Building(string name, UnitSystem unitSystem)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = name;
        public UnitSystem UnitSystem { get; private set; } = unitSystem;

        private readonly List<Level> _levels = [];
        private readonly List<Wall> _walls = [];

        public IReadOnlyList<Level> Levels => _levels.AsReadOnly();
        public IReadOnlyList<Wall> Walls => _walls.AsReadOnly();

        public void AddLevel(Level level)
        {
            _levels.Add(level);
        }

        public bool RemoveLevel(Level level)
        {
            return _levels.Remove(level);
        }

        public void AddWall(Wall wall)
        {
            _walls.Add(wall);
        }

        public bool RemoveWall(Wall wall)
        {
            return _walls.Remove(wall);
        }
    }
}
