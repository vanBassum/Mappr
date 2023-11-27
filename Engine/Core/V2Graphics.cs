using EngineLib.Extentions;
using System.Numerics;

namespace EngineLib.Core
{
    public class V2Graphics
    {
        private readonly Graphics g;

        public V2Graphics(Graphics g)
        {
            this.g = g;
        }

        public void DrawLines(Pen pen, params Vector2[] points)
        {
            g.DrawLines(pen, points.Select(a => a.ToPoint()).ToArray());
        }

        public void DrawLine(Pen pen, Vector2 start, Vector2 end)
        {
            g.DrawLine(pen, start.ToPoint(), end.ToPoint());
        }

        public void DrawRectangle(Pen pen, Vector2 location, Vector2 size)
        {
            g.DrawRectangle(pen, new Rectangle(location.ToPoint(), size.ToSize()));
        }

        public void DrawImage(Image image, Vector2 location)
        {
            g.DrawImage(image, location.ToPoint());
        }

        public void DrawCircle(Pen pen, Vector2 location, float radius)
        {
            RectangleF rect = new RectangleF(location.X - radius, location.Y - radius, 2 * radius, 2 * radius);
            g.DrawEllipse(pen, rect);
        }
    }



}
