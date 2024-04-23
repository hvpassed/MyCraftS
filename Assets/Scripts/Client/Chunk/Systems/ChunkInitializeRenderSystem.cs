 

using MyCraftS.Config;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace MyCraftS.Chunk
{
    [BurstCompile]
    public  struct CreateBlockEntity:IJobParallelFor
    {
        [ReadOnly] public NativeSlice<int> _blocks;
        [ReadOnly] public NativeArray<int> _heightMap;
        [ReadOnly] public int3 _chunkCoord;
        [ReadOnly] public int _chunkId;
        [ReadOnly] public int _chunkBufferIndex;
        public EntityCommandBuffer.ParallelWriter _entityCommandBuffer;
        
        
    /// <summary>
    /// 创建每个方块的实体
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="NotImplementedException"></exception>
        public void Execute(int index)
        {
             
        }
    }


    
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(ChunkSystemGroup),OrderLast = true)]
    public partial struct ChunkInitializeRenderSystem : ISystem
    {
        private EntityQuery _query;
        private JobHandle _lastJobHandle;
        private int _isGenerating;
        private EntityCommandBuffer _entityCommandBuffer;
        private CreateBlockEntity _lastJobStruct;
        private ComponentType _ChunkInitializeRenderTag;
 
        private void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ChunkBlocks>()
                .WithAll<ChunkCoord>()
                .WithAll<ChunkHeightMap>()
                .WithAll<ChunkID>()
                .WithAll<ChunkInitializeRenderTag>()
                .Build(ref state);
            _ChunkInitializeRenderTag = typeof(ChunkInitializeRenderTag);
        }


        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            if(CheckJobCompletion(ref state))
            {
                TryGetNowJob(ref state);
            }
        }

        private void TryGetNowJob(ref SystemState state)
        {
            var chunkEntitys = _query.ToEntityListAsync(Allocator.TempJob,out JobHandle getJob);
            getJob.Complete();
            
            var chunkEntity = chunkEntitys[0];
            chunkEntitys.Dispose();
            if (chunkEntity==Entity.Null)
            {
                return;
            }

            _entityCommandBuffer = new EntityCommandBuffer(Allocator.Persistent);
            ChunkBlocks chunkBlocks = state.EntityManager.GetComponentData<ChunkBlocks>(chunkEntity);
            ChunkCoord chunkCoord = state.EntityManager.GetComponentData<ChunkCoord>(chunkEntity);
            DynamicBuffer<ChunkHeightMap> chunkHeightMap = state.EntityManager.GetBuffer<ChunkHeightMap>(chunkEntity);
            ChunkID chunkId = state.EntityManager.GetComponentData<ChunkID>(chunkEntity);
            NativeArray<int> hm = new NativeArray<int>(TerrianConfig.ChunkSize * TerrianConfig.ChunkSize,Allocator.Persistent);
            for (int i = 0; i < TerrianConfig.ChunkSize*TerrianConfig.ChunkSize; i++)
            {
                hm[i] = chunkHeightMap[i].height;
            }
            _lastJobStruct = new CreateBlockEntity()
            {
                _blocks = chunkBlocks.blocks,
                _heightMap =  hm,
                _chunkCoord = chunkCoord.chunkCoord,
                _chunkId = chunkId.id,
                _chunkBufferIndex = chunkBlocks.bufferIndex,
                _entityCommandBuffer = _entityCommandBuffer.AsParallelWriter()
            };
            
            _lastJobHandle = _lastJobStruct.Schedule(chunkBlocks.blocks.Length, 1);
            _isGenerating = 1;
            state.EntityManager.RemoveComponent(chunkEntity, _ChunkInitializeRenderTag);
        }

        [BurstCompile]
        private void Process(ref SystemState state,Entity chunk)
        {
            
        }

        private bool CheckJobCompletion(ref SystemState state)
        {
            if (_isGenerating == 0)
            {
                return true;
            }

            if (!_lastJobHandle.IsCompleted)
            {
                return false;
            }

            _isGenerating = 0;
            _lastJobHandle.Complete();
            Debug.Log($"Chunk Initialize Render System:Complete Render Chunk {_lastJobStruct._chunkCoord}");
            
            
            
            _entityCommandBuffer.Playback(state.EntityManager);
            _entityCommandBuffer.Dispose();
            _lastJobStruct._heightMap.Dispose();
            //处理上次的数据
            return true;
        }
    }
}
