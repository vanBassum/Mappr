using Mappr.Extentions;
using System.Diagnostics;
using System.Numerics;

namespace Mappr.Controls
{
    public class FileTileSource : ITileSource
    {
        Dictionary<(int zoom, int x, int y), string> files = new Dictionary<(int zoom, int x, int y), string>();
        public Vector2 TileSize { get; } = new Vector2(256, 256);

        public FileTileSource(string dir)
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
                        files.TryAdd((zoom, x, y), file);
                    }
                }
            }
            if (files.Any())
                TileSize = Image.FromFile(files.FirstOrDefault().Value).Size.ToVector2();
        }


        public Bitmap? GetTile(int zoom, int x, int y)
        {
            if(files.TryGetValue((zoom, x, y), out string? file))
            {
                if(File.Exists(file))
                    return new Bitmap(file);
            }
            return null;    
        }
    }
}

