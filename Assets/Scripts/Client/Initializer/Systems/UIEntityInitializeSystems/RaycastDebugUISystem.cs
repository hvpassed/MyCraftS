using MyCraftS.UI.DebugUI;
using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Initializer.UI
{
    public partial class RaycastDebugUISystem:SystemBase
    {
        protected override void OnCreate()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, new MyCraftS.UI.DebugUI.RaycastInfoDebugUI()
            {
                blockPos = float3.zero,
                hitPos = float3.zero
            });
            
            RaycastInfo.RaycastInfoEntity = entity;
            
        }


        protected override void OnUpdate()
        {
            this.Enabled = false;
        }
    }
}