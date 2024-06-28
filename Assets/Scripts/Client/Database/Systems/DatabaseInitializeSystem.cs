using MyCraftS.Database;
using Unity.Entities;

namespace MyCraftS.Initializer
{
    [UpdateInGroup(typeof(OtherInitializeSystemGroup))]
    public partial class DatabaseInitializeSystem : SystemBase
    {
        protected override void OnCreate()
        {
            DatabaseManager.DatabaseInitialize();
            this.Enabled = false;
        }
        
        
        protected override void OnUpdate()
        {
            
        }
    }
}