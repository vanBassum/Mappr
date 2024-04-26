using System.Numerics;

namespace Mappr.Controls
{
    public class CoordinateScaler2D
    {
        public Vector2 Scale { get; set; }  = Vector2.One;
        public Vector2 Offset { get; set; } = Vector2.Zero;


        public CoordinateScaler2D()
        {
        }

        public CoordinateScaler2D(Vector2 scale, Vector2 offset)
        {
            Scale = scale;
            Offset = offset;
        }

        public Vector2 ApplyTransformation(Vector2 coordinates)
        {
            return coordinates * Scale + Offset;
        }

        public Vector2 ReverseTransformation(Vector2 scaledCoordinates)
        {
            return (scaledCoordinates - Offset) / Scale;
        }

    }

    public class CoordinateRegression
    {
        public static CoordinateScaler2D Fit(Vector2[] world, Vector2[] map)
        {

            float[] xWorld = world.Select(c => c.X).ToArray();
            float[] xMap = map.Select(c => c.X).ToArray();
            var xCoofs = Fit(xWorld, xMap);

            float[] yWorld = world.Select(c => c.Y).ToArray();
            float[] yMap = map.Select(c => c.Y).ToArray();
            var yCoofs = Fit(yWorld, yMap);

            var scale = new Vector2(xCoofs.slope, yCoofs.slope);
            var offset = new Vector2(xCoofs.intercept, yCoofs.intercept);

            return new CoordinateScaler2D(scale, offset);
        }


        public static Vector2[] Error(Vector2[] world, Vector2[] map, CoordinateScaler2D scaler)
        {
            if (world.Length != map.Length)
                throw new ArgumentException("Input arrays must have the same length.");

            Vector2[] errors = new Vector2[world.Length];

            for (int i = 0; i < world.Length; i++)
            {
                Vector2 transformedWorldCoordinate = scaler.ApplyTransformation(world[i]);
                Vector2 error = transformedWorldCoordinate - map[i];
                errors[i] = error;
            }

            return errors;
        }


        public static (float slope, float intercept) Fit(float[] xValues, float[] yValues)
        {
            float slope;
            float intercept;
            if (xValues.Length == 2 && yValues.Length == 2)
            {
                slope = (yValues[1] - yValues[0]) / (xValues[1] - xValues[0]);
                intercept = yValues[0] - (xValues[0] * slope);
                return (slope, intercept);
            }

            return FitRegression(xValues, yValues);
        }

        public static (float slope, float intercept) FitRegression(float[] xValues, float[] yValues)
        {
            if (xValues.Length != yValues.Length || xValues.Length < 2)
            {
                throw new ArgumentException("Input arrays must have the same length and at least 2 data points.");
            }

            int n = xValues.Length;
            float sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            for (int i = 0; i < n; i++)
            {
                sumX += xValues[i];
                sumY += yValues[i];
                sumXY += xValues[i] * yValues[i];
                sumX2 += xValues[i] * xValues[i];
            }

            float slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            float intercept = (sumY - slope * sumX) / n;

            return (slope, intercept);
        }

    }
}

