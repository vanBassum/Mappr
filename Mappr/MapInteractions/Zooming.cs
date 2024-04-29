using System.Numerics;
using Mappr.Controls;

namespace Mappr.MapInteractions
{
    internal class Zooming : IMapViewInteraction
    {
        public float ZoomFactor { get; set;}= 1.25f;
        public float MaxZoom    { get; set;}= 64f;
        public float MinZoom { get; set; } = 1f;

        public Zooming(Action<ZoomingConfigurator> configure)
        {
            var configurator = new ZoomingConfigurator(this);
            configure(configurator);

        }

        public void HandleMouseClick(object sender, MapMouseEventArgs e)
        {

        }

        public void HandleMouseWheel(object sender, MapMouseEventArgs e)
        {
            Vector2 mouseScreenPosition = e.MouseScreenPosition;
            Vector2 mouseMapPosition = e.MouseMapPosition;
            float zoomFactor = e.Delta > 0 ? this.ZoomFactor : 1f / this.ZoomFactor; // Adjust the zoom factor as needed

            // Calculate the new scale and offset
            e.MapToScreenScaler.Scale *= zoomFactor;

            if (e.MapToScreenScaler.Scale.X < MinZoom)
                e.MapToScreenScaler.Scale = new Vector2(MinZoom, MinZoom);
            if (e.MapToScreenScaler.Scale.X > MaxZoom)
                e.MapToScreenScaler.Scale = new Vector2(MaxZoom, MaxZoom);

            // Adjust the offset to pivot around the mouse position
            e.MapToScreenScaler.Offset = mouseScreenPosition - mouseMapPosition * e.MapToScreenScaler.Scale;
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

        public class ZoomingConfigurator
        {
            private readonly Zooming zooming;

            public ZoomingConfigurator(Zooming zooming)
            {
                this.zooming = zooming;
            }

            public ZoomingConfigurator WithZoomFactor(float zoomFactor)
            {
                zooming.ZoomFactor = zoomFactor;
                return this;
            }

            public ZoomingConfigurator WithMaxZoom(float maxZoom)
            {
                zooming.MaxZoom = maxZoom;
                return this;
            }

            public ZoomingConfigurator WithMinZoom(float minZoom)
            {
                zooming.MinZoom = minZoom;
                return this;
            }
        }
    }
}
