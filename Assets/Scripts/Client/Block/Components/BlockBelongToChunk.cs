using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Block
{
    public struct BlockBelongToChunk : ISharedComponentData
    {
        public int ChunkId;
        public int3 ChunkCoord;
        public int ChunkBufferIndex;
    }
}