using System.Numerics;

namespace Mappr.Controls
{
    public interface IMapObject
    {
        Vector2 MapPosition { get; set; }
        public void Draw(Graphics g, Vector2 screenPos);
    }

}

