using Mappr.Extentions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Mappr.Tiles
{

    public class FileTileSetSource : ITileSetSource
    {
        public Vector2 TileSize { get; set; } = new Vector2(256, 256);
        private readonly SortedDictionary<int, TileSet> tileSets = new();
        private readonly SortedDictionary<int, BufferedTileSet> btileSets = new();

        public FileTileSetSource(string dir)
        {
            foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(dir, file);
                var fileName = Path.GetFileNameWithoutExtension(relativePath);
                var dirName = Path.GetDirectoryName(relativePath);
                var split = fileName?.Split('x');
                if (split?.Length == 2)
                {
                    if (int.TryParse(dirName, out int zoom) &&
                        int.TryParse(split[0], out int x) &&
                        int.TryParse(split[1], out int y))
                    {
                        Insert(x, y, 1 << zoom, file);
                    }
                }
            }
            //if (files.Any())
            //   TileSize = Image.FromFile(files.FirstOrDefault().Value).Size.ToVector2();
        }

        public ITileSet? GetClosestTileSet(float zoom)
        {
            foreach (var item in btileSets)
            {
                if (item.Key >= zoom)
                    return item.Value;
            }
            return null;
        }

        void Insert(int x, int y, int zoom, string file)
        {
            if (!tileSets.ContainsKey(zoom))
            { 
                tileSets[zoom] = new TileSet(zoom);
                btileSets[zoom] = new BufferedTileSet(tileSets[zoom]);
            }
            tileSets[zoom].Add(x, y, file);
        }
    }
}



