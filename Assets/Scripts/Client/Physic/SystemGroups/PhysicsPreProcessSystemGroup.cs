using Client.SystemManage;
using Unity.Entities;
using Unity.Physics.Systems;

namespace MyCraftS.Physic.SystemGroups
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial class PhysicsPreProcessSystemGroup:ComponentSystemGroup
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