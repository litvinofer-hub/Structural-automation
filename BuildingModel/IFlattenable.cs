using Structural_Automation.Utils.Geometry;

namespace Structural_Automation.BuildingModel
{
    /// <summary>
    /// A building element that can be flattened: given as the points of the shape running
    /// through the middle of it, one dimension short of the solid. A wall or an opening
    /// flattens to the four corners of its mid-surface, a floor to the vertices of its
    /// mid-polygon.
    /// <para>
    /// The points stay global 3D coordinates. What is flattened is the element, not the
    /// coordinates: the thickness is dropped and what is left is the sheet through the
    /// middle.
    /// </para>
    /// </summary>
    public interface IFlattenable
    {
        IEnumerable<Point3d> GetFlatBuildingPoints();
    }
}
