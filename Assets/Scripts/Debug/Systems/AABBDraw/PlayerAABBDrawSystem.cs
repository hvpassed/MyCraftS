using MyCraftS.DeBug.SystemGroups;
using MyCraftS.Player;
using MyCraftS.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.DeBug
{
    [UpdateInGroup(typeof(PerFrameUpdateDebugSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial class PlayerAABBDrawSystem:SystemBase
    {
        EntityQuery m_PlayerQuery;
        Entity playerEntity;
        protected override void OnCreate()
        {
            m_PlayerQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerType>().Build(this);
        }
        
        protected override void OnUpdate()
        {
            if (playerEntity == Entity.Null)
            {
                playerEntity = m_PlayerQuery.ToEntityArray(Allocator.Temp)[0];
            }
            var PhysicsCollider = EntityManager.GetComponentData<PhysicsCollider>(playerEntity);
            var aabb = PhysicsCollider.Value.Value.CalculateAabb();
            var trans = EntityManager.GetComponentData<LocalTransform>(playerEntity);
            DrawAABB.MoveAABB(ref aabb,trans.Position);
            DrawAABB.draw(aabb,Color.blue);


        }
    }
}