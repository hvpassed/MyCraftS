 
using System.Collections.Generic;
using MyCraftS.Chunk.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.Block.Update
{
 
    partial struct SetEntityShouldUpdateEnable:IJobEntity
    {
        [ReadOnly] public NativeArray<int3> positions;
        public EntityCommandBuffer.ParallelWriter ecbp;
        private void Execute([ChunkIndexInQuery] int sortKey, Entity entity,in LocalTransform transform)
        {
            if(positions.Contains((int3)math.floor(transform.Position)))
            {
                
            ecbp.SetComponentEnabled<BlockShouldUpdate>(sortKey,entity,true);
                //Debug.Log($"Set Entity Should Update Enable: {transform.Position}");
            }
             
        }
    }
    
    
    
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(BlockUpdateSystemGroup),OrderFirst = true)]
    public partial class BlockDestroyCleanUpSystem:SystemBase
    {
        private EntityQuery _destroyedCleanup;
        private EntityQuery _blockEntityQuery;
        private int3[] deltaPoses;
        protected override void OnCreate()
        {
            _destroyedCleanup = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockDestroyCleanUp>()
                .Build(this);
            _blockEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockType>()
                .WithAll<BlockBelongToChunk>()
                .WithAll<LocalTransform>()
                .Build(this);
            deltaPoses = new int3[6]
            {
                new int3(1,0,0),
                new int3(-1,0,0),
                new int3(0,1,0),
                new int3(0,-1,0),
                new int3(0,0,1),
                new int3(0,0,-1)
            };
            
        }
        
        protected override void OnUpdate()
        {
            Dictionary<BlockBelongToChunk,List<int3>> updatePositions = new Dictionary<BlockBelongToChunk, List<int3>>();
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            Entities
                .WithStoreEntityQueryInField(ref _destroyedCleanup)
                .ForEach(
                    (in Entity entity,in BlockDestroyCleanUp blockDestroyCleanUp) =>
                    {

                        int3 BlockPos = blockDestroyCleanUp.position;
                        foreach (int3 deltaPose  in deltaPoses)
                        {
                            int3 curPos = BlockPos + deltaPose;
                            int blockID = ChunkDataContainer.getBlockid(curPos);
                            if(blockID==0||blockID==-1)
                            {
                                continue;
                            }
                            ChunkDataContainer.getAllChunkInfo(curPos, out int3 chunkCoord, out int chunkId,out int index);
                            if (index == -1)
                            {
                                continue;
                            }
                            BlockBelongToChunk blockBelongToChunk = new BlockBelongToChunk
                            {
                               ChunkCoord = chunkCoord,
                               ChunkId = chunkId,
                               ChunkBufferIndex = index
                            };
                            if (updatePositions.ContainsKey(blockBelongToChunk))
                            {
                                updatePositions[blockBelongToChunk].Add(curPos);
                            }
                            else
                            {
                                updatePositions.Add(blockBelongToChunk,new List<int3>());
                                updatePositions[blockBelongToChunk].Add(curPos);
                            }
                        }

                        ecb.RemoveComponent<BlockDestroyCleanUp>(entity);
                         
                        
                    })
                .WithoutBurst()
                .Run();
                ecb.Playback(EntityManager);
                ecb.Dispose();
                var enumerator = updatePositions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var cecb = new EntityCommandBuffer(Allocator.TempJob);
                     BlockBelongToChunk blockBelongToChunk = enumerator.Current.Key;
                        NativeArray<int3> positions = new NativeArray<int3>(enumerator.Current.Value.ToArray(), Allocator.TempJob);
                     _blockEntityQuery.SetSharedComponentFilter<BlockBelongToChunk>(blockBelongToChunk);
                     this.Dependency = new SetEntityShouldUpdateEnable()
                     {
                         positions = positions,
                         ecbp = cecb.AsParallelWriter()
                     }.ScheduleParallel(_blockEntityQuery, this.Dependency);
                     this.Dependency.Complete();
                     cecb.Playback(EntityManager);
                     cecb.Dispose();
                     positions.Dispose();
                }
        }
    }
}