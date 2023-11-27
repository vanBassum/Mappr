using System.Diagnostics;

namespace EngineLib.Rendering.Tiles
{
    public class ScalerTileSource : ITileSource
    {
        private ITileSource _tileSource;

        public ScalerTileSource(ITileSource tileSource)
        {
            _tileSource = tileSource;
        }

        public Tile? GetTile(int x, int y, float scale)
        {
            try
            {
                Tile? tile = _tileSource.GetTile(x, y, scale);

                if (tile == null)
                {
                    //Debug.WriteLine($"Tile not found at ({x},{y}) with scale {scale}");
                    return null;
                }

                if (tile.Scale != scale)
                    tile = ScaleTile(tile, scale);

                return tile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error scaling tile: {ex.Message}");
                return null;
            }
        }

        public TileSizeInfo GetTileSize(float scale)
        {
            var tileSize = _tileSource.GetTileSize(scale);
            float scaleRatio = scale / tileSize.Scale;
            return new TileSizeInfo(tileSize.TileSize * scaleRatio, scale);
        }

        private Tile ScaleTile(Tile input, float desiredScale)
        {
            float scaleRatio = desiredScale / input.Scale;

            if (Math.Abs(scaleRatio - 1.0f) < float.Epsilon)
                return input; // No scaling needed; return the original tile.

            Bitmap original = input.Bitmap;

            int newWidth = (int)Math.Ceiling(original.Width * scaleRatio);
            int newHeight = (int)Math.Ceiling(original.Height * scaleRatio);

            Bitmap resized = new Bitmap(original, newWidth, newHeight);
            original.Dispose();
            return new Tile(resized, desiredScale);


        }
    }
}



