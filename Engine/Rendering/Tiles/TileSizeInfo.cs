using System.Numerics;

namespace EngineLib.Rendering.Tiles
{
    public class TileSizeInfo
    {
        public TileSizeInfo(Vector2 tileSize, float scale)
        {
            TileSize = tileSize;
            Scale = scale;
        }

        public Vector2 TileSize { get; set; }
        public float Scale { get; set; }

    }
}



