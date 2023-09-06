using System.Numerics;

namespace Mappr.Controls
{
    public interface IDrawable
    {
        Vector2 MapPosition { get; }
        void Draw(Graphics g, Vector2 screenPos);
    }
}
