using MyCraftS.Chunk;
using MyCraftS.Chunk.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MyCraftS.Block
{
    public readonly partial struct BlockInfoAspect : IAspect
    {
        private readonly RefRO<LocalTransform> transform;
        private readonly RefRO<BlockID> blockID;

        public int BlockID
        {
            get
            {
                return blockID.ValueRO.Id;
            }
        }
        public int3 GetChunkPosition()
        {
           int3 chunkCoord = ChunkDataHelper.GetChunkCoord(transform.ValueRO.Position);
           int3 blockCoord = (int3)math.floor(transform.ValueRO.Position);
           return blockCoord - chunkCoord;
        }
    }
}