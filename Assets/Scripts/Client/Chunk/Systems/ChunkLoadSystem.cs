

using Unity.Entities;
using UnityEngine;

namespace MyCraftS.Chunk
{
 
    partial struct MyJob : IJobEntity
    {
        void Execute(ref ChunkCoord chunkData)
        {
           for(int i = 0; i < 16; i++)
            {
                for(int j = 0; j < 16; j++)
                {
                    for(int k = 0; k < 16; k++)
                    {
 
                    }
                }
            }
        }
    }
    /// <summary>
    /// 加载和保存Chunk
    /// </summary>
 
    [UpdateInGroup(typeof(ChunkSystemGroup))]
    public partial struct ChunkLoadSystem : ISystem
    {
        
       
        void OnCreate(ref SystemState state)
        {
 
        }
        void OnUpdate(ref SystemState state)
        {
            var query = state.GetEntityQuery(typeof(ChunkCoord));

            var jb = new MyJob() { };
            jb.ScheduleParallel(query);

        }
 

        private void UpdateQueue()
        {
            
        }
    }
}
