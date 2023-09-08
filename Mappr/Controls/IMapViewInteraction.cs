namespace Mappr.Controls
{
    public interface IMapViewInteraction
    {
        void HandleMouseWheel(object sender, MapMouseEventArgs e);
        void HandleMouseDown(object sender, MapMouseEventArgs e);
        void HandleMouseUp(object sender, MapMouseEventArgs e);
        void HandleMouseMove(object sender, MapMouseEventArgs e);
        void HandleMouseClick(object sender, MapMouseEventArgs e);
    }
}
