using EngineLib.Core;
using System.Numerics;

namespace EngineLib.Rendering
{
    public interface IRenderer : IComponent
    {
        public void Render(Graphics graphics, Camera camera, Transform transform);
    }

    public interface ICollider : IComponent
    {
        public bool Collides(Vector2 worldPosition, Transform transform);
    }




    public class Mesh
    {
        public List<Vector2> Vertices { get; } = new List<Vector2>();

        public Mesh() { }
        public Mesh(List<Vector2> vertices) { Vertices = vertices; }

        public void AddRectangle(Vector2 pointA, Vector2 pointB)
        {
            int startIndex = Vertices.Count;

            Vertices.Add(pointA);
            Vertices.Add(new Vector2(pointA.X, pointB.Y));
            Vertices.Add(pointB);
            Vertices.Add(new Vector2(pointB.X, pointA.Y));
        }

        public void AddLine(Vector2 pointA, Vector2 pointB)
        {
            Vertices.Add(pointA);
            Vertices.Add(pointB);
        }

        public Mesh Transform(Transform transform)
        {
            return new Mesh(Vertices.Select(v => transform.TransformPoint(v)).ToList());
        }

        public Point[] ToPoints() => Vertices.Select(v => new Point((int)v.X, (int)v.Y)).ToArray();
    }

    public class MeshRenderer : IRenderer
    {
        public List<Mesh> Meshes { get; } = new List<Mesh>();

        public void Render(Graphics graphics, Camera camera, Transform transform)
        {
            foreach (var mesh in Meshes)
            {
                var transformedPoints = mesh.Transform(transform).Transform(camera.Transform.Inverse()).ToPoints();
                graphics.DrawPolygon(Pens.Black, transformedPoints);
            }
        }
    }
    public class MeshCollider : ICollider
    {
        public List<Mesh> Meshes { get; } = new List<Mesh>();

        public bool Collides(Vector2 worldPosition, Transform transform)
        {
            foreach (var mesh in Meshes)
            {
                var transformedPoints = mesh.Transform(transform).ToPoints();
                if (IsPointInPolygon(worldPosition, transformedPoints))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPointInPolygon(Vector2 point, Point[] polygon)
        {
            // Check if the point is inside the polygon using the winding number algorithm

            int windingNumber = 0;
            int count = polygon.Length;

            for (int i = 0; i < count; i++)
            {
                int nextIndex = (i + 1) % count;

                if (polygon[i].Y <= point.Y)
                {
                    if (polygon[nextIndex].Y > point.Y && IsLeft(polygon[i], polygon[nextIndex], point) > 0)
                    {
                        windingNumber++;
                    }
                }
                else
                {
                    if (polygon[nextIndex].Y <= point.Y && IsLeft(polygon[i], polygon[nextIndex], point) < 0)
                    {
                        windingNumber--;
                    }
                }
            }

            return windingNumber != 0;
        }

        private static float IsLeft(Point P0, Point P1, Vector2 P2)
        {
            return ((P1.X - P0.X) * (P2.Y - P0.Y) - (P2.X - P0.X) * (P1.Y - P0.Y));
        }
    }





}
