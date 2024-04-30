using System;
using MyCraftS.Block;
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

    public partial struct DestroyEntity : IJobEntity
    {
        [ReadOnly] public int3 destroyPosition;
        public EntityCommandBuffer.ParallelWriter ecbp;
        
        private void Execute([ChunkIndexInQuery] int index, Entity entity,in LocalTransform transform)
        {
            float3 floatpos = math.floor(transform.Position);
            int3 intpos = new int3((int)floatpos.x, (int)floatpos.y, (int)floatpos.z);
            if (intpos.Equals(destroyPosition))
            {
                Debug.Log($"Called DestroyEntity : {intpos}");
                ecbp.DestroyEntity(index,entity);
            }
        }
    }
    
 
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(PlayerBlockActionGroup))]
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
            if (_queryAction.CalculateEntityCount() == 0 || _queryRayHit.CalculateEntityCount() == 0)
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
                        Debug.Log($"Destroying : at {position} ,cost : {destroyCost} ,destroyed : {destroyed}");
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
                    destroyPosition= new int3(-1, -1, -1);
                    destroyCost= 0;
                    destroyed = 0;
                    DestroyBlockFromPosition(position);
                     
                    break;
                }
            }
            _state = state;
        }

        private void DestroyBlockFromPosition(int3 position)
        {
            ChunkDataContainer.setBlockId(position,0);

            ChunkDataContainer.getAllChunkInfo(position,out int3 chunkCoord,out int chunkId,out int chunkIndex);

            BlockBelongToChunk filter = new BlockBelongToChunk()
            {
                ChunkCoord = chunkCoord,
                ChunkId = chunkId,
                ChunkBufferIndex = chunkIndex
            };
            _satisfyEntityQuery.SetSharedComponentFilter(filter);
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            this.Dependency = new DestroyEntity()
            {
                destroyPosition = position,
                ecbp = ecb.AsParallelWriter()
            }.ScheduleParallel(_satisfyEntityQuery, this.Dependency);
            this.Dependency.Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();
            
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