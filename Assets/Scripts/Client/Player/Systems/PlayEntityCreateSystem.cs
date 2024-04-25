using MyCraftS.Player.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.SystemManage;
using MyCraftS.Physic;
using MyCraftS.Physic.Move;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.Player
{
    [UpdateInGroup(typeof(PlayerInitializeSystemGroup))]
    public partial class PlayEntityCreateSystem : SystemBase
    {
        public Entity playerEntity;
        public EntityQuery _PlayerQuery;
        protected override void OnCreate()
        {
            _PlayerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerType>().Build(this);

        }

        protected override void OnUpdate()
        {
            if (_PlayerQuery.CalculateEntityCount() != 0)
            {
                playerEntity = _PlayerQuery.ToEntityArray(Allocator.Temp)[0];
                PlayerDataContainer.playerEntity = playerEntity;
                var physicCollider = EntityManager.GetComponentData<PhysicsCollider>(playerEntity);
                var CollideFilter = new CollisionFilter()
                {
                    BelongsTo = CollisionGroups.CreatureGroup,
                    CollidesWith = CollisionGroups.BlockGroup | CollisionGroups.LiquidGroup,
                    
                };
                physicCollider.Value.Value.SetCollisionFilter(CollideFilter);
                EntityManager.SetComponentData(playerEntity, physicCollider);
                EntityManager.SetComponentData(playerEntity,LocalTransform.FromMatrix(
                    float4x4.TRS(
                        new float3(0,68,0),
                        quaternion.identity, 
                        new float3(1,1,1)
                        )
                ));
                var physicmess = EntityManager.GetComponentData<PhysicsMass>(playerEntity);
                //physicmess.InverseMass = 0;
                physicmess.InverseInertia=float3.zero;;
                EntityManager.SetComponentData<PhysicsMass>(playerEntity,physicmess);
                EntityManager.SetComponentData<PhysicsVelocity>(playerEntity,new PhysicsVelocity()
                {
                    Linear = float3.zero,
                    Angular = float3.zero
                });
                EntityManager.AddComponentData(playerEntity, new PhysicsGravityFactor()
                {
                    Value = 0.0f
                });
                SystemManager.CanStartSystem(ManagedSystem.TickSystemGroup);
                this.Enabled = false;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EntityManager.DestroyEntity(playerEntity);
        }
    }
}
