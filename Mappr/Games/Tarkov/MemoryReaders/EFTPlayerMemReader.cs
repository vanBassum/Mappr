using Mappr.Games.Tarkov.Models;
using Mappr.Kernel;

namespace Mappr.Games.Tarkov.MemoryReaders
{
    public class EFTPlayerMemReader : IMemoryReader<EFTPlayer>
    {
        public EFTPlayer Convert(MemoryManager memoryManager, nint playerBase)
        {
            var head_index = 133;
            nint transObj = memoryManager.ReadChain(
                playerBase + 0xA8,          // Player body
                0x28,                       // SkeletonRootJoint
                0x28,                       // _values -> System.Collections.Generic.List<Transform>
                0x10,                       // base of list
                0x20 + 0x8 * head_index,  // Index of head bone -> EFT.BifacialTransform (i think)
                0x10);                      // Original : UnityEngine.Transform

            return new EFTPlayer
            {
                Head = memoryManager.Read<Transform>(transObj)
            };
        }
    }
}


