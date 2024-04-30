using Client.SystemManage;
using MyCraftS.Chunk;
using MyCraftS.Input;
using MyCraftS.Time;
using Unity.Entities;

namespace MyCraftS.Action
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerInputSystemGroup))]
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