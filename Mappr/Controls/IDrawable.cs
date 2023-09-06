using System.Numerics;

namespace Mappr.Controls
{
    public interface IDrawable
    {
        void Draw(Graphics g, CoordinateScaler2D scaler, Vector2 screenSize);
    }
}
