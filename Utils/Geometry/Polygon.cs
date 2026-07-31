
namespace Structural_Automation.Utils.Geometry
{
    /// <summary>
    /// A closed, planar polygon. The vertices are given, and kept, in order around the
    /// perimeter: each one joins the next, and the closing edge from the last back to
    /// the first is implied, so the first vertex is not repeated at the end. All
    /// vertices lies on one plane, and the outline does not cross itself.
    /// </summary>
    public class Polygon
    {
        private readonly List<Point3d> _vertices;

        public IReadOnlyList<Point3d> Vertices => _vertices.AsReadOnly();

        /// <param name="vertices">
        /// In order around the perimeter. The order is not corrected, and it decides
        /// which shape you get: a wrong order is usually caught as self-intersecting,
        /// but not always. The six vertices of an L shape, for instance, form five
        /// different valid polygons depending on their order, two of which enclose a
        /// different area, so a mistake here can pass validation and still give the
        /// wrong answer.
        /// </param>
        public Polygon(IEnumerable<Point3d> vertices)
        {
            _vertices = [.. vertices];

            if (_vertices.Count < 3)
            {
                throw new ArgumentException("A polygon needs at least three vertices.");
            }

            LengthTolerance lengthTolerance = new();

            for (int i = 0; i < _vertices.Count; i++)
            {
                if (lengthTolerance.AreEqual(_vertices[i].Distance(GetNextVertex(i)), 0))
                {
                    throw new ArgumentException("A polygon cannot have a zero-length edge.");
                }
            }

            // A zero sum means the vertices give no plane to lie on, which happens when
            // they are all on one line.
            if (GetNewellSum().IsZero())
            {
                throw new ArgumentException("A polygon's vertices must not all lie on a single line.");
            }

            Vector3d normal = GetNormal();
            foreach (Point3d vertex in _vertices)
            {
                if (!lengthTolerance.AreEqual(new Vector3d(_vertices[0], vertex).Dot(normal), 0))
                {
                    throw new ArgumentException("A polygon's vertices must all lie on the same plane.");
                }
            }

            if (HasSelfIntersection())
            {
                throw new ArgumentException("A polygon's edges must not cross each other.");
            }
        }

        private Point3d GetNextVertex(int index)
        {
            return _vertices[(index + 1) % _vertices.Count];
        }

        /// <summary>
        /// Newell's method, halved so that the result's length is the area and its
        /// direction is normal to the plane. Every vertex contributes, which keeps it
        /// stable where picking three vertices would fail if those three were nearly
        /// on one line.
        /// </summary>
        private Vector3d GetNewellSum()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            for (int i = 0; i < _vertices.Count; i++)
            {
                Point3d current = _vertices[i];
                Point3d next = GetNextVertex(i);

                x += (current.Y - next.Y) * (current.Z + next.Z);
                y += (current.Z - next.Z) * (current.X + next.X);
                z += (current.X - next.X) * (current.Y + next.Y);
            }

            return new Vector3d(x / 2, y / 2, z / 2);
        }

        /// <summary>
        /// Returns the unit vector normal to the polygon's plane.
        /// </summary>
        public Vector3d GetNormal()
        {
            return GetNewellSum().Normalized();
        }

        public double GetArea()
        {
            return GetNewellSum().GetLength();
        }

        public double GetPerimeter()
        {
            double perimeter = 0;

            for (int i = 0; i < _vertices.Count; i++)
            {
                perimeter += _vertices[i].Distance(GetNextVertex(i));
            }

            return perimeter;
        }

        /// <summary>
        /// Returns the edges in order, each running from one vertex to the next and the
        /// last closing back to the first.
        /// </summary>
        public IReadOnlyList<LineSegment3d> GetEdges()
        {
            List<LineSegment3d> edges = [];

            for (int i = 0; i < _vertices.Count; i++)
            {
                edges.Add(new LineSegment3d(_vertices[i], GetNextVertex(i)));
            }

            return edges;
        }

