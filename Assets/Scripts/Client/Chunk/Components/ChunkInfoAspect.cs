using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Chunk
{
    public readonly partial struct ChunkInfoAspect : IAspect
    {
        private readonly RefRO<ChunkBlocks> _chunkBlocks;
        private readonly RefRO<ChunkCoord> _chunkCoord;
        private readonly RefRO<ChunkID> _chunkID;
        private readonly DynamicBuffer<ChunkHeightMap> _chunkHeight;

        public int3 ChunkCoord()
        {
            return  _chunkCoord.ValueRO.chunkCoord;
        }
        
    }
}