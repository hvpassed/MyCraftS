using MyCraftS.Bake;
using MyCraftS.Chunk;
using MyCraftS.Chunk.Data;
using MyCraftS.Data.Component;
using MyCraftS.Data.IO;
using MyCraftS.DeBug;
using MyCraftS.Physic.SystemGroups;
using MyCraftS.Player.Data;
using MyCraftS.Setting;
using MyCraftS.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
 
using UnityEngine;
using Collider = Unity.Physics.Collider;

namespace MyCraftS.Physic
{
    [UpdateInGroup(typeof(EntityColliderCreateSystemGroup))]
    [UpdateAfter(typeof(BlockColliderAddSystem))]
    public partial class BlockColliderClearSystem:SystemBase
    {
        private EntityQuery _query;
        private EntityQuery _query2;
        protected override void OnCreate()
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockColliderType>()
                .WithNone<BlockColliderPrefabType>()
                .WithNone<RayColliderType>()
                .Build(this);
            _query2 = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockColliderPrefabType>()
                .Build(this);
        }


        protected override void OnUpdate()
        {
 
            

            
                // 创建球形碰撞体描述符
 


            
            
            
            var entities = _query.ToEntityArray(Allocator.Temp);


            for (int i = 0; i < entities.Length; i++)
            {
                var worldCoord = EntityManager.GetComponentData<LocalTransform>(entities[i]);
                int id = ChunkDataContainer.getBlockid(new int3((int)worldCoord.Position.x,
                    (int)worldCoord.Position.y,(int)worldCoord.Position.z));
                if(id == -1 || id == 0||BlockDataManager.BlockIDToInfoLookUp[id].canCollide == MyCraftsBoolean.False)
                {
                    if (SettingManager.DebugMode)
                    {
                        var aabb = EntityManager.GetComponentData<PhysicsCollider>(entities[i]).Value.Value.CalculateAabb();
                        var transform = EntityManager.GetComponentData<LocalTransform>(entities[i]);
                        DrawAABB.MoveAABB(ref aabb,transform.Position);
                        EntityManager.AddComponentData<ClearedBlockCollider>(entities[i],new ClearedBlockCollider()
                        {
                            aabb = aabb
                        });
                    }
 
                    EntityManager.DestroyEntity(entities[i]);
                 
                }

            }
            
        }
    }
}