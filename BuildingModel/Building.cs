using Structural_Automation.Utils.SystemParams;

namespace Structural_Automation.BuildingModel
{
    public class Building(string name, Units units)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = name;
        public Units Units { get; private set; } = units;

        private readonly List<Level> _levels = [];
        private readonly List<Wall> _walls = [];
        private readonly List<Floor> _floors = [];

        public IReadOnlyList<Level> Levels => _levels.AsReadOnly();
        public IReadOnlyList<Wall> Walls => _walls.AsReadOnly();
        public IReadOnlyList<Floor> Floors => _floors.AsReadOnly();

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

        public void AddFloor(Floor floor)
        {
            _floors.Add(floor);
        }

        public bool RemoveFloor(Floor floor)
        {
            return _floors.Remove(floor);
        }
    }
}
