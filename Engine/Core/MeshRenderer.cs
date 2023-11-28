using System.Numerics;

namespace EngineLib.Core
{
    public class MeshRenderer : IRenderer
    {
        public GameEntity Entity { get; set; } = GameEntity.Empty;


        public void Render(Graphics graphics, Matrix3x2 transform, Viewport viewport)
        {
            var meshes = Entity.GetComponents<Mesh>();
            foreach (var mesh in meshes)
            {
                Render(graphics, transform, mesh);
            }
        }


        public void Render(Graphics graphics, Matrix3x2 transform, Mesh mesh)
        {
            // Project each point in the mesh
            Vector2[] projectedMesh = new Vector2[mesh.Vertices.Length];
            for (int i = 0; i < mesh.Vertices.Length; i++)
            {
                // Apply the transformation matrix to each point
                projectedMesh[i] = Vector2.Transform(mesh.Vertices[i], transform);
            }

            // Draw the projected mesh
            if (projectedMesh != null && projectedMesh.Length >= 2)
            {
                graphics.DrawPolygon(Pens.Black, projectedMesh.Select(p => new PointF(p.X, p.Y)).ToArray());
            }
        }

    }

}
