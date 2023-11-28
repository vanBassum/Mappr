using System.Numerics;

namespace EngineLib.Core
{
    public class Mesh : IComponent
    {
        public GameEntity Entity { get; set; } = GameEntity.Empty;
        public Vector2[] Vertices { get; }

        private Mesh(Vector2[] vertices)
        {
            Vertices = vertices;
        }

        public static Mesh Rectangle(float width, float height)
        {
            Vector2[] vertices = new Vector2[]
            {
            new Vector2(-width / 2, -height / 2),
            new Vector2(width / 2, -height / 2),
            new Vector2(width / 2, height / 2),
            new Vector2(-width / 2, height / 2)
            };

            return new Mesh(vertices);
        }

        public static Mesh Circle(float radius, int segments = 16)
        {
            Vector2[] vertices = new Vector2[segments];

            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * 2 * MathF.PI;
                float x = radius * MathF.Cos(angle);
                float y = radius * MathF.Sin(angle);
                vertices[i] = new Vector2(x, y);
            }

            return new Mesh(vertices);
        }

        public static Mesh Triangle(float size)
        {
            Vector2[] vertices = new Vector2[]
            {
            new Vector2(-size / 2, -size * MathF.Sqrt(3) / 2),
            new Vector2(size / 2, -size * MathF.Sqrt(3) / 2),
            new Vector2(0, size / 2)
            };

            return new Mesh(vertices);
        }
    }

}
