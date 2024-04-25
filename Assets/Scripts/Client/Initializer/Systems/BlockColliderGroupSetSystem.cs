using MyCraftS.Bake;
using MyCraftS.Physic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace MyCraftS.Initializer
{
    [UpdateInGroup(typeof(PhysicInitializeSystemGroup))]
    public partial class BlockColliderGroupSetSystem:SystemBase
    {
        private EntityQuery _query;
        protected override void OnCreate()
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockColliderPrefabType>()
                .Build(this);
        }
        
        
        
        protected override void OnUpdate()
        {
            bool executed = false;
            var entities = _query.ToEntityArray(Allocator.Temp);
            var CollisionFilter = new CollisionFilter()
            {
                BelongsTo = CollisionGroups.BlockGroup,
                CollidesWith = CollisionGroups.CreatureGroup
            };

            foreach (var entity in entities)
            {
                var physicsCollider = EntityManager.GetComponentData<PhysicsCollider>(entity);
                physicsCollider.Value.Value.SetCollisionFilter(CollisionFilter);
                EntityManager.SetComponentData(entity, physicsCollider);
                executed = true;
            }

            if (executed)
            {
                this.Enabled = false;
            }
            
        }
    }
}