using System.Numerics;

namespace Mappr.Kernel.DataConverters
{
    public class Vector4Converter : IMemoryReader<Vector4>
    {
        public Vector4 Convert(MemoryManager manager, nint address)
        {
            return new Vector4(
                manager.Read<float>(address + 0),
                manager.Read<float>(address + 4),
                manager.Read<float>(address + 8),
                manager.Read<float>(address + 12));
        }
    }
}
