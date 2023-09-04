namespace Mappr.Tiles
{
    public class BufferedTileSet : ITileSet
    {
        private ITileSet sourceTileSet;
        private Dictionary<(int x, int y), Bitmap> tileCache = new Dictionary<(int x, int y), Bitmap>();

        public float Scale => sourceTileSet.Scale;

        public BufferedTileSet(ITileSet sourceTileSet)
        {
            this.sourceTileSet = sourceTileSet;
        }

        public Bitmap? GetTile(int x, int y)
        {
            // Check if the tile is already cached
            if (tileCache.TryGetValue((x, y), out Bitmap cachedTile))
            {
                return cachedTile;
            }

            // Load the tile from the source ITileSet
            Bitmap? tile = sourceTileSet.GetTile(x, y);

            // Cache the loaded tile
            if (tile != null)
            {
                tileCache[(x, y)] = tile;
            }

            return tile;
        }
    }

}



