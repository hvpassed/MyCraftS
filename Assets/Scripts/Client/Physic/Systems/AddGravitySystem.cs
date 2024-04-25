using MyCraftS.Physic.SystemGroups;
using MyCraftS.Setting;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace MyCraftS.Physic
{
    [BurstCompile]
    public partial struct AddGravity:IJobEntity
    {
        [ReadOnly] public float gravity;
        public bool debug;
        public void Execute([ChunkIndexInQuery] int sortKey, ref PhysicsVelocity physicsVelocity)
        {
            if (debug)
            {

                //physicsVelocity.Linear.y += gravity;
            }
        }

    }

    
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(PhysicsPreProcessSystemGroup))]
    public partial struct AddGravitySystem:ISystem
    {
        
        
        
        private void OnCreate(ref SystemState state)
        {
                
        }
         
        
        private void OnUpdate(ref SystemState state)
        {
            state.Dependency = new AddGravity()
            {
                debug = SettingManager.DebugMode,
                gravity = SettingManager.GameSetting.Gravity
            }.ScheduleParallel(state.Dependency);
            
            state.Dependency.Complete();
        }
        
    }
}