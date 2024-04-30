 
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
        
        
        public void Execute(Entity entity,[EntityIndexInQuery] int index)
        {
            
            _ParallelWriter.DestroyEntity(index, entity);
        }
    }

    [UpdateInGroup(typeof(PhysicsAfterProcessSystemGroup),OrderFirst = true)]
    public partial struct BlockColliderAllDestroySystem:ISystem
    {
        public float deleteTime;
        public float passedTime;
        public EntityQuery _queryBlockCollider;
        public EntityQuery _queryRayCollider;
        private void OnCreate(ref SystemState state)
        {
            deleteTime = 1.0f;
            passedTime = 0;
            _queryBlockCollider = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockColliderType>()
                
                .WithNone<BlockColliderPrefabType>()
                .Build(ref state);
            _queryRayCollider = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<RayColliderType>()
                .WithNone<RayColliderPrefabType>()
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
            var ecbrc = new EntityCommandBuffer(Allocator.TempJob);
            state.Dependency = new DestroyAll()
            {
                _ParallelWriter = entityCommandBuffer.AsParallelWriter()
            }.Schedule(_queryBlockCollider,state.Dependency);
            state.Dependency.Complete();
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
            state.Dependency = new DestroyAll()
            {
                _ParallelWriter = ecbrc.AsParallelWriter()
            }.Schedule(_queryRayCollider,state.Dependency);
            state.Dependency.Complete();
            ecbrc.Playback(state.EntityManager);
            ecbrc.Dispose();
        }
    }
}