        /// <summary>
        /// Returns true if both polygons lie on parallel planes.
        /// </summary>
        public bool IsParallelTo(Polygon other)
        {
            return GetNormal().IsParallelTo(other.GetNormal());
        }

        /// <summary>
        /// Returns true if both polygons lie on the same plane.
        /// </summary>
        public bool IsCoplanarWith(Polygon other)
        {
            if (!IsParallelTo(other))
            {
                return false;
            }

            return new LengthTolerance().AreEqual(
                new Vector3d(_vertices[0], other._vertices[0]).Dot(GetNormal()), 0);
        }

        /// <summary>
        /// First in-plane axis, taken along the first edge.
        /// </summary>
        private Vector3d GetPlaneXAxis()
        {
            return new Vector3d(_vertices[0], _vertices[1]).Normalized();
        }

        /// <summary>
        /// Second in-plane axis, perpendicular to the first.
        /// </summary>
        private Vector3d GetPlaneYAxis()
        {
            return GetNormal().Cross(GetPlaneXAxis());
        }

        /// <summary>
        /// Returns the vertices as coordinates within the polygon's own plane. Because
        /// every vertex is known to be on that plane, nothing is lost, and tests that
        /// would need 3D intersection maths become ordinary 2D ones.
        /// </summary>
        private List<(double X, double Y)> ProjectOntoPlane()
        {
            Vector3d planeX = GetPlaneXAxis();
            Vector3d planeY = GetPlaneYAxis();

            List<(double X, double Y)> projected = [];

            foreach (Point3d vertex in _vertices)
            {
                Vector3d offset = new(_vertices[0], vertex);
                projected.Add((offset.Dot(planeX), offset.Dot(planeY)));
            }

            return projected;
        }

        /// <summary>
        /// Returns true if the outline crosses itself, which is what separates a polygon
        /// enclosing a single area from one folded over like a bowtie. Every pair of
        /// edges is tested for sharing a point, except pairs that neighbour each other:
        /// those always meet, at the vertex between them, and that meeting is what
        /// closes the outline rather than crossing it.
        /// <para>
        /// The test runs on the vertices projected onto the polygon's own plane. Since
        /// every vertex is already known to lie on that plane, nothing is lost, and what
        /// would otherwise need 3D intersection maths becomes an ordinary 2D question.
        /// </para>
        /// <para>
        /// Comparing every pair costs time proportional to the square of the vertex
        /// count, which is nothing for the handful of vertices a structural outline has.
        /// </para>
        /// </summary>
        private bool HasSelfIntersection()
        {
            List<(double X, double Y)> flat = ProjectOntoPlane();
            int count = flat.Count;

            for (int i = 0; i < count; i++)
            {
                LineSegment2d first = new(flat[i], flat[(i + 1) % count]);

                for (int j = i + 1; j < count; j++)
                {
                    bool areNeighbours = j == i + 1 || (i == 0 && j == count - 1);

                    if (areNeighbours)
                    {
                        continue;
                    }

                    LineSegment2d second = new(flat[j], flat[(j + 1) % count]);

                    if (first.Touches(second))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Two polygons are equal when they have the same edges. Comparing edges rather
        /// than vertices matters: the same four vertices describe both a square and a
        /// bowtie, but those have different edges. Edge equality already ignores
        /// direction, so the result does not depend on where the outline starts or which
        /// way round it runs.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Polygon other)
            {
                IReadOnlyList<LineSegment3d> edges = GetEdges();
                IReadOnlyList<LineSegment3d> otherEdges = other.GetEdges();

                if (edges.Count != otherEdges.Count)
                {
                    return false;
                }

                return edges.All(otherEdges.Contains);
            }

            return false;
        }

        public override int GetHashCode()
        {
            // Sort so that where the outline starts cannot affect the result, while
            // still combining the edges with a proper mixing step.
            int[] edgeHashes = [.. GetEdges().Select(edge => edge.GetHashCode())];
            Array.Sort(edgeHashes);

            unchecked
            {
                int hash = 17;
                foreach (int edgeHash in edgeHashes)
                {
                    hash = hash * 31 + edgeHash;
                }

                return hash;
            }
        }
    }
}
