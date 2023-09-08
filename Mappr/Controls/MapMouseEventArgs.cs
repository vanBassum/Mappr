using Mappr.Extentions;
using System.Numerics;

namespace Mappr.Controls
{
    public class MapMouseEventArgs
    {
        public CoordinateScaler2D Scaler { get; }
        public Vector2 MouseScreenPosition { get; set; }
        public Vector2 MouseMapPosition { get; set; }
        public bool RequestRedraw { get; set; } = false;
        public MouseButtons MouseButton { get; set; }
        public bool IsActive { get; set; }
        public int Delta { get; set; }
        public MapMouseEventArgs(MouseEventArgs e, CoordinateScaler2D scaler)
        {
            Scaler = scaler;
            MouseScreenPosition = e.Location.ToVector2();
            MouseMapPosition = scaler.ReverseTransformation(MouseScreenPosition);
            MouseButton = e.Button;
            Delta = e.Delta;
        }
    }
}
