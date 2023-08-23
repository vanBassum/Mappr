using Mappr.Extentions;
using System.Diagnostics;
using System.Numerics;

namespace Mappr.Controls
{
    public class FileTileSource : ITileSource
    {
        Dictionary<Point, string> files = new Dictionary<Point, string>();
        public Vector2 TileSize { get; } = new Vector2(256, 256);

        public FileTileSource(string dir)
        {
            foreach(var file in Directory.GetFiles(dir))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var split = name.Split('x');
                if(split.Length == 2)
                {
                    if (int.TryParse(split[0], out int x))
                    {
                        if (int.TryParse(split[1], out int y))
                        {
                            files.TryAdd(new Point(x, y), file);
                        }
                    }
                }
            }
            if (files.Any())
                TileSize = Image.FromFile(files.FirstOrDefault().Value).Size.ToVector2();

        }


        public Bitmap? GetTile(Point point)
        {
            if(files.TryGetValue(point, out string? file))
            {
                if(File.Exists(file))
                    return new Bitmap(file);
            }
            return null;    
        }
    }
}

