﻿using System.Numerics;

namespace EngineLib.Extentions
{
    public static class VectorPointExt
    {
        public static Vector2 ToVector(this Point point) => new Vector2(point.X, point.Y);
        public static Vector2 ToVector(this PointF point) => new Vector2(point.X, point.Y);

        public static Point ToPoint(this Vector2 point) => new Point((int)point.X, (int)point.Y);
        public static PointF ToPointF(this Vector2 point) => new PointF(point.X, point.Y);

        public static Size ToSize(this Vector2 point) => new Size((int)point.X, (int)point.Y);
        public static SizeF ToSizeF(this Vector2 point) => new SizeF(point.X, point.Y);


        public static IEnumerable<Point> ToPoints(this IEnumerable<Vector2> points) => points.Select(p => p.ToPoint());

    }
}
