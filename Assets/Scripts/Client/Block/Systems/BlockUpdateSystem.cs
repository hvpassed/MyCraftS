using MyCraftS.Block.Utils;
using MyCraftS.Data.IO;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.Block.Update
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(BlockUpdateSystemGroup),OrderLast = true)]
    public partial class BlockUpdateSystem:SystemBase
    {
        
        private EntityQuery _shouldUpdateQuery;
        protected override void OnCreate()
        {
            _shouldUpdateQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockShouldUpdate>()
                .WithNone<BlockPrefabType>()
                .Build(this);
            //RequireForUpdate(_shouldUpdateQuery);
        }

        protected override void OnUpdate()
        {
            if (!_shouldUpdateQuery.IsEmpty)
            {
                var Entities = _shouldUpdateQuery.ToEntityArray(Allocator.TempJob);
                foreach (Entity entity in Entities)
                {
                    LocalTransform transform = EntityManager.GetComponentData<LocalTransform>(entity);
                    if (BlockHelper.IsBlocked(transform.Position))
                    {
                        if (!EntityManager.HasComponent<DisableRendering>(entity))
                        {
                            EntityManager.AddComponent<DisableRendering>(entity);
                        }
                    }
                    else
                    {
                        if (EntityManager.HasComponent<DisableRendering>(entity))
                        {
                            EntityManager.RemoveComponent<DisableRendering>(entity);
                        }
                    }

                    EntityManager.SetComponentEnabled<BlockShouldUpdate>(entity,false);

                }
                
                
                
                
            }
            
            
        }
    }
}