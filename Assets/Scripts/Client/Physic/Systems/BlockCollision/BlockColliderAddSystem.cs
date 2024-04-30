

using MyCraftS.Bake;
using MyCraftS.Data.Component;
 
using MyCraftS.Physic.SystemGroups;
using MyCraftS.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using BoxCollider = Unity.Physics.BoxCollider;
using Collider = Unity.Physics.Collider;
using SphereCollider = Unity.Physics.SphereCollider;

namespace MyCraftS.Physic
{
    [BurstCompile]
    public partial struct CreateBlockCollider : IJobEntity
    {
        [Unity.Collections.ReadOnly]
        public float deltaTime;
    
 
        public Entity ColliderEntity;
    
        public EntityCommandBuffer.ParallelWriter _ParallelWriter;
        public void Execute([ChunkIndexInQuery] int sortKey,in LocalTransform transform,in PhysicsVelocity velocity,in PhysicsCollider physicsCollider)
        {
            var aabb = physicsCollider.Value.Value.CalculateAabb();
            moveAabb(ref aabb,transform.Position);
            var expandAabb = ExpandAabb(aabb,velocity.Linear);
            
            for (int x = (int)expandAabb.Min.x-1; x <= (int)expandAabb.Max.x; x++)
            {
                for (int y = (int)expandAabb.Min.y-1; y <=(int)expandAabb.Max.y; y++)
                {
                    for (int z = (int)expandAabb.Min.z-1; z <= (int)expandAabb.Max.z; z++)
                    {
                        
                        var entity = _ParallelWriter.Instantiate(sortKey, ColliderEntity);
                        
                        
                        _ParallelWriter.RemoveComponent<BlockColliderPrefabType>(sortKey,entity);
                        
                        // BlobAssetReference<Collider> boxCollider = BoxCollider.Create(new BoxGeometry()
                        // {
                        //     Center = new float3(0,0,0),
                        //     Size = new float3(1,1,1),
                        //     Orientation = quaternion.identity,
                        // }, _BlockCollider.Value.GetCollisionFilter());
                        //
                        //
                        // _ParallelWriter.SetComponent(sortKey,entity,new PhysicsCollider()
                        // {
                        //     Value = boxCollider
                        // });
                        _ParallelWriter.SetComponent(sortKey,entity,LocalTransform.FromMatrix(
                            float4x4.TRS(
                                new float3(x,y,z),
                                quaternion.identity,
                                1)
                        ));
    
    
                    }
                }
            }
    
    
        }
    
        public void moveAabb(ref Aabb aabb, float3 pos)
        {
            aabb.Max.x += pos.x;
            aabb.Max.y += pos.y;
            aabb.Max.z += pos.z;
            aabb.Min.x += pos.x;
            aabb.Min.y += pos.y;
            aabb.Min.z += pos.z;
        }
        public int getIndex(int x,int index)
        {
            return index * 3 + x;
        }
        public Aabb ExpandAabb(Aabb aabb,float3 velocity)
        {
            Aabb ret = new Aabb();
            if (velocity.x > 0)
            {
                ret.Max.x = aabb.Max.x + velocity.x * deltaTime;
                ret.Min.x = aabb.Min.x;
            }
            else
            {
                ret.Max.x = aabb.Max.x;
                ret.Min.x = aabb.Min.x + velocity.x * deltaTime;
            }
            
            
            if(velocity.y > 0)
            {
                ret.Max.y = aabb.Max.y + velocity.y * deltaTime;
                ret.Min.y = aabb.Min.y;
            }
            else
            {
                ret.Max.y = aabb.Max.y;
                ret.Min.y = aabb.Min.y + velocity.y * deltaTime;
            }
    
            if (velocity.z > 0)
            {
                ret.Max.z = aabb.Max.z + velocity.z * deltaTime;
                ret.Min.z = aabb.Min.z;
            }
            else
            {
                ret.Max.z = aabb.Max.z;
                ret.Min.z = aabb.Min.z + velocity.z * deltaTime;
            }
    
            ret.Max.x =  math.floor(ret.Max.x) + 1;
            ret.Max.y =  math.floor(ret.Max.y) + 1;
            ret.Max.z =  math.floor(ret.Max.z) + 1;
            ret.Min.x =  math.floor(ret.Min.x);
            ret.Min.y =  math.floor(ret.Min.y);
            ret.Min.z =  math.floor(ret.Min.z);
            return ret;
        } 
    
    }
    
    
    
