using EngineLib.Core;
using EngineLib.Extentions;
using System.Numerics;

namespace EngineLib.Rendering
{
    public class MeshCollider : ICollider
    {
        public List<Mesh> Meshes { get; } = new List<Mesh>();

        public bool Collides(Vector2 worldPosition, Transform transform)
        {
            foreach (var mesh in Meshes)
            {
                var transformedPoints = mesh.Transform(transform).Vertices.ToPoints().ToArray();
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
