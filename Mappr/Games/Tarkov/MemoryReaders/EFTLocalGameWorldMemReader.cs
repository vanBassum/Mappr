using Mappr.Games.Tarkov.Models;
using Mappr.Kernel;

namespace Mappr.Games.Tarkov.MemoryReaders
{
    public class EFTLocalGameWorldMemReader : IMemoryReader<EFTLocalGameWorld>
    {
        public EFTLocalGameWorld Convert(MemoryManager manager, nint gameWorld)
        {
            nint mainPlayerAddr = manager.ReadChain(gameWorld + 0x30, 0x18, 0x28, 0x118);

            return new EFTLocalGameWorld
            {
                MainPlayer = manager.Read<EFTPlayer>(mainPlayerAddr)
            };
        }
    }
}


