using Mappr.Games.Tarkov.Models;
using Mappr.Kernel;

namespace Mappr.Games.Tarkov.MemoryReaders
{
    public class EFTLocalGameWorldMemReader : IMemoryReader<EFTLocalGameWorld>
    {
        public EFTLocalGameWorld Convert(MemoryManager manager, nint gameWorld)
        {
            nint localGameWorld = manager.ReadChain(gameWorld + 0x30, 0x18, 0x28);
            nint mainPlayerAddr = manager.ReadChain(localGameWorld + 0x118);

            return new EFTLocalGameWorld
            {
                MainPlayer = manager.Read<EFTPlayer>(mainPlayerAddr),
                //RegisteredPlayers = GetPlayers(manager, localGameWorld)
            };
        }


        List<EFTPlayer> GetPlayers(MemoryManager manager, nint localGameWorld)
        {
            List<EFTPlayer> result = new List<EFTPlayer>();

            nint list = manager.ReadAddress(localGameWorld + 0xC0);


            nint listBase = manager.ReadAddress(list + 0x10);
            int listCount = manager.Read<int>(list + 0x18);

            for(int i=0; i<listCount; i++)
            {
                nint addr = listBase + i * 0x08;

                var a = manager.ReadAddress(addr);
                var b = manager.ReadAddress(a + 0xA8);
                var c = manager.ReadAddress(b + 0x28);
                //var d = manager.ReadAddress(c + 0x28);

                //result.Add(manager.Read<EFTPlayer>(addr));
            }


            return result;
        }


    }

}


