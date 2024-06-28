using MyCraftS.Block;
using Unity.Collections;
using Unity.Entities;

namespace MyCraftS.Chunk
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(ChunkSystemGroup),OrderLast = true)]
    public partial struct ChunkDeleteSystem:ISystem
    {
        
        private EntityQuery _toDeleteChunkQuery;

        private EntityQuery _BelongBlocks;
        void OnCreate(ref SystemState state)
        {
            _toDeleteChunkQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ChunkDeleteTag>()
                .Build(ref state);
            _BelongBlocks = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockType>()
                .WithNone<BlockPrefabType>()
                .WithAll<BlockBelongToChunk>()
                .Build(ref state);

        }
        
        
        void OnUpdate(ref SystemState state)
        {
            if (_toDeleteChunkQuery.CalculateEntityCount() == 0)
            {
                return;
            }
            
            
            var chunkEntities = _toDeleteChunkQuery.ToEntityArray(Allocator.Temp);
            var ChunkToDelete = chunkEntities[0];
            var chunkCoord = state.EntityManager.GetComponentData<ChunkCoord>(ChunkToDelete);
            


        }
    }
}