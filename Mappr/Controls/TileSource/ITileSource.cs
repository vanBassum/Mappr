using System.Numerics;

namespace Mappr.Controls
{
    public interface ITileSource
    {
        Vector2 TileSize { get; }
        Bitmap? GetTile(Point point);
    }


}

