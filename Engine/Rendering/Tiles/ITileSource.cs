using System.Collections.Generic;
using System.Numerics;

namespace EngineLib.Rendering.Tiles
{


    public interface ITileSource
    {
        Tile? GetTile(int x, int y, float scale);
        TileSizeInfo GetTileSize(float scale);
    }
}



