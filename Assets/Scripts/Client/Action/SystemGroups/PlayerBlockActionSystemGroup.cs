 
using MyCraftS.Chunk;
using MyCraftS.Input;
using MyCraftS.SystemManage;
using MyCraftS.Time;
using Unity.Entities;
using Unity.Physics.Systems;

namespace MyCraftS.Action
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    //[UpdateAfter(typeof(PlayerInputSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial class PlayerBlockActionSystemGroup:ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            SystemManager.GameInitialedEvent+=StartSystem;
            this.Enabled = false;
        }

        private void StartSystem()
        {
            this.Enabled = true;
        }
    }
}