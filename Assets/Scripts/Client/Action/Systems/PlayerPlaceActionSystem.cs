using MyCraftS.Block.Utils;
using MyCraftS.Chunk.Data;
using MyCraftS.Data.IO;
using MyCraftS.Input;
using MyCraftS.Physic;
using MyCraftS.Player.Data;
using MyCraftS.Setting;
using MyCraftS.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace MyCraftS.Action
{
    [UpdateInGroup(typeof(PlayerBlockActionSystemGroup))]
    [UpdateAfter(typeof(PlayerDestroyActionSystem))]
    public partial class PlayerPlaceActionSystem:SystemBase
    {
        private EntityQuery _queryCanCreate;

 

        protected override void OnCreate()
        {

            _queryCanCreate = new EntityQueryBuilder(Allocator.Temp)
                 .WithAll<CanCreate>()
                 .WithAll<CanCreateInfo>()
                 .Build(this);
        }

        protected override void OnUpdate()
        {

            
            if (_queryCanCreate.IsEmpty)
            {
                return;
            }
 
            CanCreateInfo canCreateInfo= EntityManager.GetComponentData<CanCreateInfo>(BlockTriggerAddSystem.CanCreateEntity);
            EntityManager.SetComponentEnabled<CanCreate>(BlockTriggerAddSystem.CanCreateEntity,false);
            //Debug.Log($"player place action : {canCreateInfo.position}");
            int blockId = ChunkDataContainer.getBlockid(canCreateInfo.position);
            
            if (CheckPlaceAble(canCreateInfo.position))
            {
                int bi = canCreateInfo.blockId;
                BlockInfo blockInfo = BlockDataManager.BlockIDToInfoLookUp[bi];
                //一些处理
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData<WantCreate>(entity, new WantCreate()
                {
                    position = canCreateInfo.position,
                    blockId = bi,
                    canCollide = blockInfo.canCollide == MyCraftsBoolean.True,
                    hasDirection = blockInfo.hasDirect == MyCraftsBoolean.True
                });
            }
 
        }

        private int GetPlaceBlockID()
        {
            return 2;
        }
        private bool CheckPlaceAble(int3 placePos)
        {
            int blockId = ChunkDataContainer.getBlockid(placePos);
            if (blockId == -1)
            {
                return false;
            }else if (blockId == 0)
            {
                return true;
            }
 
            BlockInfo blockInfo = BlockDataManager.BlockIDToInfoLookUp[blockId];
            if (blockInfo.canCollide == MyCraftsBoolean.True)
            {
                return false;
            }
            else
            {
                BlockHelper.DestroyBlockAtPosition(placePos);
                return true;
            }
        }
    }
}