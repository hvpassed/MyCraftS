using MyCraftS.Block;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace  MyCraftS.Bake.Baker
{
    public struct BlockEntityPrefab:IComponentData
    {
        public EntityPrefabReference Prefab;
    }
    public class BlockPrefabAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public int id;
    }


#if UNITY_EDITOR
    public class BlockPrefabBaker: Baker<BlockPrefabAuthoring>
    {
        public override void Bake(BlockPrefabAuthoring authoring)
        {
            //Debug.Log($"Bake Block Prefab{authoring.prefab.name}");

            
            var entityPrefab = new EntityPrefabReference(authoring.prefab);
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new BlockEntityPrefab() {Prefab = entityPrefab});
            AddComponent<BlockID>(entity,new BlockID(){Id = authoring.id});
            AddComponent<BlockShouldUpdate>(entity, new BlockShouldUpdate());
            SetComponentEnabled<BlockShouldUpdate>(entity,false);

        }
    }    
#endif
}