using Mappr.Extentions;
using System.Numerics;

namespace Mappr.Controls
{
    public class MapViewInteractions
    {
        public event EventHandler RequestRefresh;
        private bool isDragging = false;
        private Point lastMouseLocation;
        private CoordinateScaler2D mapScreenScaler;

        public MapViewInteractions(CoordinateScaler2D scaler)
        {
            mapScreenScaler = scaler;
        }

        public void HandleMouseWheel(MouseEventArgs e)
        {
            // Get the mouse position in map coordinates
            Vector2 mouseScreenPosition = e.Location.ToVector2();
            Vector2 mouseMapPosition = mapScreenScaler.ReverseTransformation(mouseScreenPosition);

            float zoomFactor = e.Delta > 0 ? 1.1f : 0.9f; // Adjust the zoom factor as needed

            // Calculate the new scale and offset
            mapScreenScaler.Scale *= zoomFactor;

            // Adjust the offset to pivot around the mouse position
            mapScreenScaler.Offset = mouseScreenPosition - mouseMapPosition * mapScreenScaler.Scale;

            RequestRefresh?.Invoke(this, EventArgs.Empty); // Trigger refresh event
        }

        public void HandleMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMouseLocation = e.Location;
            }
        }

        public void HandleMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
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
}
