using System.Drawing.Imaging;

namespace Mappr
{
    class ImageTiler
    {
        private const int TileSize = 256; // Size of each tile (256x256 pixels)

        public void ProcessImage(string largeImagePath, string outputDirectory)
        {
            // Load the large image from a file
            using (Bitmap largeImage = new Bitmap(largeImagePath))
            {
                // Calculate the highest zoom level based on the size of the large image
                int highestZoomLevel = CalculateHighestZoomLevel(largeImage.Width, largeImage.Height);

                Bitmap currentImage = largeImage;

                // Iterate from the highest zoom level down to level 0
                for (int zoomLevel = highestZoomLevel; zoomLevel >= 0; zoomLevel--)
                {
                    // Divide the large image into tiles and save them
                    DivideAndSaveTiles(currentImage, outputDirectory, zoomLevel);

                    // If we haven't reached zoom level 0, scale the image down
                    if (zoomLevel > 0)
                    {
                        Bitmap scaledImage = ScaleImage(currentImage, 0.5f);

                        // Dispose of the current image and replace it with the scaled image
                        currentImage.Dispose();
                        currentImage = scaledImage;
                    }
                }
            }
        }

        // Calculate the highest zoom level based on the width and height of the large image
        private int CalculateHighestZoomLevel(int width, int height)
        {
            int maxSize = Math.Max(width, height);
            int zoomLevel = 0;

            while (maxSize > TileSize)
            {
                maxSize /= 2;
                zoomLevel++;
            }

            return zoomLevel;
        }

        // Method to divide a large image into tiles and save them
        private void DivideAndSaveTiles(Bitmap largeImage, string outputDirectory, int zoomLevel)
        {
            int tileCountX = (int)Math.Ceiling((double)largeImage.Width / TileSize);
            int tileCountY = (int)Math.Ceiling((double)largeImage.Height / TileSize);

            for (int y = 0; y < tileCountY; y++)
            {
                for (int x = 0; x < tileCountX; x++)
                {
                    // Calculate the position and size of the current tile
                    int tileX = x * TileSize;
                    int tileY = y * TileSize;
                    int tileWidth = Math.Min(TileSize, largeImage.Width - tileX);
                    int tileHeight = Math.Min(TileSize, largeImage.Height - tileY);

                    // Create a new bitmap for the current tile
                    using (Bitmap tile = largeImage.Clone(new Rectangle(tileX, tileY, tileWidth, tileHeight), largeImage.PixelFormat))
                    {
                        // Save the tile to a file
                        string tilePath = Path.Combine(outputDirectory, $"{zoomLevel}", $"{x}x{y}.png");
                        Directory.CreateDirectory(Path.Combine(outputDirectory, $"{zoomLevel}"));
                        tile.Save(tilePath, ImageFormat.Png);
                    }
                }
            }
        }

        // Method to scale an image by a given factor
        private Bitmap ScaleImage(Bitmap originalImage, float scaleFactor)
        {
            int newWidth = (int)(originalImage.Width * scaleFactor);
            int newHeight = (int)(originalImage.Height * scaleFactor);

            // Create a new bitmap with the scaled dimensions
            Bitmap scaledImage = new Bitmap(newWidth, newHeight);

            // Create graphics object and draw the original image onto the scaled image
            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, new Rectangle(0, 0, newWidth, newHeight));
            }

            return scaledImage;
        }
    }


}
