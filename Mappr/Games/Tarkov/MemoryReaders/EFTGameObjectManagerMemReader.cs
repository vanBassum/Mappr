using Mappr.Games.Tarkov.Models;
using Mappr.Kernel;

namespace Mappr.Games.Tarkov.MemoryReaders
{
    public class EFTGameObjectManagerMemReader : IMemoryReader<EFTGameObjectManager>
    {
        public EFTGameObjectManager Convert(MemoryManager manager, nint gameObjectManager)
        {
            return new EFTGameObjectManager
            {
                GameWorld = manager.Read<EFTLocalGameWorld>(FindGameWorldAddress(manager, gameObjectManager))
            };
        }

        nint FindGameWorldAddress(MemoryManager memoryManager, nint gameObjectManager)
        {
            nint activeNodes = memoryManager.ReadChain(gameObjectManager + 0x28, 0);
            nint lastActiveNode = memoryManager.ReadChain(gameObjectManager + 0x20, 0);
            nint cObject = memoryManager.ReadAddress(activeNodes + 0x8);    //0x00 gives problems, so start at 0x08

            do
            {
                nint obj = memoryManager.ReadAddress(cObject + 0x10);
                nint nameptr = memoryManager.ReadAddress(obj + 0x60);
                string? name = memoryManager.Read<string>(nameptr);

                if (name == "GameWorld")
                    return obj;

                cObject = memoryManager.ReadAddress(cObject + 0x8);
            }
            while (cObject != lastActiveNode);
            return 0;
        }
    }
}


