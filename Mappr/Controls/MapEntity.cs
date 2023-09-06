using System.Numerics;

namespace Mappr.Controls
{
    public class MapEntity : IDrawable
    {
        public Vector2 MapPosition { get; set; }

        public void Draw(Graphics g, Vector2 screenPos)
        {
            
        }

        virtual public void Draw(Graphics g, CoordinateScaler2D scaler, Vector2 screenSize)
        {
            var screenPos = scaler.ApplyTransformation(MapPosition);
            bool isObjectOnScreen = screenPos.X >= 0 && screenPos.Y >= 0 && screenPos.X < screenSize.X && screenPos.Y < screenSize.Y;
            if(isObjectOnScreen)
                DrawCross(g, Pens.Red, screenPos);
        }

        void DrawCross(Graphics g, Pen pen, Vector2 screenPos, int crossSize = 10)
        {
            // Calculate the starting and ending points for the cross lines
            Point startPointHorizontal = new Point((int)screenPos.X - crossSize, (int)screenPos.Y);
            Point endPointHorizontal = new Point((int)screenPos.X + crossSize, (int)screenPos.Y);
            Point startPointVertical = new Point((int)screenPos.X, (int)screenPos.Y - crossSize);
            Point endPointVertical = new Point((int)screenPos.X, (int)screenPos.Y + crossSize);

            // Draw the horizontal and vertical lines to create the cross
            g.DrawLine(pen, startPointHorizontal, endPointHorizontal);
            g.DrawLine(pen, startPointVertical, endPointVertical);
        }
    }







}
