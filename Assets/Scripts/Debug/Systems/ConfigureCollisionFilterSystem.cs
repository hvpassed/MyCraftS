using Client.SystemManage;
using MyCraftS.Bake;
using MyCraftS.Chunk.Data;
using MyCraftS.Config;
using MyCraftS.Player;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace MyCraftS.DeBug
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ConfigureCollisionFilterSystem:SystemBase
    {
        EntityQuery _playerQuery;
        EntityQuery _blockColliderPrefabQuery;
        protected override void OnCreate()
        {
            SystemManager.DebugSystemStartOnceEvent+= StartSystem;
            _playerQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerType>().Build(this);
            _blockColliderPrefabQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockColliderPrefabType>()
                .Build(this);
            this.Enabled = false;
            
        }


        protected override void OnUpdate()
        {
            bool canCollide = true;
            var player = _playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerCollider = EntityManager.GetComponentData<PhysicsCollider>(player);
            
            var blockColliderEntities = _blockColliderPrefabQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in blockColliderEntities)
            {
                var entityCollider = EntityManager.GetComponentData<PhysicsCollider>(entity);
                if (!CollisionFilter.IsCollisionEnabled(entityCollider.Value.Value.GetCollisionFilter(),
                        playerCollider.Value.Value.GetCollisionFilter()))
                {
                    canCollide = false;
                }


            }

            if (canCollide)
            {
                Debug.Log("Player can collide with block");
            }
            else
            {
                Debug.Log("Player can't collide with block");
            }
            
            this.Enabled = false;
        }
        
        public void StartSystem(string pressKey)
        {
            if (pressKey == "ConfigureCollision")
            {
                this.Enabled = true;
            }
        }
    }
}