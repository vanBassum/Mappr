namespace Mappr.Tiles
{
    public class Tile
    {
        public Bitmap Bitmap { get; set; }
        public float Scale { get; set; }

        public Tile(Bitmap bitmap, float scale)
        {
            Bitmap = bitmap;
            Scale = scale;
        }
    }
}



