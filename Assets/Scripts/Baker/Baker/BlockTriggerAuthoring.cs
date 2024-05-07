
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace MyCraftS.Bake.Baker
{
    public class BlockTriggerAuthoring : MonoBehaviour
    {
        // Start is called before the first frame update

    }

    public struct BlockTriggerPrefabType : IComponentData
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

            // 创建碰撞体
            BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.BoxCollider.Create(boxGeometry);
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
