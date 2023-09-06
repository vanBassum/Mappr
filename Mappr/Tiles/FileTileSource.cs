using System.Diagnostics;
using System.Numerics;

namespace Mappr.Tiles
{
    public class FileTileSource : ITileSource
    {
        private string _basePath;
        private Vector2 _tileSize = new Vector2(256, 256);
        public FileTileSource(string basePath)
        {
            _basePath = basePath;
        }

        public Tile? GetTile(int x, int y, float scale)
        {
            int zoom = (int)Math.Round(Math.Log(scale, 2));

            try
            {
                string filePath = Path.Combine(_basePath, $"{zoom}\\{x}x{y}.jpg");
                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"Tile not found: {filePath}");
                    return null;
                }

                Bitmap tileBitmap = new Bitmap(filePath);
                return new Tile(tileBitmap, 1<<zoom);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading tile: {ex.Message}");
                return null;
            }
        }

        public TileSizeInfo GetTileSize(float scale)
        {
            int zoom = (int)Math.Round(Math.Log(scale, 2));
            return new TileSizeInfo(_tileSize, 1 << zoom);
        }
    }
}



