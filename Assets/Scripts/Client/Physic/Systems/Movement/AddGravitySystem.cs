using MyCraftS.Physic.SystemGroups;
using MyCraftS.Setting;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace MyCraftS.Physic
{
    [BurstCompile]
    public partial struct AddGravity:IJobEntity
    {
        [ReadOnly] public float gravity;
        public bool debug;
        public float deltaTime;
        public void Execute([ChunkIndexInQuery] int sortKey, ref PhysicsVelocity physicsVelocity)
        {
 
            physicsVelocity.Linear.y += gravity *deltaTime;
 
             
        }

    }

    
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(PhysicsPreProcessSystemGroup))]
    public partial struct AddGravitySystem:ISystem
    {
         
        
 


        private void OnUpdate(ref SystemState state)
        {
             
            state.Dependency = new AddGravity()
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                debug = SettingManager.DebugMode,
                gravity = SettingManager.GameSetting.Gravity
            }.ScheduleParallel(state.Dependency);
            
            state.Dependency.Complete();
        }
        
    }
}