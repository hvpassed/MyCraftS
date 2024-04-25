using Client.SystemManage;
using MyCraftS.Player.Data;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace MyCraftS.Player
{
    [UpdateInGroup(typeof(PlayerInitializeSystemGroup))]
    public partial class PlayerGravityStartSystem:SystemBase
    {

        
        protected override void OnCreate()
        {
            
            SystemManager.GameInitialedEvent+= StartSystem;
            this.Enabled = false;
        }

        public void StartSystem()
        {
            this.Enabled = true;
        }
        

        protected override void OnUpdate()
        {
            Debug.Log("gravity on");
            EntityManager.SetComponentData(PlayerDataContainer.playerEntity,
                new PhysicsGravityFactor()
                {
                    Value = 1.0f
                });
            this.Enabled = false;
        }
    }
}