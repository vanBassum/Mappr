namespace Mappr
{
    class MakeImage
    {
        // Method to get the file path of a tile based on its coordinates
        string GetFile(int z, int x, int y) => $"maps/tarkov/labs/{z}/{x}x{y}.png";

        public MakeImage()
        {
            int z = 6; // Zoom level, assuming 6 in this example
            int tileWidth = 256; // Width of each tile in pixels
            int tileHeight = 256; // Height of each tile in pixels
            int tileCountX = 60; // Number of tiles in width
            int tileCountY = 31; // Number of tiles in height (you mentioned 36x36)

            // Calculate the size of the large image
            int largeImageWidth = tileCountX * tileWidth;
            int largeImageHeight = tileCountY * tileHeight;

            // Create a large bitmap to hold the combined image
            using (Bitmap largeImage = new Bitmap(largeImageWidth, largeImageHeight))
            {
                // Create a graphics object from the large bitmap
                using (Graphics g = Graphics.FromImage(largeImage))
                {
                    // Loop through each tile
                    for (int x = 0; x < tileCountX; x++)
                    {
                        for (int y = 0; y < tileCountY; y++)
                        {
                            // Get the file path for the current tile
                            string filePath = GetFile(z, x, y);

                            // Check if the file exists before loading it
                            if (!File.Exists(filePath))
                            {
                                Console.WriteLine($"Tile file not found: {filePath}");
                                continue;
                            }

                            // Load the tile image from the file
                            using (Bitmap tile = new Bitmap(filePath))
                            {
                                // Calculate the position of the tile in the large image
                                int offsetX = x * tileWidth;
                                int offsetY = y * tileHeight;

                                // Draw the tile onto the large image at the calculated position
                                g.DrawImage(tile, offsetX, offsetY, tileWidth, tileHeight);
                            }
                        }
                    }
                }

                // Save the large image to the specified file path
                string saveTo = @"C:\Users\bas\Desktop\map2.png";
                largeImage.Save(saveTo, System.Drawing.Imaging.ImageFormat.Png);
                Console.WriteLine($"Combined image saved to {saveTo}");
            }
        }
    }


}
