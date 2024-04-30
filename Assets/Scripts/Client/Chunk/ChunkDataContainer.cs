using MyCraftS.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.VisualScripting.Member;

namespace MyCraftS.Chunk.Data
{
    public unsafe static class ChunkDataContainer
    {

        public static int ChunkIDAllocator = 0;
        public static Entity ChunkManager;

        public static int AllocatedCount = 0;
        /// <summary>
        /// 保存区块加载的数据
        /// </summary>
        public static NativeArray<int> BlocksData;

        public static NativeHashMap<int, int3> IndexToChunkCoord;

        public static NativeHashMap<int3, int> ChunkCoordToIndex;
        
        public static NativeHashMap<int,int3> ChunkIDToCoord;

        public const int ChunkSize = TerrianConfig.ChunkSize * TerrianConfig.ChunkSize*TerrianConfig.MaxHeight;
        static ChunkDataContainer()
        {
            BlocksData = new NativeArray<int>(TerrianConfig.ChunkSize*TerrianConfig.ChunkSize
                *TerrianConfig.MaxHeight*TerrianConfig.MaxLoadedChunk, Allocator.Persistent);

            IndexToChunkCoord = new NativeHashMap<int, int3>(TerrianConfig.MaxLoadedChunk, Allocator.Persistent);
            ChunkCoordToIndex = new NativeHashMap<int3, int>(TerrianConfig.MaxLoadedChunk, Allocator.Persistent);
            ChunkIDToCoord = new NativeHashMap<int, int3>(TerrianConfig.MaxLoadedChunk, Allocator.Persistent);
        }
        

        public static int getBlockid(int3 WorldCoord)
        {
            int index = getIndex(WorldCoord);
            if(index == -1)
            {
                return -1;
            }
            return BlocksData[index];
 
        }


        public static void setBlockId(int3 worldCoord, int id)
        {
            int ind = getIndex(worldCoord);
            if (ind != -1)
            {
                BlocksData[ind] = id;
            }
        }
        
        public static int getBlockid(int3 ChunkPos,int3 LocalPos)
        {
            int index = getIndex(ChunkPos, LocalPos);
            if (index == -1)
            {
                return -1;
            }
            return BlocksData[index];
        }
        public static int getIndex(int3 WorldPos)
        {
            int3 chunkPos = ChunkDataHelper.GetChunkCoord(WorldPos);
            int3 LocalPos = new int3(WorldPos.x - chunkPos.x, WorldPos.y, WorldPos.z - chunkPos.z);

            return getIndex(chunkPos, LocalPos);
        }


        public static int getIndex(int3 ChunkPos,int3 LocalPos)
        {
            if(ChunkCoordToIndex.TryGetValue(ChunkPos,out int index))
            {
                return index*ChunkSize+ChunkDataHelper.IndexGetter(LocalPos.x,LocalPos.y,LocalPos.z);
            }
            else
            {
                return -1;
            }
        }
        
        public static int getChunkID(int3 worldPos)
        {
            int3 chunkPos = ChunkDataHelper.GetChunkCoord(worldPos);
            var enumerator = ChunkIDToCoord.GetEnumerator();
            while (enumerator.MoveNext())
            {
                bool3 boo = enumerator.Current.Value == chunkPos;
                if (boo.x == true && boo.y == true && boo.z == true)
                {
                    return enumerator.Current.Key;
                }
            }
            return -1;
        }
        /// <summary>
        /// 返回索引
        /// </summary>
        /// <param name="chunkCoord"></param>
        /// <returns>-1 表示区块buffer用尽</returns>
        ///  
        public static int Allocate(int3 chunkCoord,out int id)
        {
            id = -1;
            if (AllocatedCount >= TerrianConfig.MaxLoadedChunk)
            {
                
                return -1;
            }
            
            ChunkInfoClear(chunkCoord);

            for(int i = 0;i<TerrianConfig.MaxLoadedChunk; i++)
            {
                if (!IndexToChunkCoord.ContainsKey(i))
                {
                    IndexToChunkCoord.Add(i, chunkCoord);
                    ChunkCoordToIndex.Add(chunkCoord, i);
                    AllocatedCount++;
                    id = ChunkIDAllocator ;
                    ChunkIDToCoord.Add(ChunkIDAllocator, chunkCoord);
                    ChunkIDAllocator++;
                    return i;
                }

            }
            return -1;

        }


