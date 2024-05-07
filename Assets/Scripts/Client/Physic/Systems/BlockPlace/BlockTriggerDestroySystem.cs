using MyCraftS.Bake.Baker;
using MyCraftS.Physic.SystemGroups;
using Unity.Collections;
using Unity.Entities;

namespace MyCraftS.Physic
{


    [UpdateInGroup(typeof(PhysicsAfterProcessSystemGroup))]
    public partial struct BlockTriggerDestroySystem : ISystem
    {

        public partial struct DestroyBlockTrigger:IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecbp;
            private void Execute([ChunkIndexInQuery] int sortKet,Entity entity)
            {
                ecbp.DestroyEntity(sortKet,entity);
            }
        }


        private EntityQuery _queryBlockTrigger;

        public void OnCreate(ref SystemState state)
        {
            _queryBlockTrigger = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockTriggerType>()
                .WithNone<BlockTriggerPrefabType>()
                .Build(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!BlockTriggerAddSystem.CanDestroy)
            {
                return;
            }
            BlockTriggerAddSystem.CanDestroy = false;
            if (_queryBlockTrigger.IsEmpty)
            {
                return;
            }
            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            state.Dependency = new DestroyBlockTrigger()
            {
                ecbp = entityCommandBuffer.AsParallelWriter()
            }.Schedule(_queryBlockTrigger, state.Dependency);
            state.Dependency.Complete();
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }
    }
}