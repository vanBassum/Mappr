using System.Numerics;

namespace Mappr.Extentions
{
    public static class Vector2Ext
    {
        public static Point ToPoint(this Vector2 vector, Rounding rounding = Rounding.Down)
        {
            switch (rounding)
            {
                case Rounding.Down: return new Point((int)vector.X, (int)vector.Y);
                case Rounding.Up: return new Point((int)Math.Ceiling(vector.X), (int)Math.Ceiling(vector.Y));
                case Rounding.Round: return new Point((int)Math.Round(vector.X), (int)Math.Round(vector.Y));
            }
            throw new NotImplementedException("Rounding method not implemented.");
        }

        public enum Rounding
        {
            Up,
            Down,
            Round
        }

    }

}

