using Mappr.Tiles;
using System.Numerics;

namespace Mappr.Controls
{
    public class TileRenderer
    {
        private readonly ITileSetSource tileSource;
        private readonly CoordinateScaler2D mapToScreen;
        private readonly Vector2 screenSize;

        public TileRenderer(ITileSetSource tileSource, CoordinateScaler2D mapToScreen, Vector2 screenSize)
        {
            this.tileSource = tileSource;
            this.mapToScreen = mapToScreen;
            this.screenSize = screenSize;
        }

        public void RenderTiles(Graphics g, float zoomLevel)
        {
            // Find the closest ITileSet based on the provided zoom level
            ITileSet? closestTileSet = tileSource.GetClosestTileSet(zoomLevel);

            if (closestTileSet == null)
                return;

            // Calculate the bounds of the screen in map coordinates
            Vector2 screenTopLeft = mapToScreen.ReverseTransformation(Vector2.Zero);
            Vector2 screenBottomRight = mapToScreen.ReverseTransformation(screenSize);

            // Calculate the size of each tile in map coordinates
            Vector2 tileSizeInMapCoords = tileSource.TileSize / mapToScreen.Scale;

            // Calculate the indices of the first and last visible tiles
            int firstTileX = (int)Math.Floor(screenTopLeft.X / tileSizeInMapCoords.X);
            int firstTileY = (int)Math.Floor(screenTopLeft.Y / tileSizeInMapCoords.Y);
            int lastTileX = (int)Math.Ceiling(screenBottomRight.X / tileSizeInMapCoords.X);
            int lastTileY = (int)Math.Ceiling(screenBottomRight.Y / tileSizeInMapCoords.Y);

            // Calculate the scaling factor based on the difference between zoomLevel and closestTileSet.Scale
            float scalingFactor = zoomLevel / closestTileSet.Scale;

            // Loop through each visible tile and render it
            for (int x = firstTileX; x <= lastTileX; x++)
            {
                for (int y = firstTileY; y <= lastTileY; y++)
                {
                    // Get the tile from the closest ITileSet
                    Bitmap? tile = closestTileSet.GetTile(x, y);

                    if (tile != null)
                    {
                        // Calculate the screen position of the tile
                        Vector2 tileScreenPosition = mapToScreen.ApplyTransformation(new Vector2(x * tileSizeInMapCoords.X, y * tileSizeInMapCoords.Y) * scalingFactor);

                        // Scale the tile based on the scaling factor
                        int scaledWidth =  (int)Math.Ceiling(tile.Width * scalingFactor);
                        int scaledHeight = (int)Math.Ceiling(tile.Height * scalingFactor);
                        g.DrawImage(tile, new RectangleF(tileScreenPosition.X, tileScreenPosition.Y, scaledWidth, scaledHeight));
                    }
                }
            }
        }


        public void RenderTilesSnap(Graphics g, float zoomLevel)
        {
            // Find the closest ITileSet based on the provided zoom level
            ITileSet? closestTileSet = tileSource.GetClosestTileSet(zoomLevel);
            
            if (closestTileSet == null)
                return;

            // Calculate the bounds of the screen in map coordinates
            Vector2 screenTopLeft = mapToScreen.ReverseTransformation(Vector2.Zero);
            Vector2 screenBottomRight = mapToScreen.ReverseTransformation(screenSize);

            // Calculate the size of each tile in map coordinates
            Vector2 tileSizeInMapCoords = tileSource.TileSize / mapToScreen.Scale;

            // Calculate the indices of the first and last visible tiles
            int firstTileX = (int)Math.Floor(screenTopLeft.X / tileSizeInMapCoords.X);
            int firstTileY = (int)Math.Floor(screenTopLeft.Y / tileSizeInMapCoords.Y);
            int lastTileX = (int)Math.Ceiling(screenBottomRight.X / tileSizeInMapCoords.X);
            int lastTileY = (int)Math.Ceiling(screenBottomRight.Y / tileSizeInMapCoords.Y);

            // Loop through each visible tile and render it
            for (int x = firstTileX; x <= lastTileX; x++)
            {
                for (int y = firstTileY; y <= lastTileY; y++)
                {
                    // Get the tile from the closest ITileSet
                    Bitmap? tile = closestTileSet.GetTile(x, y);

                    if (tile != null)
                    {
                        // Calculate the screen position of the tile
                        Vector2 tileScreenPosition = mapToScreen.ApplyTransformation(new Vector2(x * tileSizeInMapCoords.X, y * tileSizeInMapCoords.Y));

                        // Draw the tile on the Graphics object
                        g.DrawImage(tile, tileScreenPosition.X, tileScreenPosition.Y);
                    }
                }
            }
        }

    }
}
