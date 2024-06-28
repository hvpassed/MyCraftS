 
using MyCraftS.Action;
using MyCraftS.SystemManage;
using Unity.Entities;

namespace MyCraftS.Block.Update
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerBlockActionSystemGroup))]
    public partial class BlockUpdateSystemGroup:ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            SystemManager.GameInitialedEvent += StartSystem;
            this.Enabled= false;
        }
        
        private void StartSystem()
        {
            this.Enabled = true;
        }
    }
}