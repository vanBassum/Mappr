namespace Mappr.Meuk
{
    public class FloatAverageCalculator
    {
        private float[] values;
        private int index;
        private float sum;

        public FloatAverageCalculator(int capacity)
        {
            values = new float[capacity];
            index = 0;
            sum = 0;
        }

        public float Add(float value)
        {
            sum -= values[index];
            values[index] = value;
            sum += value;
            index++;
            if (index >= values.Length)
                index = 0;
            return sum / values.Length;

        }
    }


}
