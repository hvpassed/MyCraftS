using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using BoxCollider = Unity.Physics.BoxCollider;
using Collider = Unity.Physics.Collider;

namespace  MyCraftS.Bake
{
    
    public struct RayColliderPrefabType : IComponentData
    {
        
    }
    public class RaycastColliderAuthoring : MonoBehaviour
    {
 
    }
    
 
    public class RaycastColliderBaker : Baker<RaycastColliderAuthoring>
    {
        public override void Bake(RaycastColliderAuthoring authoring)
        {
             
            var blockColliderEntity = GetEntity(TransformUsageFlags.Dynamic);
            // 创建立方体碰撞体描述符
            BoxGeometry boxGeometry = new BoxGeometry
            {
                Center = new float3(0.5f, 0.5f, 0.5f),
                Size = new float3(1, 1, 1),
                Orientation = quaternion.identity,
                BevelRadius = 0f
            };

            // 创建碰撞体
            BlobAssetReference<Collider> collider =  BoxCollider.Create(boxGeometry);
            collider.Value.SetCollisionResponse(CollisionResponsePolicy.None);
            AddComponent<PhysicsCollider>(blockColliderEntity, new PhysicsCollider
            {
                Value = collider
            });
            AddSharedComponent<PhysicsWorldIndex>(blockColliderEntity, new PhysicsWorldIndex()
            {
                Value = 0
            });
            AddComponent(blockColliderEntity, new MyCraftS.Physic.RayColliderType());
            AddComponent(blockColliderEntity, new RayColliderPrefabType());
        }
    }

}