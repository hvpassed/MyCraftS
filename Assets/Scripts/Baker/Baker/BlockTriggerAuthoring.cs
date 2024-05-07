using MyCraftS.Physic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using BoxCollider = Unity.Physics.BoxCollider;
using Collider = Unity.Physics.Collider;
using Material = Unity.Physics.Material;


namespace  MyCraftS.Bake.Baker
{
    
    public struct BlockTriggerPrefabType : IComponentData
    {
        
    }
    
    
    public class BlockTriggerAuthoring:MonoBehaviour
    {
        
        
        
    }



    public class BlockTriggerBaker : Baker<BlockTriggerAuthoring>
    {
        public override void Bake(BlockTriggerAuthoring authoring)
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
            CollisionFilter collisionFilter = new CollisionFilter()
            {
                BelongsTo = CollisionGroups.BlockGroup,
                CollidesWith = CollisionGroups.CreatureGroup

            };
            // 创建碰撞体
            BlobAssetReference<Collider> collider =  BoxCollider.Create(boxGeometry,collisionFilter);
            collider.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
            AddComponent<PhysicsCollider>(blockColliderEntity, new PhysicsCollider
            {
                Value = collider
            });
            AddSharedComponent<PhysicsWorldIndex>(blockColliderEntity, new PhysicsWorldIndex()
            {
                Value = 0
            });
            AddComponent(blockColliderEntity, new MyCraftS.Physic.BlockTriggerType());
            AddComponent(blockColliderEntity, new BlockTriggerPrefabType());
        }
    }
}