using System.Numerics;

namespace Mappr.Controls.Interactions
{
    internal class Zooming : IMapViewInteraction
    {
        private float zoomFactor = 1.25f;
        private float maxZoom = 64f;
        private float minZoom = 1f;

        public void HandleMouseClick(object sender, MapMouseEventArgs e)
        {

        }

        public void HandleMouseWheel(object sender, MapMouseEventArgs e)
        {
            Vector2 mouseScreenPosition = e.MouseScreenPosition;
            Vector2 mouseMapPosition = e.MouseMapPosition;
            float zoomFactor = e.Delta > 0 ? this.zoomFactor : 1f / this.zoomFactor; // Adjust the zoom factor as needed

            // Calculate the new scale and offset
            e.Scaler.Scale *= zoomFactor;

            if (e.Scaler.Scale.X < minZoom)
                e.Scaler.Scale = new Vector2(minZoom, minZoom);
            if (e.Scaler.Scale.X > maxZoom)
                e.Scaler.Scale = new Vector2(maxZoom, maxZoom);

            // Adjust the offset to pivot around the mouse position
            e.Scaler.Offset = mouseScreenPosition - mouseMapPosition * e.Scaler.Scale;
            e.RequestRedraw = true;
        }


        public void HandleMouseDown(object sender, MapMouseEventArgs e)
        {
        }

        public void HandleMouseUp(object sender, MapMouseEventArgs e)
        {
        }

        public void HandleMouseMove(object sender, MapMouseEventArgs e)
        {
        }
    }
}
