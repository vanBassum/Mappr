using EngineLib.Core;
using System.Numerics;

namespace EngineLib.Rendering
{
    public class LinesRenderer : IRenderer
    {
        public Vector2[] Points { get; set; } = new Vector2[0];

        public void Render(V2Graphics graphics, Camera camera, GameObject gameObject)
        {
            if (Points.Length < 2)
            {
                // Need at least two points to draw a line
                return;
            }

            Vector2[] worldSpacePoints = new Vector2[Points.Length];

            // Transform each point to world space, considering the local position of the GameObject
            for (int i = 0; i < Points.Length; i++)
            {
                Vector2 localPosition = Points[i];
                Vector2 worldPosition = gameObject.Transform.TransformPoint(localPosition);
                worldSpacePoints[i] = camera.ProjectToScreen(worldPosition);
            }

            // Draw the line using the transformed points
            graphics.DrawLines(worldSpacePoints);
        }
    }
}
