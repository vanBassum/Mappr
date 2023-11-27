using EngineLib.Core;
using EngineLib.Rendering;
using System.Numerics;

namespace EngineLib.Rendering.Tiles
{
    //public class TileRenderer : IRenderer
    //{
    //    public ITileSource? TileSource { get; set; }
    //
    //    public void Render(V2Graphics graphics, Camera camera, GameObject gameObject)
    //    {
    //        if (TileSource == null)
    //            return;
    //
    //        // Get the camera's position in world coordinates
    //        Vector2 cameraPosition = camera.Transform.Position;
    //
    //        // Get the size of a single tile at the current scale
    //        float cameraScale = camera.Transform.Scale.X;
    //        float gameObjectScale = gameObject.Transform.Scale.X;
    //        TileSizeInfo tileSizeInfo = TileSource.GetTileSize(gameObjectScale / cameraScale);
    //
    //        // Calculate the range of tiles visible in the camera's view
    //        float halfViewportWidth = camera.ViewPort.X / 2;
    //        float halfViewportHeight = camera.ViewPort.Y / 2;
    //
    //        int startX = (int)Math.Floor((cameraPosition.X - halfViewportWidth) / tileSizeInfo.TileSize.X);
    //        int endX = (int)Math.Ceiling((cameraPosition.X + halfViewportWidth) / tileSizeInfo.TileSize.X);
    //        int startY = (int)Math.Floor((cameraPosition.Y - halfViewportHeight) / tileSizeInfo.TileSize.Y);
    //        int endY = (int)Math.Ceiling((cameraPosition.Y + halfViewportHeight) / tileSizeInfo.TileSize.Y);
    //
    //        // Loop through visible tiles and render them
    //        for (int y = startY; y <= endY; y++)
    //        {
    //            for (int x = startX; x <= endX; x++)
    //            {
    //                // Get the tile at the current position
    //                Tile? tile = TileSource.GetTile(x, y, gameObjectScale / cameraScale);
    //
    //                if (tile != null)
    //                {
    //                    // Calculate the world position of the tile, considering the GameObject's transformation
    //                    Vector2 worldPosition = new Vector2(x * tileSizeInfo.TileSize.X, y * tileSizeInfo.TileSize.Y);
    //                    worldPosition *= cameraScale / gameObjectScale;
    //                    worldPosition = gameObject.Transform.TransformPoint(worldPosition);
    //
    //                    // Calculate the screen position to render the tile
    //                    Vector2 screenPosition = camera.ProjectToScreen(worldPosition);
    //
    //                    // Render the tile at the screen position
    //                    RenderTile(graphics, tile, screenPosition, tileSizeInfo.TileSize.X, tileSizeInfo.TileSize.Y);
    //                }
    //            }
    //        }
    //    }
    //
    //
    //
    //    private void RenderTile(V2Graphics graphics, Tile tile, Vector2 screenPosition, float tileWidth, float tileHeight)
    //    {
    //
    //        graphics.DrawImage(tile.Bitmap, screenPosition);
    //        //graphics.DrawRectange(Pens.Black, screenPosition, new Vector2(tileWidth, tileHeight));
    //        // Implement your tile rendering logic here
    //        // You might draw the tile using graphics.DrawImage or other appropriate methods
    //        // Example: graphics.DrawImage(tile.Image, screenPosition.X, screenPosition.Y, tileWidth, tileHeight);
    //    }
    //}
}
