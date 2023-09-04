using System.Numerics;

namespace Mappr.Tiles
{
    public interface ITileSetSource
    {
        public Vector2 TileSize { get; set; }
        ITileSet? GetClosestTileSet(float zoom);
    }
}



