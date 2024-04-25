using System;
using System.Drawing;
using MyCraftS.DeBug.SystemGroups;
using MyCraftS.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine.UIElements;

namespace MyCraftS.DeBug
{
    [UpdateInGroup(typeof(PerFrameUpdateDebugSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial class ClearedAABBDrawSystem:SystemBase
    {
        private EntityQuery _query;
        
        protected override void OnCreate()
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ClearedBlockCollider>()
                .Build(this);
        }


        protected override void OnUpdate()
        {
            var entities = _query.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var CleardBlockCollider = EntityManager.GetComponentData<ClearedBlockCollider>(entities[i]);
                DrawAABB.draw(CleardBlockCollider.aabb,UnityEngine.Color.black,0.1f);
                EntityManager.RemoveComponent<ClearedBlockCollider>(entities[i]);
                EntityManager.DestroyEntity(entities[i]);

            }
            
            
            
        }
    }
}