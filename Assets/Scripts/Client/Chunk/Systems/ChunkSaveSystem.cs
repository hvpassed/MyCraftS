

 
using System;
using MyCraftS.Block;
using MyCraftS.Chunk.Data;
using MyCraftS.Setting;
using MyCraftS.SystemManage;
using System.IO;

using MyCraftS.Block.Utils;
using MyCraftS.Config;
using MyCraftS.Database;
using MyCraftS.Database.Model;
using MyCraftS.Player;
using MyCraftS.Player.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using BinaryWriter = Unity.Entities.Serialization.BinaryWriter;

namespace MyCraftS.Chunk
{
    [UpdateInGroup(typeof(ChunkSystemGroup))]
    [UpdateBefore(typeof(ChunkDeleteSystem))]
    public partial class ChunkSaveSystem : SystemBase 
    {
        //[BurstCompile]
        private partial struct ConvertToBlockSerializeData:IJobEntity
        {
            public NativeReference<int> count;
            [ReadOnly] public int offset;
            [ReadOnly] public int ChunkSize;
            [ReadOnly] public bool disableRendering;
            [NativeDisableParallelForRestriction]
            public NativeList<BlockSerializationData> blockSerializationDatas;
            private void Execute([EntityIndexInQuery] int sortKey, Entity entity,in LocalTransform localTransform,in BlockID id)
            {
                //Debug.Log($"all length:{blockSerializationDatas.Capacity}"+count.Value);
                count.Value++;
                //blockSerializationDatas[sortKey + offset] = convert(localTransform, id.Id);
                blockSerializationDatas.Add(convert(localTransform,id.Id));
                    
            }

            private BlockSerializationData convert(in LocalTransform transform, int blockId)
            {
                int3 chunkPos = GetChunkCoord(transform.Position);
                int3 deltaPos = (int3)math.floor(transform.Position) - chunkPos;

                return new BlockSerializationData()
                {
                    chunkPosition = chunkPos,
                    localPosition = deltaPos,
                    blockId = blockId,
                    disableRendering = disableRendering
                };

            }

            public  int3 GetChunkCoord(float3 entityPos)
            {
                int fx = Mathf.FloorToInt(entityPos.x);
                int fz = Mathf.FloorToInt(entityPos.z);
                int x = fx - (MyMod(fx, ChunkSize));
                int z = fz - (MyMod(fz, ChunkSize));
                return new int3(x, 0, z);
            }
            
            
            private  int MyMod(int x,int m)
            {
                int r = x % m;
                if (r < 0 && m > 0)
                {
                    r += m;
                }
                else if (r > 0 && m < 0)
                {
                    r += m; 
                }
                return r;
            }
        }

        private float time;
        

        private EntityQuery _entityToSaveQueryDiabled,_entityToSaveQuery,_chunkQuery;


        protected override void OnCreate()
        {
            time = 0;
            _entityToSaveQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockType>()
                .WithNone<BlockPrefabType>()
                .WithAll<BlockBelongToChunk>()
                .WithNone<DisableRendering>()
                .WithAll<LocalTransform,BlockID>()
                .Build(this);
            _entityToSaveQueryDiabled =new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockType>()
                .WithNone<BlockPrefabType>()
                .WithAll<BlockBelongToChunk>()
                .WithAll<DisableRendering>()
                .WithAll<LocalTransform,BlockID>()
                .Build(this);
            
            _chunkQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAspect<ChunkInfoAspect>()
                .WithAll<ChunkType>()
                .WithNone<ChunkInitializeRenderTag>()
                .WithNone<ChunkDeleteTag>()
                .Build(this);


            SystemManager.GameInitialedEvent += StartSystem;
            this.Enabled = false;
        }
        private void StartSystem()
        {
            
            this.Enabled = true;
        }

        protected override void OnUpdate()
        {
            time += SystemAPI.Time.DeltaTime;
            if (time < SettingManager.GameSetting.SaveGap)
            {
                return;
            }
            time -= SettingManager.GameSetting.SaveGap;

            var Chunks = _chunkQuery.ToEntityArray(Allocator.Temp);
            if (Chunks.Length == 0)
            {
                return;
            }

            LocalTransform playerTransform = EntityManager.GetComponentData<LocalTransform>(PlayerDataContainer.playerEntity);
            int3 playerPos = new int3((int)playerTransform.Position.x, 0, (int)playerTransform.Position.z);
            Entity toSaveChunk = Chunks[0];
            int3 ChunkCoords = new int3();
            bool find = false;
            this.Dependency.Complete();
            
            for (int i = 0; i < Chunks.Length; i++)
            {
                var chunkInfoAspect = EntityManager.GetAspect<ChunkInfoAspect>(Chunks[i]);
                int3 coords = new int3(chunkInfoAspect.ChunkCoord().x, 0, chunkInfoAspect.ChunkCoord().z);
                int distance = (int)math.distance(coords/16, playerPos/16);
                
                if(distance> SettingManager.PlayerSetting.ViewDistance)
                {
                    toSaveChunk = Chunks[i];
                    find = true;
                    ChunkCoords = coords;
                    break;
                }
            }

            if (!find)
            {
                return;
            }
            
            ChunkDataContainer.getAllChunkInfo(ChunkCoords,out int3 chunkCoord,
                out int chunkId,out int chunkIndex);
            
            
            BlockBelongToChunk filter = new BlockBelongToChunk()
            {   
                ChunkCoord = chunkCoord,
                ChunkId = chunkId,
                ChunkBufferIndex = chunkIndex
            };
            
            _entityToSaveQuery.SetSharedComponentFilter<BlockBelongToChunk>(filter);
            _entityToSaveQueryDiabled.SetSharedComponentFilter(filter);
            if (_entityToSaveQuery.IsEmpty)
            {
                return;
            }



            NativeReference<int> count = new NativeReference<int>(Allocator.TempJob);
            NativeList<BlockSerializationData> blockSerializationDatas = new NativeList<BlockSerializationData>(
                TerrianConfig.ChunkSize*TerrianConfig.ChunkSize*TerrianConfig.MaxHeight,Allocator.TempJob);
            count.Value = 0;
            this.Dependency = new ConvertToBlockSerializeData()
            {
                count = count,
                offset = 0,
                ChunkSize = TerrianConfig.ChunkSize,
                disableRendering = false,
                blockSerializationDatas = blockSerializationDatas
            }.Schedule(_entityToSaveQuery, this.Dependency);
            this.Dependency.Complete();
            this.Dependency = new ConvertToBlockSerializeData()
            {
                count = count,
                offset = count.Value,
                ChunkSize = TerrianConfig.ChunkSize,
                disableRendering = true,
                blockSerializationDatas = blockSerializationDatas
            }.Schedule(_entityToSaveQueryDiabled, this.Dependency);
            this.Dependency.Complete();
            Debug.Log("Convert Complete");
            count.Dispose();
            ChunkDataModel chunkDataModel = new ChunkDataModel();
            chunkDataModel.X = chunkCoord.x;
            chunkDataModel.Z = chunkCoord.z;
            chunkDataModel.ChunkID = chunkId;
            BlockSerializationData[] blockSerialization = blockSerializationDatas.ToArray(Allocator.Temp).ToArray();
            chunkDataModel.BlocksData = BlockSerialization.Serialize(blockSerialization);
            ChunkDataModel.Upsert(chunkDataModel,DatabaseManager.GameDatabase);
            blockSerializationDatas.Dispose();
            EntityManager.AddComponent<ChunkDeleteTag>(toSaveChunk);
            //this.Enabled = false;
        }
    }
}
