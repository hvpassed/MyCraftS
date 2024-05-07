using System.Collections.Generic;
using MyCraftS.Chunk;
using MyCraftS.Chunk.Data;
using MyCraftS.Data.IO;
using MyCraftS.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.Block.Utils
{
    public static class BlockHelper
    {
        private static int3[] deltaPoses;
        public static Dictionary<BlockBelongToChunk, List<int3>> updatePositions;
        public static Dictionary<BlockBelongToChunk, NativeHashSet<int3>> destroyPositions;
        static BlockHelper()
        {
            updatePositions  = new Dictionary<BlockBelongToChunk, List<int3>>();
            destroyPositions = new Dictionary<BlockBelongToChunk, NativeHashSet<int3>>();
            deltaPoses = new int3[6]
            {
                new int3(1, 0, 0),
                new int3(-1, 0, 0),
                new int3(0, 1, 0),
                new int3(0, -1, 0),
                new int3(0, 0, 1),
                new int3(0, 0, -1)
            };
        }
        public static bool CanBlockByOther(int self,int other)
        {
            if (other == 0||other==-1)
                return false;
            if(self==0||self==-1)
                return true;
            var selfinfo =BlockDataManager.BlockIDToInfoLookUp[self];
            var otherinfo =BlockDataManager.BlockIDToInfoLookUp[other];
            //如果是液体，液体可以被液体任何方块遮挡（除了空气）
            if (selfinfo.isLiquid == MyCraftsBoolean.True)
            {
 
                return true;
            }
            else
            {
                if(otherinfo.isLiquid == MyCraftsBoolean.True)
                {
                    return false;
                }
 
                return true;
            }
 
        }

        public static bool IsBlocked(int3 WorldPosition)
        {
            int selfId = ChunkDataContainer.getBlockid(WorldPosition);
            bool isBlocked = true;
            foreach (int3 deltaPose in deltaPoses)
            {
                int otherId = ChunkDataContainer.getBlockid(WorldPosition + deltaPose);
                if (!CanBlockByOther(selfId, otherId))
                {
                    isBlocked = false;
                    break;
                }
            }

            return isBlocked;
        }
        public static bool IsBlocked(float3 WorldPosition)
        {
            float3 floord = math.floor(WorldPosition);
            int3 int3 = (int3) floord;
            return IsBlocked(int3);
        }

        public static void CreateBlock(EntityManager entityManager, int3 worldPos,int blockId)
        {
            var entityPrefab = BlockDataManager.BlockIdToEntityLookUp[blockId];
            var entity = entityManager.Instantiate(entityPrefab);
            entityManager.SetComponentData(entity, LocalTransform.FromMatrix(float4x4.TRS(
                new float3(worldPos.x,worldPos.y, worldPos.z),
                quaternion.identity,
                new float3(1, 1, 1)
                
                )));

            if (!IsBlocked(worldPos))
            {
                entityManager.RemoveComponent<DisableRendering>(entity);
                
            }

            entityManager.AddComponentData<BlockID>(entity, new BlockID()
            {
                Id = blockId
            });

            entityManager.RemoveComponent<BlockPrefabType>(entity);
            entityManager.AddComponentData<BlockShouldUpdate>(entity,new BlockShouldUpdate());
            entityManager.SetComponentEnabled<BlockShouldUpdate>(entity,false);
            ChunkDataContainer.getAllChunkInfo(worldPos, out int3 chunkCoord,out int chunkId,out int index);
            BlockBelongToChunk blockBelongToChunk = new BlockBelongToChunk()
            {
                ChunkId = chunkId,
                ChunkCoord = chunkCoord,
                ChunkBufferIndex = index
            };
            entityManager.AddSharedComponent(entity, blockBelongToChunk);
            ChunkDataContainer.setBlockId(worldPos,blockId);

        }
        
        public static void ClearUpdatePositions()
        {
            updatePositions.Clear();
        }
        public static void ClearDestroyPositions()
        {
            var enumerator = destroyPositions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Dispose();
            }
            destroyPositions.Clear();
        }

        public static void DestroyBlockAtPosition(int3 worldPosision)
        {
            ChunkDataContainer.getAllChunkInfo(worldPosision, out int3 chunkCoord, out int chunkId, out int index);
            BlockBelongToChunk blockBelongToChunk = new BlockBelongToChunk()
            {
                ChunkId = chunkId,
                ChunkCoord = chunkCoord,
                ChunkBufferIndex = index
            };
            if (destroyPositions.ContainsKey(blockBelongToChunk))
            {
                destroyPositions[blockBelongToChunk].Add(worldPosision);
            }
            else
            {
                NativeHashSet<int3> hashSet = new NativeHashSet<int3>(1, Allocator.Persistent);
                destroyPositions.Add(blockBelongToChunk, hashSet);
                destroyPositions[blockBelongToChunk].Add(worldPosision);
            }


        }
        
        public static void AddShouldUpdateBlock(int3 pos)
        {
            foreach (int3 deltaPose  in deltaPoses)
            {
                int3 curPos = pos + deltaPose;
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
        }
    }
}