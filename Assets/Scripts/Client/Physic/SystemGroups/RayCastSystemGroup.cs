using Client.SystemManage;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace MyCraftS.Physic.SystemGroups
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateBefore(typeof(AfterPhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial class RayCastSystemGroup:ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            SystemManager.GameInitialedEvent += StartSystem;
            this.Enabled = false;
        }
        
        public void StartSystem()
        {
            EntityQuery _physicsV = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PhysicsVelocity>()
                .Build(this);

            var entities = _physicsV.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                EntityManager.AddComponentData(entity, new IsGrounded());
                EntityManager.SetComponentEnabled(entity, typeof(IsGrounded), false);
            }
            
            this.Enabled = true;
        }
        
    }
}