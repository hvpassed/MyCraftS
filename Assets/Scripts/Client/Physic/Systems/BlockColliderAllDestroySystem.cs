 
using MyCraftS.Bake;
using MyCraftS.Physic.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MyCraftS.Physic
{

    public partial struct DestroyAll:IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter _ParallelWriter;
        
        
        public void Execute(Entity entity,[EntityIndexInQuery] int index, [ReadOnly] in BlockColliderType blockColliderType)
        {
            
            _ParallelWriter.DestroyEntity(index, entity);
        }
    }

    [UpdateInGroup(typeof(PhysicsAfterProcessSystemGroup),OrderFirst = true)]
    public partial struct BlockColliderAllDestroySystem:ISystem
    {
        public float deleteTime;
        public float passedTime;
        public EntityQuery _query;
        private void OnCreate(ref SystemState state)
        {
            deleteTime = 1.0f;
            passedTime = 0;
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockColliderType>()
                .WithNone<BlockColliderPrefabType>()
                .Build(ref state);
        }
        
        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            // if(passedTime< deleteTime)
            // {
            //     passedTime += SystemAPI.Time.DeltaTime;
            //     return;
            // }
            // else
            // {
            //     passedTime -= deleteTime;
            // }
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            
            state.Dependency = new DestroyAll()
            {
                _ParallelWriter = entityCommandBuffer.AsParallelWriter()
            }.Schedule(_query,state.Dependency);
            state.Dependency.Complete();
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }
    }
}