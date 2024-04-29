using Mappr.Controls;
using Mappr.Entities;
using Mappr.Extentions;
using System.Numerics;

namespace Mappr
{
    public class PlayerEntity : MapEntity
    {
        public Vector2 MapPosition { get; set; }
        public float Rotation { get; set; } // Angle in radians

        public PlayerEntity(Vector2 initialPosition)
        {
            MapPosition = initialPosition;
            Rotation = 0; // Initial rotation (e.g., facing north)
        }

        public override void Draw(Graphics g, CoordinateScaler2D scaler, Vector2 screenSize)
        {
            // Calculate the screen position of the player
            Vector2 screenPosition = scaler.ApplyTransformation(MapPosition);

            // Calculate the endpoints of the arrow
            Vector2 arrowStart = screenPosition;
            Vector2 arrowEnd = screenPosition + new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation)) * 20; // Adjust arrow length as needed

            // Draw the player icon (e.g., a circle) at the player's position
            //if(MouseHover)
            //    g.FillEllipse(Brushes.Blue, screenPosition.X - 5, screenPosition.Y - 5, 10, 10);
            //else
                g.FillEllipse(Brushes.Red, screenPosition.X - 5, screenPosition.Y - 5, 10, 10);

            // Draw the arrow
            g.DrawLine(Pens.Blue, arrowStart.ToPoint(), arrowEnd.ToPoint());
        }
    }
}
