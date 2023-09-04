namespace Mappr.Tiles
{
    public interface ITileSet
    {
        public float Scale { get;  }
        public Bitmap? GetTile(int x, int y);
    }

}



