using System;
using System.Diagnostics;
using System.Numerics;

namespace Mappr.Controls
{
    public class BufferedTileSource : ITileSource
    {
        Dictionary<Point, Bitmap> buffer = new ();
        int maxBufferSize;

        ITileSource tileSource;
        public Vector2 TileSize { get; } = new Vector2(256, 256);

        public BufferedTileSource(ITileSource source, int maxBufferSize)
        {
            this.tileSource = source;
            this.maxBufferSize = maxBufferSize;
            this.TileSize = source.TileSize;
        }

        public Bitmap? GetTile(Point point)
        {
            //If its in buffer, return it.
            if(buffer.TryGetValue(point, out Bitmap? img))
                return img; 

            //Load the tile
            Bitmap? bmp = tileSource.GetTile(point);
            if (bmp != null)
            {
                if(buffer.Count >= maxBufferSize)
                    RemoveFurthestTile(point);

                Debug.WriteLine($"Loading {point.X}x{point.Y}");
                buffer.Add(point, bmp);
            }
            return bmp;
        }

        void RemoveFurthestTile(Point point)
        {
            if (buffer.Count == 0)
                return; // No tiles to remove

            Point furthestPoint = buffer.Keys.First();
            double maxDistance = CalculateDistance(point, furthestPoint);

            foreach (var bufferedPoint in buffer.Keys)
            {
                double distance = CalculateDistance(point, bufferedPoint);
                if (distance > maxDistance)
                {
                    furthestPoint = bufferedPoint;
                    maxDistance = distance;
                }
            }
            Debug.WriteLine($"Remove {furthestPoint.X}x{furthestPoint.Y}");
            buffer.Remove(furthestPoint);

        }
        double CalculateDistance(Point p1, Point p2)
        {
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}

