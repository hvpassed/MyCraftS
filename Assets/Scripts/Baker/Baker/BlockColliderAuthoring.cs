 
using UnityEngine;
using Unity.Entities;
using Unity.Physics;

namespace MyCraftS.Bake
{
    public struct BlockColliderPrefabType : IComponentData
    {
        
    }
    public class BlockColliderAuthoring : MonoBehaviour
    {
        
    }
    
    public class BlockColliderBaker : Baker<BlockColliderAuthoring>
    {
        public override void Bake(BlockColliderAuthoring authoring)
        {
            
            var blockColliderEntity = GetEntity(TransformUsageFlags.Renderable);
             
            
            AddComponent(blockColliderEntity, new MyCraftS.Physic.BlockColliderType());
            AddComponent(blockColliderEntity, new BlockColliderPrefabType());
        }
    }
}