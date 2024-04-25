
using MyCraftS.Chunk.Data;
using MyCraftS.Chunk.Manage;
using MyCraftS.Player.Data;
using MyCraftS.Setting;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace MyCraftS.Chunk
{
    [UpdateInGroup(typeof(ChunkSystemGroup))]
    [UpdateAfter(typeof(ChunkGeneratorSystem))]
    public partial class ChunkQueueUpdateSystem : SystemBase
    {
        private readonly FixedString32Bytes SystemName = "Chunk Queue Update System";
        
        protected override void OnCreate()
        {
 
            base.OnCreate();
            

        }
        [BurstCompile]
        protected override void OnUpdate()
        {
            LocalTransform transform = EntityManager.GetComponentData<LocalTransform>(PlayerDataContainer.playerEntity);
            var position = transform.Position;
            int3 playerLocateChunk = ChunkDataHelper.GetChunkCoord(position);
            int viewDistance = SettingManager.PlayerSetting.ViewDistance;
            var chunkManageAspect = SystemAPI.GetAspect<ChunkManageDataAspect>(ChunkDataContainer.ChunkManager);



            for (int i = -viewDistance; i <= viewDistance; i++)
            {
                for (int j = -viewDistance; j <= viewDistance; j++)
                {
                    int3 newChunkPos = playerLocateChunk + new int3(i * 16, 0, j * 16);
                    if (!chunkManageAspect.chunkLoaded.ValueRO.LoadedSet.Contains(newChunkPos)&&
                        !chunkManageAspect.chunkNotLoaded.ValueRO.waitForLoaded.Contains(newChunkPos))
                    {
                        chunkManageAspect.chunkNotLoaded.ValueRW.waitForLoaded.Add(newChunkPos);
                        //Debug.Log($"{SystemName}: Add Chunk {newChunkPos} to Queue");
                    }
                }
            }

        }
    }
}
