using System.Numerics;
using Mappr.Controls;

namespace Mappr.Entities
{

    public abstract class MapEntity : IMapViewInteraction, IDrawable
    {
        virtual public void Draw(Graphics g, CoordinateScaler2D scaler, Vector2 screenSize) { }

        virtual public void HandleMouseClick(object sender, MapMouseEventArgs e) { }
        virtual public void HandleMouseDown(object sender, MapMouseEventArgs e) { }
        virtual public void HandleMouseMove(object sender, MapMouseEventArgs e) { }
        virtual public void HandleMouseUp(object sender, MapMouseEventArgs e) { }
        virtual public void HandleMouseWheel(object sender, MapMouseEventArgs e) { }
    }

}
