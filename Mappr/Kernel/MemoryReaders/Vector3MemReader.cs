using System.Numerics;

namespace Mappr.Kernel.DataConverters
{
    public class Vector3MemReader : IMemoryReader<Vector3>
    {
        public Vector3 Convert(MemoryManager manager, nint address)
        {
            return new Vector3(
                manager.Read<float>(address + 0),
                manager.Read<float>(address + 4),
                manager.Read<float>(address + 8));
        }
    }
}
