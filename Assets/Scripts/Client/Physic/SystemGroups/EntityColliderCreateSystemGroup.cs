using Client.SystemManage;
using MyCraftS.Time;
using Unity.Entities;
using Unity.Physics.Systems;

namespace MyCraftS.Physic.SystemGroups
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsPreProcessSystemGroup))]
    public partial class EntityColliderCreateSystemGroup:ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            SystemManager.GameInitialedEvent+= StartSystem;
            this.Enabled = false;
        }

        public void StartSystem()
        {
            this.Enabled = true;
        }
    }
}