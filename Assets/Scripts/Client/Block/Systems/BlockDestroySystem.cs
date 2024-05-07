using MyCraftS.Action;
using MyCraftS.Block.Update;
using MyCraftS.Block.Utils;
using MyCraftS.Chunk;
using MyCraftS.Chunk.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyCraftS.Block
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(BlockUpdateSystemGroup), OrderFirst = true)]
    public partial class BlockDestroySystem : SystemBase
    {
        [BurstCompile]
        public partial struct DestroyEntity : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecbp;
            [ReadOnly] public NativeHashSet<int3> destroyPositions;

            private void Execute([ChunkIndexInQuery] int sortKey,Entity entity,in LocalTransform localTransform)
            {
                int3 intpos = (int3)math.floor(localTransform.Position);
                if (destroyPositions.Contains(intpos))
                {
                    ecbp.AddComponent(sortKey, entity, new BlockDestroyCleanUp()
                    {
                        position = intpos
                    });
                    ecbp.DestroyEntity(sortKey, entity);
                }
            }
        }


        protected EntityQuery _destroyQuery;
        protected override void OnCreate()
        {
            _destroyQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockBelongToChunk>()
                .WithAll<BlockType>()
                .WithAll<LocalTransform>()
                .Build(this);
        }


        protected override void OnUpdate()
        {
            if (_destroyQuery.IsEmpty)
            {
                return;
            }

            var enumerator = BlockHelper.destroyPositions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _destroyQuery.SetSharedComponentFilter(enumerator.Current.Key);
                EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
                this.Dependency = new DestroyEntity()
                {
                    destroyPositions = enumerator.Current.Value,
                    ecbp = ecb.AsParallelWriter()
                }.ScheduleParallel(_destroyQuery, this.Dependency);
                this.Dependency.Complete();
                ecb.Playback(EntityManager);
                ecb.Dispose();
                var enum2 = enumerator.Current.Value.GetEnumerator();
                while(enum2.MoveNext())
                {
                    ChunkDataContainer.setBlockId(enum2.Current, 0);
                }

            }
            BlockHelper.ClearDestroyPositions();
        }
        //private void DestroyBlockFromPosition(int3 position)
        //{
        //    ChunkDataContainer.setBlockId(position, 0);

        //    ChunkDataContainer.getAllChunkInfo(position, out int3 chunkCoord, out int chunkId, out int chunkIndex);
        //    if (chunkIndex == -1)
        //    {
        //        Debug.LogError($"error chunk index at {position}");
        //        return;
        //    }
        //    BlockBelongToChunk filter = new BlockBelongToChunk()
        //    {
        //        ChunkCoord = chunkCoord,
        //        ChunkId = chunkId,
        //        ChunkBufferIndex = chunkIndex
        //    };
        //    _destroyQuery.SetSharedComponentFilter(filter);
        //    EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        //    this.Dependency = new DestroyEntity()
        //    {
        //        destroyPositions = position,
        //        ecbp = ecb.AsParallelWriter()
        //    }.ScheduleParallel(_destroyQuery, this.Dependency);
        //    this.Dependency.Complete();
        //    ecb.Playback(EntityManager);
        //    ecb.Dispose();

        //}
    }
}
