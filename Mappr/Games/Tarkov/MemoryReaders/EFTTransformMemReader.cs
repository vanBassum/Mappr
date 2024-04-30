using Mappr.Games.Tarkov.Models;
using Mappr.Kernel;
using System.Numerics;

namespace Mappr.Games.Tarkov.MemoryReaders
{
    public class EFTTransformMemReader : IMemoryReader<Transform>
    {
        public Transform? Convert(MemoryManager memoryManager, nint transObj)
        {
            var matrix = memoryManager.ReadAddress(transObj + 0x38);
            var transformIndex = memoryManager.Read<int>(transObj + 0x40);
            var transformList = memoryManager.ReadAddress(matrix + 0x18);
            var parentIndicesList = memoryManager.ReadAddress(matrix + 0x20);
            return GetTransform(memoryManager, transformList, parentIndicesList, transformIndex);
        }

        Transform? GetTransform(MemoryManager memoryManager, nint transformList, nint parentIndicesList, int transformIndex)
        {
            if (transformIndex == -1)
                return null;

            nint transformBase = transformList + 0x28 * transformIndex;
            int parentIndex = memoryManager.Read<int>(parentIndicesList + 4 * transformIndex);
            Transform? parent = GetTransform(memoryManager, transformList, parentIndicesList, parentIndex);
            return new Transform
            {
                Parent = parent,
                Position = memoryManager.Read<Vector3>(transformBase),
                Rotation = memoryManager.Read<Quaternion>(transformBase + 12),
                Scale = memoryManager.Read<Vector3>(transformBase + 28)
            };
        }
    }
}


