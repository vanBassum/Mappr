using EngineLib.Core;
using System.Numerics;

namespace EngineLib.Rendering
{
    public class Mesh
    {
        public List<Vector2> Vertices { get; } = new List<Vector2>();

        public Mesh() { }
        public Mesh(List<Vector2> vertices) { Vertices = vertices; }

        public void AddRectangle(Vector2 position, Vector2 size)
        {
            int startIndex = Vertices.Count;

            Vertices.Add(position);
            Vertices.Add(position + size * Vector2.UnitX);
            Vertices.Add(position + size);
            Vertices.Add(position + size * Vector2.UnitY);
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

    }





}
