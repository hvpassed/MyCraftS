using MyCraftS.Physic.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace MyCraftS.Physic
{
    partial struct CleanAngular:IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter _ParallelWriter;
        public void Execute([ChunkIndexInQuery] int sortKey,Entity entity,ref PhysicsVelocity velocity)
        {
            _ParallelWriter.SetComponent(sortKey,entity,new PhysicsVelocity()
            {
                Angular = float3.zero,
                Linear = velocity.Linear
            });
        }
    }
    [UpdateInGroup(typeof(PhysicsPreProcessSystemGroup))]
    public partial struct CleanAngularSystem:ISystem
    {
        
        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            state.Dependency = new CleanAngular()
            {
                _ParallelWriter = entityCommandBuffer.AsParallelWriter()
            }.ScheduleParallel(state.Dependency);
            
            state.Dependency.Complete();
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
            
        }
        
    }
}