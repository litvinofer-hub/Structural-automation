namespace Structural_Automation.BuildingModel
{
    public class SubLevel(double offset)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Offset relative to the parent Level's Elevation.
        /// </summary>
        public double Offset { get; private set; } = offset;

        public override bool Equals(object? obj)
        {
            return obj is SubLevel other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
