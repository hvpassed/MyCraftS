
using MyCraftS.Chunk.Data;
using MyCraftS.Chunk.Manage;
using MyCraftS.Config;
using MyCraftS.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
namespace MyCraftS.Chunk
{
    [BurstCompile]
    public partial struct GenerateChunk : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<int> blockinfo;
        
        [ReadOnly]
        public NativeArray<int> heightMap;
        public void Execute(int index)
        {
            int h = index * 16;
            for(int y = h; y < h + 16&&y<TerrianConfig.MaxHeight; y++)
            {
                for(int x = 0; x < TerrianConfig.ChunkSize; x++)
                {
                    for(int z = 0; z < TerrianConfig.ChunkSize; z++)
                    {
                        if(y>heightMap[ChunkDataHelper.IndexGetterXZ(x,z)])
                        {
                            continue;
                        }
                        int ind = ChunkDataHelper.IndexGetter(x, y, z);
                        blockinfo[ind] = 1;
                    }
                }
            }
            
        }
    }

 



    [UpdateInGroup(typeof(ChunkSystemGroup))]
    [UpdateAfter(typeof(ChunkLoadSystem))]
    public partial struct ChunkGeneratorSystem:ISystem
    {
        static readonly ProfilerMarker chunkGeneratorSystemProfilerMarker = 
            new ProfilerMarker("ChunkGeneratorSystem");
        private FixedString32Bytes SystemName;
 
        private int3 chunkLoading;
        private int _isGenerating;
        private int isFirst;
        Entity chunkManager;
        private JobHandle jb;
        private GenerateChunk generateChunk;
        private DynamicBuffer<ChunkHeightMap> chunkHeightMap;
        Entity entity;
        private EntityQuery _query;
        private int count;
        private void OnCreate(ref SystemState state)
        {
 
            _isGenerating = 0;
            isFirst = 1;
            SystemName = "Chunk Generator System";
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ChunkLoaded>()
                .WithAll<ChunkNotLoaded>()
                .Build(ref state);
            count = 0;
        }
        
        private void OnUpdate(ref SystemState state)
        {
            chunkGeneratorSystemProfilerMarker.Begin();
            if (chunkManager.Equals(Entity.Null))
            {

                chunkManager = _query.GetSingletonEntity();
                if (chunkManager.Equals(Entity.Null))
                {
                    Debug.LogWarning("ChunkManager is null");
                }
            }

            if (CheckJobCompletion(ref state))
            {
                TryStartNewJobs(ref state);
            }
            
            chunkGeneratorSystemProfilerMarker.End();
        }

        private bool CheckJobCompletion(ref SystemState state)
        {
            if (_isGenerating==0) return true; // No job running

            if (!jb.IsCompleted) return false; // Job still running

            jb.Complete(); // Complete the job and proceed
            ProcessCompletedJob(ref state);
            return true; // Job completed
        }

        private void ProcessCompletedJob(ref SystemState state)
        {
            count++;
            //Debug.Log($"Have processd:{count}");
            int ind = ChunkDataContainer.Allocate(chunkLoading,out int chunkid);
            
            NativeSlice<int> chunkBlocksData = ChunkDataContainer.Slice(ind);
            if (checkSafety(generateChunk.blockinfo, chunkBlocksData))
            {
                chunkBlocksData.CopyFrom(generateChunk.blockinfo);
            }
            state.EntityManager.AddComponentData(entity, new ChunkBlocks()
            {
                bufferIndex = ind,
                blocks = chunkBlocksData
            });
            state.EntityManager.AddComponentData(entity,new ChunkInitializeRenderTag());
            state.EntityManager.AddComponentData(entity, new ChunkID()
            {
                id = chunkid
            });
            generateChunk.blockinfo.Dispose();
            UpdateChunkManagementState(ref state);
            Debug.Log($"Chunk Generate System: Chunk @id:{chunkid} @bufferIndex:{ind} @ChunkCoord:{chunkLoading} Generated!");
        }

        private bool checkSafety(NativeArray<int> source,NativeSlice<int> des)
        {
            if(source.Length == des.Length 
                &&source.Length==TerrianConfig.ChunkSize*TerrianConfig.ChunkSize*TerrianConfig.MaxHeight)
                return true;
            else
            {
                return false;
            }
        }
        private void UpdateChunkManagementState(ref SystemState state)
        {
            var chunkManageAspect = SystemAPI.GetAspect<ChunkManageDataAspect>(chunkManager);
            //Debug.Log($"{SystemName}: Loaded {chunkLoading}");
            chunkManageAspect.chunkNotLoaded.ValueRW.waitForLoaded.Remove(chunkLoading);
            chunkManageAspect.chunkLoaded.ValueRW.LoadedSet.Add(chunkLoading);
            _isGenerating = 0;
        }

        private void TryStartNewJobs(ref SystemState state)
        {
            if (chunkManager.Equals(Entity.Null))
            {
                Debug.LogWarning("ChunkManager is null");
            }
            var chunkManageAspect = SystemAPI.GetAspect<ChunkManageDataAspect>(chunkManager);
            while (chunkManageAspect.chunkNotLoaded.ValueRO.waitForLoaded.Count > 0 &&!ChunkDataContainer.chunkFull())
            {
                if (TryLoadChunk(ref state,chunkManageAspect)) break;
            }
        }

        private bool TryLoadChunk(ref SystemState state,ChunkManageDataAspect chunkManageAspect)
        {
            var enumerator = chunkManageAspect.chunkNotLoaded.ValueRW.waitForLoaded.GetEnumerator();
            if (!enumerator.MoveNext()) return false;

            chunkLoading = enumerator.Current;
            if (chunkManageAspect.chunkLoaded.ValueRO.LoadedSet.Contains(chunkLoading))
            {
                chunkManageAspect.chunkNotLoaded.ValueRW.waitForLoaded.Remove(chunkLoading);
                return false;
            }

            StartGeneratingChunk(ref state);
            return true;
        }

        private void StartGeneratingChunk(ref SystemState state)
        {
            _isGenerating = 1;
            //Debug.Log($"{SystemName}: Loading {chunkLoading}");
            

            entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, new ChunkCoord()
            {
                chunkCoord = chunkLoading
            });

            state.EntityManager.AddComponentData(entity, new ChunkType());
 
            chunkHeightMap = state.EntityManager.AddBuffer<ChunkHeightMap>(entity);
            chunkHeightMap.Resize(TerrianConfig.ChunkSize * TerrianConfig.ChunkSize,NativeArrayOptions.ClearMemory);
            NativeArray<int> hm = new NativeArray<int>(TerrianConfig.ChunkSize * TerrianConfig.ChunkSize, Allocator.Persistent);
            for (int i = 0; i < TerrianConfig.ChunkSize; i++)
            {
                for(int j = 0; j < TerrianConfig.ChunkSize; j++)
                {
                    int height = getHeight(i,j);
                    int id = ChunkDataHelper.IndexGetterXZ(i, j);
                    hm[id] = height;
                    chunkHeightMap[id] = new ChunkHeightMap()
                    {
                        height = height
                        
                    };

                }
            }
            
            generateChunk = new GenerateChunk()
            {
                blockinfo = new NativeArray<int>(TerrianConfig.ChunkSize * TerrianConfig.ChunkSize 
                                                                         * TerrianConfig.MaxHeight, Allocator.Persistent),
                heightMap = hm
            };

            jb = generateChunk.Schedule(TerrianConfig.MaxHeight/16+1,1);
        }



        private int getHeight(int x,int z  )
        {
            float noise = PerlinNoise2D.Generate((float)x+chunkLoading.x, (float)z+chunkLoading.z, 1 / 64f, 2f);
            return (int)(64 + 10*(noise/0.36-1));
        }








        //private void OnUpdate(ref SystemState state)
        //{
        //    int pass = 0;

            //    if (isFirst != 1)//不是第一次
            //    {
            //        if (isGenerating ==1 )//如果有工作,工作完成
            //        {
            //            if (jb.IsCompleted)
            //            {
            //                jb.Complete();
            //                var entity = state.EntityManager.CreateEntity();
            //                state.EntityManager.AddComponentData(entity, new ChunkCoord() {
            //                    chunkCoord = chunkLoading,
            //                    blocks = new NativeArray<int>(generateChunk.blockinfo,Allocator.Persistent)
            //                });
            //                state.EntityManager.AddComponentData(entity, new ChunkID()
            //                {
            //                    id = chunkIdCount
            //                });
            //                chunkIdCount++;
            //                generateChunk.blockinfo.Dispose();
            //                var chunkManageAspect = SystemAPI.GetAspect<ChunkManageDataAspect>(ChunkDataContainer.ChunkManager);
            //                if (isGenerating == 1)
            //                {
            //                    Debug.Log($"{SystemName}: Loaded {chunkLoading}");
            //                    chunkManageAspect.chunkNotLoaded.ValueRW.waitForLoaded.Remove(chunkLoading);
            //                    chunkManageAspect.chunkLoaded.ValueRW.LoadedSet.Add(chunkLoading);
            //                    isGenerating = 0;
            //                }



            //            }
            //            else
            //            {
            //                pass = 1;
            //            }
            //        }

            //    }
            //    else
            //    {

            //    }
            //    if (pass == 0)
            //    {

            //        var chunkManageAspect = SystemAPI.GetAspect<ChunkManageDataAspect>(ChunkDataContainer.ChunkManager);
            //        int get = 0;
            //        while (get == 0 && chunkManageAspect.chunkNotLoaded.ValueRO.waitForLoaded.Count > 0)
            //        {
            //            var enumerator = chunkManageAspect.chunkNotLoaded.ValueRW.waitForLoaded.GetEnumerator();
            //            enumerator.MoveNext();

            //            chunkLoading = enumerator.Current;

            //            if (!chunkManageAspect.chunkLoaded.ValueRO.LoadedSet.Contains(chunkLoading))
            //            {


            //                isGenerating = 1;
            //                Debug.Log($"{SystemName}: Loading {chunkLoading}");
            //                get = 1;
            //                isFirst = 0;
            //                //分配任务

            //                generateChunk = new GenerateChunk() {
            //                    blockinfo = new NativeArray<int>(TerrianConfig.ChunkSize * TerrianConfig.ChunkSize * TerrianConfig.MaxHeight, Allocator.Persistent)
            //                };
            //                jb = generateChunk.Schedule(TerrianConfig.MaxHeight/16,1);

            //            }
            //            else
            //            {
            //                chunkManageAspect.chunkNotLoaded.ValueRW.waitForLoaded.Remove(chunkLoading);
            //            }


            //        }

            //    }

            //}
    }
    }
