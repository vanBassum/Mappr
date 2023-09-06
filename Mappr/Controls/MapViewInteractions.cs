using Mappr.Extentions;
using System.Numerics;

namespace Mappr.Controls
{
    public class MapViewInteractions
    {
        public event EventHandler<MapMouseEventArgs> MouseWheel;
        public event EventHandler<MapMouseEventArgs> MouseDown;
        public event EventHandler<MapMouseEventArgs> MouseUp;
        public event EventHandler<MapMouseEventArgs> MouseMove;
        public event EventHandler RequestRefresh;
        private bool isDragging = false;
        private Point lastMouseLocation;
        private CoordinateScaler2D mapScreenScaler;
        public float ZoomFactor { get; set; } = 1.25f;
        public float MaxZoom { get; set; } = 64f;
        public float MinZoom { get; set; } = 1f;

        public MapViewInteractions(CoordinateScaler2D scaler, Control control)
        {
            mapScreenScaler = scaler;
            control.MouseWheel += (s, e) => HandleMouseWheel(e);
            control.MouseDown += (s, e) =>  HandleMouseDown(e);
            control.MouseUp += (s, e) =>    HandleMouseUp(e);
            control.MouseMove += (s, e) =>  HandleMouseMove(e);
        }

        MapMouseEventArgs GetArgs(Vector2 screenPos)
        {
            return new MapMouseEventArgs(mapScreenScaler)
            {
                MapPosition = mapScreenScaler.ReverseTransformation(screenPos),
                ScreenPosition = screenPos,
            };
        }

        public void HandleMouseWheel(MouseEventArgs e)
        {
            MapMouseEventArgs args = GetArgs(e.Location.ToVector2());
            MouseWheel?.Invoke(this, args);
            if (args.RequestRedraw) RequestRefresh?.Invoke(this, EventArgs.Empty);
            if (args.BlockMapInteractions) return;

            Vector2 mouseScreenPosition = e.Location.ToVector2();
            Vector2 mouseMapPosition = mapScreenScaler.ReverseTransformation(mouseScreenPosition);
            float zoomFactor = e.Delta > 0 ? ZoomFactor : 1f / ZoomFactor; // Adjust the zoom factor as needed

            // Calculate the new scale and offset
            mapScreenScaler.Scale *= zoomFactor;

            if (mapScreenScaler.Scale.X < MinZoom)
                mapScreenScaler.Scale = new Vector2(MinZoom, MinZoom);
            if (mapScreenScaler.Scale.X > MaxZoom)
                mapScreenScaler.Scale = new Vector2(MaxZoom, MaxZoom);

            // Adjust the offset to pivot around the mouse position
            mapScreenScaler.Offset = mouseScreenPosition - mouseMapPosition * mapScreenScaler.Scale;

            RequestRefresh?.Invoke(this, EventArgs.Empty); // Trigger refresh event
        }

        public void HandleMouseDown(MouseEventArgs e)
        {
            MapMouseEventArgs args = GetArgs(e.Location.ToVector2());
            MouseDown?.Invoke(this, args);
            if (args.RequestRedraw) RequestRefresh?.Invoke(this, EventArgs.Empty);
            if (args.BlockMapInteractions) return;

            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMouseLocation = e.Location;
            }
        }

        public void HandleMouseUp(MouseEventArgs e)
        {
            MapMouseEventArgs args = GetArgs(e.Location.ToVector2());
            MouseUp?.Invoke(this, args);
            if (args.RequestRedraw) RequestRefresh?.Invoke(this, EventArgs.Empty);
            if (args.BlockMapInteractions) return;

            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            MapMouseEventArgs args = GetArgs(e.Location.ToVector2());
            MouseMove?.Invoke(this, args);
            if (args.RequestRedraw) RequestRefresh?.Invoke(this, EventArgs.Empty);
            if (args.BlockMapInteractions) return;

            if (isDragging)
            {
                int deltaX = e.Location.X - lastMouseLocation.X;
                int deltaY = e.Location.Y - lastMouseLocation.Y;
                mapScreenScaler.Offset += new Vector2(deltaX, deltaY);
                lastMouseLocation = e.Location;
                RequestRefresh?.Invoke(this, EventArgs.Empty); // Trigger refresh event
            }
        }
    }

    public class MapMouseEventArgs
    {
        public CoordinateScaler2D Scaler { get; }
        public Vector2 ScreenPosition { get; set; }
        public Vector2 MapPosition { get; set; }
        public bool BlockMapInteractions { get; set; } = false;
        public bool RequestRedraw { get; set; } = false;
        public MapMouseEventArgs(CoordinateScaler2D scaler)
        {
            Scaler = scaler;
        }


    }
}
