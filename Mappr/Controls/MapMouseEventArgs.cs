using Mappr.Extentions;
using System.Numerics;

namespace Mappr.Controls
{
    public class MapMouseEventArgs
    {
        public CoordinateScaler2D MapToScreenScaler { get; }
        public Vector2 MouseScreenPosition { get; }
        public Vector2 MouseMapPosition { get; }
        public MouseButtons MouseButton { get;  }
        public int Delta { get; }

        public bool IsActive { get; set; }
        public bool RequestRedraw { get; set; } = false;

        public MapMouseEventArgs(MouseEventArgs e, CoordinateScaler2D scaler)
        {
            MapToScreenScaler = scaler;
            MouseScreenPosition = e.Location.ToVector2();
            MouseMapPosition = scaler.ReverseTransformation(MouseScreenPosition);
            MouseButton = e.Button;
            Delta = e.Delta;
        }
    }
}
