﻿using Mappr.Tiles;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace Mappr.Controls
{
    public class TileRenderer
    {
        private readonly ITileSetSource tileSource;
        private readonly CoordinateScaler2D mapToScreen;
        private Vector2 screenSize;

        public TileRenderer(ITileSetSource tileSource, CoordinateScaler2D mapToScreen, Vector2 screenSize)
        {
            this.tileSource = tileSource;
            this.mapToScreen = mapToScreen;
            this.screenSize = screenSize;
        }


        public void RenderTiles(Graphics g)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stopwatch load = new Stopwatch();
            var topleftscreen = Vector2.Zero;
            //screenSize = new Vector2(600, 600);
            float zoomLevel = mapToScreen.Scale.X;
            ITileSet? closestTileSet = tileSource.GetClosestTileSet(zoomLevel);

            if (closestTileSet == null)
                return;

            float scalingFactor = zoomLevel / closestTileSet.Scale;

            // Calculate the bounds of the screen in map coordinates
            Vector2 screenTopLeft = mapToScreen.ReverseTransformation(topleftscreen);
            Vector2 screenBottomRight = mapToScreen.ReverseTransformation(screenSize + topleftscreen);

            // Calculate the size of each tile in map coordinates
            Vector2 tileSizeInMapCoords = tileSource.TileSize * scalingFactor / mapToScreen.Scale;

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
                    Bitmap? tile = closestTileSet.GetTile(x, y);
                    if (tile != null)
                    {
                        // Calculate the screen position of the tile
                        Vector2 tileScreenPosition = mapToScreen.ApplyTransformation(new Vector2(x, y) * tileSizeInMapCoords);

                        // Scale the tile based on the scaling factor
                        int scaledWidth = (int)Math.Ceiling(tileSource.TileSize.X * scalingFactor);
                        int scaledHeight = (int)Math.Ceiling(tileSource.TileSize.Y * scalingFactor);
                        var rect = new Rectangle((int)Math.Ceiling(tileScreenPosition.X), (int)Math.Ceiling(tileScreenPosition.Y), scaledWidth, scaledHeight);
                        load.Restart();
                        if(scalingFactor == 1)
                            g.DrawImage(tile, (int)Math.Ceiling(tileScreenPosition.X), (int)Math.Ceiling(tileScreenPosition.Y));
                        else
                            g.DrawImage(tile, rect);
                        load.Stop();
                        t += load.Elapsed;
                        
                    }
                }
            }


            stopwatch.Stop();
            Font font = new Font("Arial", 12);
            Brush brush = Brushes.Green; // You can choose a different color
            g.DrawString($"Tiles: {stopwatch.ElapsedMilliseconds} {t.TotalMilliseconds}", font, brush, 0, 0);
        }
    }
}
