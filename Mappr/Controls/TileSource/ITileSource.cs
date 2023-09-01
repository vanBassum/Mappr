using System.Numerics;

namespace Mappr.Controls
{
    public interface ITileSource
    {
        Vector2 TileSize { get; }
        Bitmap? GetTile(int zoom, int x, int y);
    }

}

