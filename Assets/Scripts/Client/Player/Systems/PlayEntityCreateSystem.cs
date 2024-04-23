using MyCraftS.Player.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MyCraftS.Player
{
    [UpdateInGroup(typeof(PlayerInitializeSystemGroup))]
    public partial class PlayEntityCreateSystem : SystemBase
    {
        public Entity playerEntity;
        protected override void OnCreate()
        {
            base.OnCreate();
            playerEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(playerEntity, LocalTransform.FromMatrix(float4x4.TRS(
                new float3(0,64,0),quaternion.identity,new float3(1,1,1))));

            PlayerDataContainer.playerEntity = playerEntity;
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EntityManager.DestroyEntity(playerEntity);
        }
    }
}
