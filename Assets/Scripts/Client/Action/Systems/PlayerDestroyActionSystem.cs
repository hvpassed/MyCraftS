using System;
using MyCraftS.Block;
using MyCraftS.Block.Utils;
using MyCraftS.Chunk;
using MyCraftS.Chunk.Data;
using MyCraftS.Data.IO;
using MyCraftS.Input;
using MyCraftS.Physic;
using MyCraftS.Player.Data;
using MyCraftS.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.Action
{
    enum DestroyActionState
    {
        None,
        Destroying,
        Destroyed
    }

    //public partial struct DestroyEntity : IJobEntity
    //{
    //    [ReadOnly] public int3 destroyPosition;
    //    public EntityCommandBuffer.ParallelWriter ecbp;

    //    private void Execute([ChunkIndexInQuery] int index, Entity entity, in LocalTransform transform)
    //    {
    //        float3 floatpos = math.floor(transform.Position);
    //        int3 intpos = new int3((int)floatpos.x, (int)floatpos.y, (int)floatpos.z);
    //        if (intpos.Equals(destroyPosition))
    //        {
    //            Debug.Log($"Called DestroyEntity : {intpos}");
    //            ecbp.AddComponent(index, entity, new BlockDestroyCleanUp()
    //            {
    //                position = destroyPosition
    //            });
    //            ecbp.DestroyEntity(index + 1, entity);
    //        }
    //    }
    //}


    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(PlayerBlockActionSystemGroup))]
    public partial class PlayerDestroyActionSystem:SystemBase
    {

        private EntityQuery _queryAction;
        private EntityQuery _queryRayHit;
        private EntityQuery _satisfyEntityQuery;
        private DestroyActionState _state = DestroyActionState.None;
        private int3 destroyPosition;
        private float destroyCost,destroyed;
        public float destroySpeed;
        
        protected override void OnCreate()
        {
            _queryAction = new EntityQueryBuilder(Allocator.Temp).WithAll<DestroyAction>().Build(this);
            _queryRayHit = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<CameraRayHitType>()
                .WithAll<CameraRayHitInfo>()
                .WithAll<CameraRayIsHit>()
                .Build(this);
            _satisfyEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockBelongToChunk>()
                .WithAll<BlockType>()
                .WithAll<LocalTransform>()
                .Build(this);
            RequireForUpdate(_queryAction);
            RequireForUpdate(_queryRayHit);
            destroySpeed = 50;
        }
        
        
        protected override void OnUpdate()
        {
            if (_queryAction.IsEmpty || _queryRayHit.IsEmpty)
            {
                SetState(DestroyActionState.None);
                return;
            }

            var rayHitInfo = EntityManager.GetComponentData<CameraRayHitInfo>(PlayerDataContainer.cameraRayHitEntity);
            SetState(DestroyActionState.Destroying, new int3((int)rayHitInfo.blockPosition.x, 
                (int)rayHitInfo.blockPosition.y, (int)rayHitInfo.blockPosition.z));

        }
        
        
        private void SetState(DestroyActionState state,int3 position = new int3())
        {
            switch (state)
            {
                case DestroyActionState.None:
                {
                    destroyPosition = new int3(-1, -1, -1);
                    destroyCost = 0;
                    destroyed = 0;
                    break;
                }
                case DestroyActionState.Destroying:
                {
                    bool3 positionChanged = position == destroyPosition;
                    if (positionChanged.x && positionChanged.y && positionChanged.z)
                    {
                        //Debug.Log($"Destroying : at {position} ,cost : {destroyCost} ,destroyed : {destroyed}");
                        destroyed += destroySpeed * SystemAPI.Time.DeltaTime;
                        if (destroyed >= destroyCost&&destroyCost!=0)
                        {
                            SetState(DestroyActionState.Destroyed, position);
                            return;
                        }
                    }
                    else
                    {
                        destroyPosition = position;
                        destroyCost = getCost(destroyPosition);
                        destroyed = 0;

                    }
                    break;
                }
                case DestroyActionState.Destroyed:
                {
                    Debug.Log($"Destroyed : at {position}");
                    BlockHelper.DestroyBlockAtPosition(destroyPosition);

                    destroyPosition = new int3(-1, -1, -1);
                    destroyCost= 0;
                    destroyed = 0;
                    //DestroyBlockFromPosition(position);
                    

                    break;
                }
            }
            _state = state;
        }


        
        

        private int getCost(int3 worldPosition)
        {
            int id = ChunkDataContainer.getBlockid(worldPosition);
            if (id == 0 || id == -1)
            {
                return 0;
            }
            BlockInfo bi = BlockDataManager.BlockIDToInfoLookUp[id];
            if (bi.canBeDestroyed == MyCraftsBoolean.False)
            {
                return 0;
            }
            else
            {
                return 100;
            }
        }
 
    }
}