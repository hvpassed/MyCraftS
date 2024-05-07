using Client.SystemManage;
using MyCraftS.Action;
using MyCraftS.Bake.Baker;
using MyCraftS.Data.IO;
using MyCraftS.Input;
using MyCraftS.Physic.SystemGroups;
using MyCraftS.Player.Data;
using MyCraftS.Setting;
using MyCraftS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace MyCraftS.Physic
{
    [UpdateInGroup(typeof(PhysicsPreProcessSystemGroup))]
 
    public partial class BlockTriggerAddSystem : SystemBase
    {

        public static Entity CanCreateEntity;
        private Entity TriggerPrefab;
        private EntityQuery _placeActionQuery;
        private EntityQuery _rayCastQuery;
        private float deltaTime;
        public static bool CanDestroy = false;
        public static bool CheckOnce = false;
        protected override void OnCreate()
        {
            CanCreateEntity =  EntityManager.CreateEntity();
            EntityManager.AddComponentData(CanCreateEntity,new CanCreate());
            EntityManager.AddComponentData(CanCreateEntity,new CanCreateInfo()
            {
                blockId = 0,
                position = new int3(0,0,0)
            });
            deltaTime = 0;
            EntityManager.SetComponentEnabled<CanCreate>(CanCreateEntity,false);
            _placeActionQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlaceAction>()
                .Build(this);
            _rayCastQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAspect<CameraRayHitAspect>()
                .Build(this);

            SystemManager.GameInitialedEvent += StartSystem;
            this.Enabled = false;
        }
        private void StartSystem()
        {
            EntityQuery triggerprefab = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<BlockTriggerPrefabType>()
            .WithAll<BlockTriggerType>()
            .Build(this);
            TriggerPrefab = triggerprefab.GetSingletonEntity();
            this.Enabled = true;
        }
        protected override void OnUpdate()
        {
            deltaTime += SystemAPI.Time.DeltaTime;
            if (deltaTime < SettingManager.PlayerSetting.PlaceGapTime)
            {
                return;
            }
            deltaTime -= SettingManager.PlayerSetting.PlaceGapTime;

            if (_placeActionQuery.IsEmpty||_rayCastQuery.IsEmpty)
            {
 
                return;
            }
            int blockId = GetBlockId();
            var rayAspect = SystemAPI.GetAspect<CameraRayHitAspect>(PlayerDataContainer.cameraRayHitEntity);
            EntityManager.SetComponentEnabled<PlaceAction>(PlayerBlockInputProcessSystem.placeEntity, false);

            int3 placePos = rayAspect.GetPlacePos();
            BlockInfo blockInfo = BlockDataManager.BlockIDToInfoLookUp[blockId];
            if (blockInfo.canCollide==MyCraftsBoolean.True)
            {
                var newTrigger = EntityManager.Instantiate(TriggerPrefab);

                EntityManager.RemoveComponent<BlockTriggerPrefabType>(newTrigger);
                EntityManager.SetComponentData(newTrigger, LocalTransform.FromMatrix(
                    Matrix4x4.TRS(
                        new float3(placePos.x, placePos.y, placePos.z),
                        quaternion.identity,
                        new float3(1, 1, 1))));

                CanDestroy = false;
                CheckOnce = true;
                EntityManager.SetComponentData<CanCreateInfo>(CanCreateEntity, new CanCreateInfo()
                {
                    blockId = blockId,
                    position = placePos
                });
 
            }
            else
            {
                EntityManager.SetComponentData<CanCreateInfo>(CanCreateEntity, new CanCreateInfo()
                {
                    blockId = blockId,
                    position = placePos
                });
                EntityManager.SetComponentEnabled<CanCreate>(CanCreateEntity, true);
            }

        }



        public int GetBlockId()
        {
            return 9;
        }
    }
}
