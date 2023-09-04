using System.Numerics;

namespace Mappr.Controls
{
    public class CoordinateScaler2D
    {
        public Vector2 Scale { get; set; }  = Vector2.One;
        public Vector2 Offset { get; set; } = Vector2.Zero;

        public Vector2 ApplyTransformation(Vector2 coordinates)
        {
            return coordinates * Scale + Offset;
        }

        public Vector2 ReverseTransformation(Vector2 scaledCoordinates)
        {
            return (scaledCoordinates - Offset) / Scale;
        }
    }
}

