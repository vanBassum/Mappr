namespace Mappr.Kernel.DataConverters
{
    public class FloatConverter : IMemoryReader<float>
    {
        public float Convert(MemoryManager manager, nint address)
        {
            return BitConverter.ToSingle(manager.ReadBytes(address, 4), 0);
        }
    }
}
