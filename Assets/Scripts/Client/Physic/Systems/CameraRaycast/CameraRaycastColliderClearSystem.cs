using MyCraftS.Bake;
using MyCraftS.Chunk.Data;
using MyCraftS.Data.IO;
using MyCraftS.Physic.SystemGroups;
using MyCraftS.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SocialPlatforms;

namespace MyCraftS.Physic
{
    [UpdateInGroup(typeof(PhysicsPreProcessSystemGroup))]
    [UpdateAfter(typeof(CameraRaycastColliderAddSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial class CameraRaycastColliderClearSystem:SystemBase
    {
        private EntityQuery _query;
        protected override void OnCreate()
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<RayColliderType>()
                .WithNone<RayColliderPrefabType>()
                .Build(this);
        }


        protected override void OnUpdate()
        {
            var entities = _query.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                LocalTransform localTransform = EntityManager.GetComponentData<LocalTransform>(entities[i]);
                if (shouldDestroy(ChunkDataContainer.getBlockid(new int3(localTransform.Position))))
                {
                    EntityManager.DestroyEntity(entities[i]);
                }
 
            }
        }

        private bool shouldDestroy(int id)
        {
            if (id == 0 || id == -1)
            {
                return true;
            }
            BlockInfo bi = BlockDataManager.BlockIDToInfoLookUp[id];
            if (bi.canRayCast==MyCraftsBoolean.True)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}