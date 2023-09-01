using System.Numerics;

namespace Mappr.Controls
{
    public struct CoordinateSet
    {
        public Vector2 World { get; }
        public Vector2 Map { get; }

        public CoordinateSet(Vector2 world, Vector2 map)
        {
            World = world;
            Map = map;
        }
    }

    public class VectorRegression
    {
        private List<CoordinateSet> coordinateSets;
        private Vector<float> coefficients;

        public VectorRegression(List<CoordinateSet> coordinateSets)
        {
            if (coordinateSets.Count < 2)
                throw new ArgumentException("Invalid input data. The list of coordinate sets must have at least two elements.");

            this.coordinateSets = coordinateSets;
            coefficients = CalculateCoefficients();
        }

        public Vector<float> CalculateCoefficients()
        {
            if (coordinateSets.Count < 2)
                throw new ArgumentException("Invalid input data. The list of coordinate sets must have at least two elements.");
            
            int numSamples = coordinateSets.Count;

            // Calculate coefficients using linear regression formula
            float sumX = 0.0f;
            float sumY = 0.0f;
            float sumXX = 0.0f;
            float sumXY = 0.0f;

            foreach (var set in coordinateSets)
            {
                sumX += set.World.X;
                sumY += set.World.Y;
                sumXX += set.World.X * set.World.X;
                sumXY += set.World.X * set.World.Y;
            }

            float a = (numSamples * sumXY - sumX * sumY) / (numSamples * sumXX - sumX * sumX);
            float b = (sumY - a * sumX) / numSamples;

            return new Vector<float>(new float[] { b, a });
        }

        public Vector2 WorldToMap(Vector2 world)
        {
            float mapX = coefficients[0] + coefficients[1] * world.X;
            float mapY = world.Y; // Assuming Y is not transformed in this simple example

            return new Vector2(mapX, mapY);
        }

        public Vector2 MapToWorld(Vector2 map)
        {
            float worldX = (map.X - coefficients[0]) / coefficients[1];
            float worldY = map.Y; // Assuming Y is not transformed in this simple example

            return new Vector2(worldX, worldY);
        }
    }
}

