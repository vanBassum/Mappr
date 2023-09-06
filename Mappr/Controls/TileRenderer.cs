using Mappr.Tiles;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace Mappr.Controls
{
    public class TileRenderer
    {
        private readonly CoordinateScaler2D mapToScreen;

        public TileRenderer(CoordinateScaler2D mapToScreen)
        {
            this.mapToScreen = mapToScreen;
        }


        public void RenderTiles(Graphics g, ITileSource TileSource, Vector2 screenSize)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stopwatch load = new Stopwatch();
            var topleftscreen = Vector2.Zero;
            //screenSize = new Vector2(600, 600);
            float zoomLevel = mapToScreen.Scale.X;

            // Calculate the bounds of the screen in map coordinates
            Vector2 screenTopLeft = mapToScreen.ReverseTransformation(topleftscreen);
            Vector2 screenBottomRight = mapToScreen.ReverseTransformation(screenSize + topleftscreen);

            // Calculate the size of each tile in map coordinates
            var tileInfo = TileSource.GetTileSize(zoomLevel);
            Vector2 tileSizeInScreenCoords = tileInfo.TileSize;
            Vector2 tileSizeInMapCoords = tileSizeInScreenCoords / mapToScreen.Scale;

            // Calculate the indices of the first and last visible tiles
            int firstTileX = (int)Math.Floor(screenTopLeft.X / tileSizeInMapCoords.X);
            int firstTileY = (int)Math.Floor(screenTopLeft.Y / tileSizeInMapCoords.Y);
            int lastTileX = (int)Math.Ceiling(screenBottomRight.X / tileSizeInMapCoords.X);
            int lastTileY = (int)Math.Ceiling(screenBottomRight.Y / tileSizeInMapCoords.Y);

            TimeSpan t = new TimeSpan();

            // Loop through each visible tile and render it
            for (int x = firstTileX; x <= lastTileX - 1; x++)
            {
                for (int y = firstTileY; y <= lastTileY - 1; y++)
                {
                    // Get the tile from the closest ITileSet
                    Tile? tile = TileSource.GetTile(x, y, zoomLevel);
                    if (tile != null)
                    {
                        // Calculate the screen position of the tile
                        Vector2 tileScreenPosition = mapToScreen.ApplyTransformation(new Vector2(x, y) * tileSizeInMapCoords);

                        // Scale the tile based on the scaling factor
                        load.Restart();
                        g.DrawImage(tile.Bitmap, (int)Math.Ceiling(tileScreenPosition.X), (int)Math.Ceiling(tileScreenPosition.Y));
                        load.Stop();
                        t += load.Elapsed;
                        
                    }
                }
            }


            stopwatch.Stop();
            Font font = new Font("Arial", 12);
            Brush brush = Brushes.Yellow; // You can choose a different color
            g.DrawString($"Tiles: {stopwatch.ElapsedMilliseconds} {t.TotalMilliseconds}", font, brush, 0, 0);
        }
    }
}
