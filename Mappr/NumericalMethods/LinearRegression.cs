using System.Data;

namespace Mappr.NumericalMethods
{
    public class LinearRegression
    {
        public static (float A, float B) PerformLinearRegression(float[] xValues, float[] yValues)
        {
            if (xValues.Length != yValues.Length)
                throw new ArgumentException("Input arrays must have the same length.");

            int n = xValues.Length;
            float sumX = xValues.Sum();
            float sumY = yValues.Sum();
            float sumXY = xValues.Zip(yValues, (x, y) => x * y).Sum();
            float sumX2 = xValues.Select(x => x * x).Sum();
            float denominator = n * sumX2 - sumX * sumX;
            float A = (n * sumXY - sumX * sumY) / denominator;
            float B = (sumY * sumX2 - sumX * sumXY) / denominator;
            return (A, B);
        }
    }

}

