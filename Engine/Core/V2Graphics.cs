using System.Numerics;

namespace EngineLib.Core
{
    public class V2Graphics
    {
        private readonly Graphics g;
        public V2Graphics(Graphics g) { 
            this.g = g; 
        }


        public void DrawCircle(Vector2 center, float radius)
        {
            float x = center.X - radius;
            float y = center.Y - radius;
            float diameter = 2 * radius;

            // Create a rectangle that represents the circle
            RectangleF circleRect = new RectangleF(x, y, diameter, diameter);

            // Draw the circle using Graphics object
            g.DrawEllipse(Pens.Black, circleRect);
        }

        public void DrawLines(Pen pen, Vector2[] points)
        {
            g.DrawLines(pen, points.Select(a=> new Point((int)a.X, (int)a.Y)).ToArray());
        }

    }
}
