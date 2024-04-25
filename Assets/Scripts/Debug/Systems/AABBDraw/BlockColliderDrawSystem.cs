using MyCraftS.Bake;
using MyCraftS.DeBug.SystemGroups;
using MyCraftS.Physic;
using MyCraftS.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.DeBug
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(PerFrameUpdateDebugSystemGroup))]
    public partial class BlockColliderDrawSystem:SystemBase
    {
        
        EntityQuery _entityQuery;
        
        protected override void OnCreate()
        {
            _entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockColliderType>()
                .WithNone<BlockColliderPrefabType>()
                .Build(this);
            
        }
        
        protected override void OnUpdate()
        {
            
            var entityArray = _entityQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entityArray.Length; i++)
            {
                var entity = entityArray[i];
                var blockCollider = EntityManager.GetComponentData<PhysicsCollider>(entity);
                var aabb = blockCollider.Value.Value.CalculateAabb();
                var transform = EntityManager.GetComponentData<LocalTransform>(entity);
                MoveAABB(ref aabb,transform.Position);
                DrawAABB.draw(aabb,Color.red);
            }
        }

        private void MoveAABB(ref Aabb aabb, float3 pos)
        {
            aabb.Max.x += pos.x;
            aabb.Max.y += pos.y;
            aabb.Max.z += pos.z;
            aabb.Min.x += pos.x;
            aabb.Min.y += pos.y;
            aabb.Min.z += pos.z;
        }
    }
}