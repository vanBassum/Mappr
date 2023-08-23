using System.Numerics;

namespace Mappr.Extentions
{
    public static class SizeExt
    {
        public static Vector2 ToVector2(this Size size)
        {
            return new Vector2(size.Width, size.Height);
        }
    }
}

