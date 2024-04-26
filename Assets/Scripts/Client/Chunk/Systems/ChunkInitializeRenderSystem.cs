 

using Client.SystemManage;
using MyCraftS.Block;
using MyCraftS.Block.Utils;
using MyCraftS.Config;
using MyCraftS.Data.IO;
using MyCraftS.Setting;
using Test;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MyCraftS.Chunk
{
 
    public  struct CreateBlockEntity:IJobParallelFor
    {
        [ReadOnly] public NativeArray<int> _blocks;
        [ReadOnly] public NativeArray<int> _heightMap;
        [ReadOnly] public int3 _chunkCoord;
        [ReadOnly] public int _chunkId;
        [ReadOnly] public int _chunkBufferIndex;
        public EntityCommandBuffer.ParallelWriter _entityCommandBuffer;
        [ReadOnly]
        public NativeArray<int3> _DeltaPos;
        [ReadOnly]
        public NativeHashMap<int, Entity> _blockIdToEntityLookUp;

        
        
        
        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        /// 创建每个方块的实体
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute(int index)
        {
            BlockBelongToChunk blockBelongToChunk = new BlockBelongToChunk()
            {
                ChunkId = _chunkId,
                ChunkCoord = _chunkCoord,
                ChunkBufferIndex = _chunkBufferIndex
            };
            int startIndex = index * TerrianConfig.ChunkSize;
            for (int y = startIndex; y < (startIndex + TerrianConfig.ChunkSize) && y < TerrianConfig.MaxHeight; y++)
            {
                for (int x = 0; x < TerrianConfig.ChunkSize; x++)
                {
                    for (int z = 0; z < TerrianConfig.ChunkSize; z++)
                    {
                        if (y > _heightMap[ChunkDataHelper.IndexGetterXZ(x, z)])
                        {
                            continue;
                        }

                        bool canShow = false;
                        int3 curPos = new int3(x, y, z);
                        int blockIndex = ChunkDataHelper.IndexGetter(x, y, z);
                        int selfId = _blocks[blockIndex];
                        if (selfId == 0)
                        {
                            continue;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            int3 checkPos= curPos+_DeltaPos[i];
                            int otherId = GetOtherBlockID(checkPos);
                            if(!BlockHelper.CanBlockByOther(selfId,otherId))
                            {
                                canShow = true;
                                break;
                            }
                        }


                        var entity = _entityCommandBuffer.Instantiate(0, _blockIdToEntityLookUp[selfId]);
                        if (!canShow)
                        {
                            //_entityCommandBuffer.SetComponent();
                            
                        }
                        else
                        {
                            _entityCommandBuffer.RemoveComponent<DisableRendering>(1,entity);
                        }

                        _entityCommandBuffer.SetComponent(3, entity,  LocalTransform.FromMatrix(float4x4.TRS(
                            new float3(_chunkCoord.x+x,y,_chunkCoord.z+z),quaternion.identity, new float3(1,1,1)
                            )));
                        _entityCommandBuffer.SetComponent<BlockID>(4,entity,new BlockID(){Id=selfId});
                        _entityCommandBuffer.RemoveComponent<BlockPrefabType>(5, entity);
                        _entityCommandBuffer.AddSharedComponent(6, entity, blockBelongToChunk);
                    }
                }
            }
            
            
        }

 
 
 

        public int GetOtherBlockID(int3 other)
        {
            if(other.y>=TerrianConfig.MaxHeight||other.y<0 || 
               other.x<0||other.x>=TerrianConfig.ChunkSize||
               other.z<0||other.z>=TerrianConfig.ChunkSize)
                return 0 ;
            else
            {
                int index =ChunkDataHelper.IndexGetter(other.x,other.y,other.z);
                if (_blocks[index] < 0)
                {
 
                    return 0;
                }

                return _blocks[index];
            }
        }
    
 
    }

    
    //[RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(ChunkSystemGroup),OrderLast = true)]
    public partial struct ChunkInitializeRenderSystem : ISystem
    {
        private EntityQuery _query;
        private JobHandle _lastJobHandle;
        private int _isGenerating;
        private EntityCommandBuffer _entityCommandBuffer;
        private CreateBlockEntity _lastJobStruct;
        private ComponentType _ChunkInitializeRenderTag;
        private NativeArray<int3> _deltaPos;
        private int initGameShouldRender;

        private bool isGameInitialized;
        
        private int renderedCount;
        private void OnCreate(ref SystemState state)
        {
            initGameShouldRender = (SettingManager.PlayerSetting.ViewDistance * 2 + 1) *
                                   (SettingManager.PlayerSetting.ViewDistance * 2 + 1);
            renderedCount = 0;
            isGameInitialized = false;
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ChunkBlocks>()
                .WithAll<ChunkCoord>()
                .WithAll<ChunkHeightMap>()
                .WithAll<ChunkID>()
                .WithAll<ChunkInitializeRenderTag>()
                .Build(ref state);
            _ChunkInitializeRenderTag = typeof(ChunkInitializeRenderTag);
            _deltaPos = new NativeArray<int3>(6, Allocator.Persistent);
            _deltaPos[0] = new int3(1, 0, 0);
            _deltaPos[1] = new int3(-1, 0, 0);
            _deltaPos[2] = new int3(0, 1, 0);
            _deltaPos[3] = new int3(0, -1, 0);
            _deltaPos[4] = new int3(0, 0, 1);
            _deltaPos[5] = new int3(0, 0, -1);
            _isGenerating = 0;
        }


        //[BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            //初始化渲染完成
            if (!isGameInitialized &&renderedCount == initGameShouldRender)
            {
                isGameInitialized = true;
                Debug.Log($"Game Initialized");
                SystemManager.GameInitialized();
            }
            if(CheckJobCompletion(ref state))
            {
                TryGetNowJob(ref state);
            }
            
            
            
        }

        private void TryGetNowJob(ref SystemState state)
        {
            var chunkEntitys = _query.ToEntityListAsync(Allocator.TempJob,out JobHandle getJob);
            getJob.Complete();
            if (_query.CalculateEntityCount() == 0)
            {
                return;
            }
            var chunkEntity = chunkEntitys[0];
            chunkEntitys.Dispose();
 

            _entityCommandBuffer = new EntityCommandBuffer(Allocator.Persistent);
            ChunkBlocks chunkBlocks = state.EntityManager.GetComponentData<ChunkBlocks>(chunkEntity);
            NativeArray<int> blocks = new NativeArray<int>(chunkBlocks.blocks.Length, Allocator.TempJob);
            for(int i = 0;i<chunkBlocks.blocks.Length;i++)
            {
                blocks[i] = chunkBlocks.blocks[i];
            }
            ChunkCoord chunkCoord = state.EntityManager.GetComponentData<ChunkCoord>(chunkEntity);
            DynamicBuffer<ChunkHeightMap> chunkHeightMap = state.EntityManager.GetBuffer<ChunkHeightMap>(chunkEntity);
            ChunkID chunkId = state.EntityManager.GetComponentData<ChunkID>(chunkEntity);
            NativeArray<int> hm = new NativeArray<int>(TerrianConfig.ChunkSize * TerrianConfig.ChunkSize,Allocator.Persistent);
            for (int i = 0; i < TerrianConfig.ChunkSize*TerrianConfig.ChunkSize; i++)
            {
                hm[i] = chunkHeightMap[i].height;
            }

            NativeHashMap<int,Entity> lookUp = new NativeHashMap<int, Entity>(BlockDataManager.BlockIdToEntityLookUp.Count,Allocator.TempJob);
            var enumtor = BlockDataManager.BlockIdToEntityLookUp.GetEnumerator();
            while (enumtor.MoveNext())
            {
                lookUp.Add(enumtor.Current.Key,enumtor.Current.Value);
            }
            _lastJobStruct = new CreateBlockEntity()
            {
                _blocks = blocks,
                _heightMap =  hm,
                _chunkCoord = chunkCoord.chunkCoord,
                _chunkId = chunkId.id,
                _chunkBufferIndex = chunkBlocks.bufferIndex,
                _entityCommandBuffer = _entityCommandBuffer.AsParallelWriter(),
                _DeltaPos = _deltaPos,
                _blockIdToEntityLookUp = lookUp
            };
            
            _lastJobHandle = _lastJobStruct.Schedule(TerrianConfig.MaxHeight/TerrianConfig.ChunkSize+1, 1);

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
            //Debug.Log($"Chunk Initialize Render System:Complete Render Chunk {_lastJobStruct._chunkCoord}");
 
            
            
            _entityCommandBuffer.Playback(state.EntityManager);
            _entityCommandBuffer.Dispose();
            _lastJobStruct._heightMap.Dispose();
            _lastJobStruct._blocks.Dispose();
            //处理上次的数据
            _lastJobStruct._blockIdToEntityLookUp.Dispose();
            renderedCount++;
            return true;
        }
        [BurstCompile]
        private void OnDestroy()
        {
            _deltaPos.Dispose();
        }
    }
}