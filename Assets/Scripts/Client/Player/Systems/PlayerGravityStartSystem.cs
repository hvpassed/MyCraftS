using Client.SystemManage;
using MyCraftS.Player.Data;
using MyCraftS.Setting;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.Player
{
    [UpdateInGroup(typeof(PlayerInitializeSystemGroup))]
    public partial class PlayerGravityStartSystem:SystemBase
    {

        private float recordGravity;
        protected override void OnCreate()
        {

            recordGravity = SettingManager.GameSetting.Gravity;
            SettingManager.GameSetting.Gravity = 0;
            SystemManager.GameInitialedEvent+= StartSystem;
            this.Enabled = false;
        }

        public void StartSystem()
        {
            
            this.Enabled = true;
        }
        

        protected override void OnUpdate()
        {
            
            SettingManager.GameSetting.Gravity = recordGravity;
            Debug.Log($"gravity on:{SettingManager.GameSetting.Gravity}m/s^2");
            // EntityManager.SetComponentData(PlayerDataContainer.playerEntity,
            //     new PhysicsGravityFactor()
            //     {
            //         Value = 0.0f
            //     });
            EntityManager.SetComponentData(PlayerDataContainer.playerEntity,LocalTransform.FromMatrix(
                float4x4.TRS(
                    new float3(0,250,0),
                    quaternion.identity,
                    new float3(1,1,1)
                    
                    )
                
                ));
            this.Enabled = false;
        }
    }
}