    [UpdateInGroup(typeof(EntityColliderCreateSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial struct BlockColliderAddSystem:ISystem
    {
        private Entity colliderEntity;
        
        private EntityQuery _velocityQuery;
        private EntityQuery _colliderPrefabQuery;
        
        private void OnCreate(ref SystemState state)
        {
            _velocityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PhysicsVelocity>()
                .Build(ref state);
            _colliderPrefabQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockColliderPrefabType>()
                .Build(ref state);

        }

        // private void OnUpdate(ref SystemState state)
        // {
        //     if(colliderEntity== Entity.Null)
        //     {
        //         colliderEntity = _colliderPrefabQuery.ToEntityArray(Allocator.Temp)[0];
        //     }
        //     if(colliderEntity== Entity.Null)
        //     {
        //         return;
        //     }
        //     var _BlockCollider = state.EntityManager.GetComponentData<PhysicsCollider>(colliderEntity);
        //     var EntitiesWithVelocity = _velocityQuery.ToEntityArray(Allocator.Temp);
        //
        //     foreach (Entity ent in EntitiesWithVelocity)
        //     {
        //         var physicsCollider = state.EntityManager.GetComponentData<PhysicsCollider>(ent);
        //         var lt = state.EntityManager.GetComponentData<LocalTransform>(ent);
        //         var pv = state.EntityManager.GetComponentData<PhysicsVelocity>(ent);
        //          
        //         var aabb = physicsCollider.Value.Value.CalculateAabb();
        //         DrawAABB.MoveAABB(ref aabb,lt.Position);
        //         var expandAabb = ExpandAabb(aabb,pv.Linear,SystemAPI.Time.DeltaTime);
        //         
        //         for (int x = (int)expandAabb.Min.x-1; x <= (int)expandAabb.Max.x; x++)
        //         {
        //             for (int y = (int)expandAabb.Min.y-1; y <=(int)expandAabb.Max.y; y++)
        //             {
        //                 for (int z = (int)expandAabb.Min.z-1; z <= (int)expandAabb.Max.z; z++)
        //                 {
        //                     
        //                     var entity = state.EntityManager.Instantiate( colliderEntity);
        //                     
        //                     
        //                     state.EntityManager.RemoveComponent<BlockColliderPrefabType>(entity);
        //                     // BlobAssetReference<Collider> boxCollider = BoxCollider.Create(new BoxGeometry()
        //                     // {
        //                     //     Center = new float3(0,0,0),
        //                     //     Size = new float3(1,1,1),
        //                     //     Orientation = quaternion.identity,
        //                     // }, _BlockCollider.Value.Value.GetCollisionFilter());
        //                     //
        //                     //
        //                     // state.EntityManager.SetComponentData(entity,new PhysicsCollider()
        //                     // {
        //                     //     Value = boxCollider
        //                     // });
        //                     state.EntityManager.SetComponentData(entity,LocalTransform.FromMatrix(
        //                         float4x4.TRS(
        //                             new float3(x,y,z),
        //                             quaternion.identity,
        //                             1)
        //                     ));
        //         
        //         
        //         
        //                 }
        //             }
        //         }
        //     }
        //
        //
        //
        // }
        //
        //
        public Aabb ExpandAabb(Aabb aabb,float3 velocity,float deltaTime)
        {
            Aabb ret = new Aabb();
            if (velocity.x > 0)
            {
                ret.Max.x = aabb.Max.x + velocity.x * deltaTime;
                ret.Min.x = aabb.Min.x;
            }
            else
            {
                ret.Max.x = aabb.Max.x;
                ret.Min.x = aabb.Min.x + velocity.x * deltaTime;
            }
            
            
            if(velocity.y > 0)
            {
                ret.Max.y = aabb.Max.y + velocity.y * deltaTime;
                ret.Min.y = aabb.Min.y;
            }
            else
            {
                ret.Max.y = aabb.Max.y;
                ret.Min.y = aabb.Min.y + velocity.y * deltaTime;
            }
        
            if (velocity.z > 0)
            {
                ret.Max.z = aabb.Max.z + velocity.z * deltaTime;
                ret.Min.z = aabb.Min.z;
            }
            else
            {
                ret.Max.z = aabb.Max.z;
                ret.Min.z = aabb.Min.z + velocity.z * deltaTime;
            }
        
            ret.Max.x =  math.floor(ret.Max.x) + 1;
            ret.Max.y =  math.floor(ret.Max.y) + 1;
            ret.Max.z =  math.floor(ret.Max.z) + 1;
            ret.Min.x =  math.floor(ret.Min.x);
            ret.Min.y =  math.floor(ret.Min.y);
            ret.Min.z =  math.floor(ret.Min.z);
            return ret;
        } 
        private void OnUpdate(ref SystemState state)
        {
            if (colliderEntity == Entity.Null)
            {   
                EntityQuery _query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<BlockColliderPrefabType>()
                    .Build(ref state);
                colliderEntity = _query.ToEntityArray(Allocator.Temp)[0];
                
            }
        
            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
             
              
            state.Dependency = new CreateBlockCollider()
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                ColliderEntity = colliderEntity,
                 
                _ParallelWriter = entityCommandBuffer.AsParallelWriter()
            }.Schedule(state.Dependency);
            state.Dependency.Complete();
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        
        }
        
        
    }
}