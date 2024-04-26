 
using Client.SystemManage;
using Unity.Entities;

namespace MyCraftS.DeBug.SystemGroups
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PerFrameUpdateDebugSystemGroup:ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
             SystemManager.DebugSystemStartEvent += StartSystem;
             SystemManager.DebugSystemCloseEvent += CloseSystem;
             this.Enabled = false;
        }


        public void StartSystem()
        {
            this.Enabled = true;
        }
        
        public void CloseSystem()
        {
            this.Enabled = false;
        }
    }
}