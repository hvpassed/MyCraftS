using MyCraftS.Initializer.UI;
using MyCraftS.Input;
using MyCraftS.Physic.SystemGroups;
using MyCraftS.Player.Data;
using MyCraftS.Setting;
using MyCraftS.UI.DebugUI;
using MyCraftS.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace MyCraftS.Physic
{
    [UpdateInGroup(typeof(RayCastSystemGroup))]
    //[RequireMatchingQueriesForUpdate]
    public partial class CameraRaycastCollideCheck:SystemBase
    {
        private CollisionFilter _filter;
        private EntityQuery singletonQuery;
        private ComponentType _camIsHitType;
        protected override void OnCreate()
        {
            _camIsHitType = typeof(CameraRayIsHit);
            
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();

            singletonQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(builder);
            _filter = new CollisionFilter()
            {
                BelongsTo = CollisionGroups.RaycastGroup,
                CollidesWith = CollisionGroups.RaycastGroup|CollisionGroups.CreatureGroup|CollisionGroups.BlockGroup,
                GroupIndex = CollisionGroups.RaycastGroupIndex
            };
        }

        protected override void OnUpdate()
        {
            var collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            LocalTransform playerPos = EntityManager.GetComponentData<LocalTransform>(PlayerDataContainer.playerEntity);
            float3 camPos = playerPos.Position + SettingManager.PlayerSetting.CameraOffset;
            CameraForward cameraForward =
                EntityManager.GetComponentData<CameraForward>(PlayerDataContainer.cameraEntity);
            RaycastInput raycastInput = new RaycastInput()
            {
                Start = camPos,
                End = camPos + cameraForward.direction * SettingManager.PlayerSetting.rayDistance,
                Filter = _filter
            };
            // Debug.DrawLine(camPos, camPos + cameraForward.direction * SettingManager.PlayerSetting.rayDistance,
            //     Color.red,0.2f);
            //Debug.Log($"Campos : {camPos}, EndPos : {camPos + cameraForward.direction * SettingManager.PlayerSetting.rayDistance}");
            if (collisionWorld.CastRay(raycastInput, out RaycastHit hit))
            {
                LocalTransform localTransform = EntityManager.GetComponentData<LocalTransform>(hit.Entity);
                // //方块
                // if (EntityManager.HasComponent<RayColliderType>(hit.Entity))
                // {
                    EntityManager.SetComponentEnabled(PlayerDataContainer.cameraRayHitEntity,_camIsHitType,true);
                    var cameraHitInfo =
                        EntityManager.GetComponentData<CameraRayHitInfo>(PlayerDataContainer.cameraRayHitEntity);
                    HitSide side = RayHitHelper.CheckHitSide(hit.Position, localTransform.Position);
                    EntityManager.SetComponentData<RaycastInfoDebugUI>(RaycastInfo.RaycastInfoEntity,new RaycastInfoDebugUI()
                    {
                        hitPos = hit.Position,
                        blockPos = localTransform.Position,
                        hitSide = side
                    });
                    cameraHitInfo.hitSide = side;
                    cameraHitInfo.blockPosition = localTransform.Position;
                    EntityManager.SetComponentData(PlayerDataContainer.cameraRayHitEntity,cameraHitInfo);
                    //Debug.Log($"{side}");
                    
                    Aabb aabb = new Aabb();
                    Aabb aabb2 = new Aabb();
                    aabb2.Min = math.floor(localTransform.Position);
                    aabb2.Max = aabb2.Min + 1;
                    aabb.Min = (math.floor(hit.Position));
                    aabb.Max = aabb.Min + 1;
                    DrawAABB.draw(aabb,Color.grey,0.1f);
                    DrawAABB.draw(aabb2,Color.black,0.1f);
                // }
                // else
                // {
                //     EntityManager.SetComponentEnabled(PlayerDataContainer.cameraRayHitEntity,_camIsHitType,false);
                // }

            }
            else
            {
                EntityManager.SetComponentEnabled(PlayerDataContainer.cameraRayHitEntity,_camIsHitType,false);
            }


        }
    }
}