using System.Diagnostics;
using System.Numerics;

namespace EngineLib.Rendering.Tiles
{
    public class CachingTileSource : ITileSource
    {
        private ITileSource _tileSource;
        private Dictionary<(int, int, float), Tile> _tileCache;
        private int _maxCacheSize;

        public CachingTileSource(ITileSource tileSource, int maxCacheSize)
        {
            _tileSource = tileSource;
            _tileCache = new Dictionary<(int, int, float), Tile>();
            _maxCacheSize = maxCacheSize;
        }

        public Tile? GetTile(int x, int y, float scale)
        {
            try
            {
                // Check if the tile is already in the cache.
                if (_tileCache.TryGetValue((x, y, scale), out Tile? cachedTile))
                    return cachedTile;

                //Debug.WriteLine($"Tile not found in cache at ({x},{y}) with scale {scale}");

                // Tile not in cache, fetch it from the source.
                Tile? tile = _tileSource.GetTile(x, y, scale);

                if (tile == null)
                    return null;

                EnsureSpace(x, y, scale);

                _tileCache[(x, y, scale)] = tile;
                return tile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error caching tile: {ex.Message}");
                return null;
            }
        }

        //Ensures there is at least space for a single tile.
        private void EnsureSpace(int x, int y, float scale)
        {
            if (_tileCache.Count >= _maxCacheSize)
            {
                if (RemoveTilesWithDifferentScale(scale))
                    return; // Enough space has been freed up by removing different-scale tiles.

                RemoveFurthestTile(x, y); // If not enough space is freed by removing different-scale tiles, resort to proximity-based eviction.
            }
        }

        private bool RemoveTilesWithDifferentScale(float desiredScale)
        {
            // Determine the keys of tiles with scales different from the desired scale.
            var differentScaleKeys = _tileCache.Keys
                .Where(key => key.Item3 != desiredScale)
                .ToList();

            // Remove the tiles with different scales.
            foreach (var keyToRemove in differentScaleKeys)
            {
                _tileCache[keyToRemove].Bitmap.Dispose();
                _tileCache.Remove(keyToRemove);
            }


            return differentScaleKeys.Count > 0; // Return true if any different-scale tiles were removed.
        }

        private void RemoveFurthestTile(int x, int y)
        {
            // Find the tile that is furthest away from the requested (x, y) coordinates.

            (int xToRemove, int yToRemove, float scaleToRemove) tileToRemoveKey = default;
            double maxDistance = 0.0;

            foreach (var kvp in _tileCache)
            {
                double distance = CalculateDistance(x, y, kvp.Key.Item1, kvp.Key.Item2);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    tileToRemoveKey = kvp.Key;
                }
            }

            // Remove the tile furthest from the requested coordinates from the cache.
            _tileCache[tileToRemoveKey].Bitmap.Dispose();
            _tileCache.Remove(tileToRemoveKey);
        }

        private double CalculateDistance(int x1, int y1, int x2, int y2)
        {
            // Calculate the Euclidean distance between two points (x1, y1) and (x2, y2).
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public TileSizeInfo GetTileSize(float scale)
        {
            return _tileSource.GetTileSize(scale);
        }
    }
}



