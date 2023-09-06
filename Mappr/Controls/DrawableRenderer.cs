using System.Numerics;

namespace Mappr.Controls
{
    public class DrawableRenderer
    {
        private readonly CoordinateScaler2D mapToScreen;

        public DrawableRenderer(CoordinateScaler2D mapToScreen)
        {
            this.mapToScreen = mapToScreen;
        }

        public void Render(Graphics g, IEnumerable<IDrawable> drawables, Vector2 screenSize)
        {
            foreach (var drawable in drawables)
            {
                var screenPos = mapToScreen.ApplyTransformation(drawable.MapPosition);
                drawable.Draw(g, screenPos);
            }
        }
    }
}
