using System.Buffers;

namespace Mappr.Kernel.DataConverters
{
    public class IntMemReader : IMemoryReader<int>
    {
        public int Convert(MemoryManager manager, nint address)
        {
            return BitConverter.ToInt32(manager.ReadBytes(address, 4), 0);
        }
    }
}
