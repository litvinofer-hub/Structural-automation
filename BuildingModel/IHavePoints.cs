using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    public interface IHavePoints
    {
        IEnumerable<Point3d> GetPoints();
    }
}