        public static NativeSlice<int> Slice(int index)
        {
            return  new NativeSlice<int>(BlocksData, index*ChunkSize, ChunkSize);

        }
        /// <summary>
        /// 通过ChunkCoord清理buffer
        /// </summary>
        /// <param name="ChunkCoord"></param>
        public static void ChunkInfoClear(int3 ChunkCoord)
        {
            if (ChunkCoordToIndex.ContainsKey(ChunkCoord))
            {
                int index = -1;
                var enumerator = IndexToChunkCoord.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    bool3 boo = enumerator.Current.Value == ChunkCoord;
                    if (boo.x == true && boo.y == true && boo.z == true)
                    {
                        index = enumerator.Current.Key;
                        IndexToChunkCoord.Remove(enumerator.Current.Key);
                    }
                }
                ChunkCoordToIndex.Remove(ChunkCoord);
                var enumeratorChunkIDToCoord = ChunkIDToCoord.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    bool3 boo = enumerator.Current.Value == ChunkCoord;
                    if (boo.x == true && boo.y == true && boo.z == true)
                    {
                        ChunkIDToCoord.Remove(enumerator.Current.Key);
                    }
                }
                AllocatedCount--;
                MemClear(index);
            }
        }
        /// <summary>
        /// 通过index清理chunk的buffer
        /// </summary>
        /// <param name="index"></param>
        public static void ChunkInfoClear(int index)
        {
            if (IndexToChunkCoord.ContainsKey(index))
            {
                int3 chunkCoord = IndexToChunkCoord[index];
                ChunkCoordToIndex.Remove(chunkCoord);
                IndexToChunkCoord.Remove(index);
                var enumerator = ChunkIDToCoord.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    bool3 boo = enumerator.Current.Value == chunkCoord;
                    if (boo.x == true && boo.y == true && boo.z == true)
                    {
                        ChunkIDToCoord.Remove(enumerator.Current.Key);
                    }
                }
                AllocatedCount--;
                MemClear(index);
            }

        }
        /// <summary>
        /// 清零
        /// </summary>
        /// <param name="index">分配的序号</param>
        public static void MemClear(int index)
        {

            ClearPartOfNativeArray(index);

        }


        public static unsafe void ClearPartOfNativeArray(int index)
        {
            // 确保 start 和 length 参数是有效的
            if (index < 0 || ChunkSize < 0 || index * ChunkSize + ChunkSize > BlocksData.Length)
            {
                return;
            }

            // 计算要清零的起始指针位置
            void* startPtr = (void*)((int*)NativeArrayUnsafeUtility.GetUnsafePtr(BlocksData) + index*ChunkSize);
            // 计算清零的字节长度
            long sizeInBytes = ChunkSize * sizeof(int);

            // 执行内存清零操作
            UnsafeUtility.MemClear(startPtr, sizeInBytes);
        }


        public static void getAllChunkInfo(int3 worldPosition, out int3 chunkCoord, out int chunkId, out int ChunkIndex)
        {
            chunkCoord =  ChunkDataHelper.GetChunkCoord(worldPosition);
            chunkId = getChunkID(worldPosition);
            ChunkIndex = getIndex(worldPosition);
        }
        public static void Dispose()
        {
            BlocksData.Dispose();
        }

        public static bool chunkFull()
        {
            return AllocatedCount >= TerrianConfig.MaxLoadedChunk;
        }
    }
}
