using System.Numerics;

namespace Mappr.Kernel.DataConverters
{
    public class QuaternionMemReader : IMemoryReader<Quaternion>
    {
        public Quaternion Convert(MemoryManager manager, nint address)
        {
            return new Quaternion(
                manager.Read<float>(address + 0),
                manager.Read<float>(address + 4),
                manager.Read<float>(address + 8),
                manager.Read<float>(address + 12));
        }
    }
}
