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

            // List of supported image file extensions
            string[] supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };

            foreach (var extension in supportedExtensions)
            {
                string filePath = Path.Combine(_basePath, $"{zoom}\\{x}x{y}{extension}");

                // Check if the file exists with the current extension
                if (File.Exists(filePath))
                {
                    try
                    {
                        // Load the image file using the correct extension
                        Bitmap tileBitmap = new Bitmap(filePath);
                        return new Tile(tileBitmap, 1 << zoom);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error loading tile: {ex.Message}");
                        // If there's an error with the current file, continue to the next extension
                    }
                }
            }

            // If no file is found or all attempts to load a file fail, return null
            return null;
        }


        public TileSizeInfo GetTileSize(float scale)
        {
            int zoom = (int)Math.Round(Math.Log(scale, 2));
            return new TileSizeInfo(_tileSize, 1 << zoom);
        }
    }
}



