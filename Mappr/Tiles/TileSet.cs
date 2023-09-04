namespace Mappr.Tiles
{

    public class TileSet : ITileSet
    {
        public float Scale { get; set; }
        private readonly Dictionary<(int x, int y), string> files = new();

        public TileSet(float scale)
        {
            Scale = scale;
        }

        public Bitmap? GetTile(int x, int y)
        {
            if (files.TryGetValue((x, y), out var file))
                return new Bitmap(file);
            return null;
        }

        public void Add(int x, int y, string file)
        {
            files[(x, y)] = file;
        }
    }
}



