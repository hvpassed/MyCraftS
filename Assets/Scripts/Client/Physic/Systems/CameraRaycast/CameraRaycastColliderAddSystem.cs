using MyCraftS.Bake;
using MyCraftS.Chunk.Data;
using MyCraftS.Input;
using MyCraftS.Physic.SystemGroups;
using MyCraftS.Player.Data;
using MyCraftS.Setting;
using MyCraftS.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.Physic
{
    [BurstCompile]
    public struct CreateRaycastCollider:IJobParallelFor
    {
        [ReadOnly] public float3 start, end;
        public EntityCommandBuffer.ParallelWriter ecbp;
        [ReadOnly] public float rayDistance;
        public Entity rayCastEntity;
 
        public void Execute(int index)
        {
             
            int startx = (int)(math.floor(start.x - rayDistance));
 

            for (int z = (int)(math.floor(start.z - rayDistance)); z <= (int)(math.floor(start.z + rayDistance)); z++)
            {
                for(int y = (int)(math.floor(start.y-rayDistance));y<=(int)(math.floor(start.y+rayDistance));y++)
                {
                    float3 pos = new float3(startx+index, y, z);
                    if (LineIntersects(start, end, pos))
                    {
                        var entity = ecbp.Instantiate(index, rayCastEntity);
                        
                        
                        ecbp.RemoveComponent<RayColliderPrefabType>(index+1,entity);
                        
 
                        ecbp.SetComponent(index+2,entity,LocalTransform.FromMatrix(
                            float4x4.TRS(
                                new float3(startx+index,y,z),
                                quaternion.identity,
                                1)
                        ));
                    }
                }
            }
            
            
            
        }
        
        
        
        bool LineIntersects (float3 start, float3 end, float3 Pos)
        {
            float3 min = Pos, max = Pos + 1;
            
            
            float tmin = 0f;
            float tmax = 1f;

 
            for (int i = 0; i < 3; i++)
            {
                if (math.abs(end[i] - start[i]) < math.EPSILON)
                {
 
                    if (start[i] < min[i] || start[i] > max[i])
                    {
                        return false;
                    }
                }
                else
                {
 
                    float t1 = (min[i] - start[i]) / (end[i] - start[i]);
                    float t2 = (max[i] - start[i]) / (end[i] - start[i]);

                    tmin = math.max(tmin, math.min(t1, t2));
                    tmax = math.min(tmax, math.max(t1, t2));

                    if (tmax < tmin)
                    {
 
                        return false;
                    }
                }
            }

 
 
            return true;
        }
    }


    [UpdateInGroup(typeof(PhysicsPreProcessSystemGroup))]
    
    public partial struct CameraRaycastColliderAddSystem:ISystem
    {
        private float3 cameraOffset;
        public Entity rayCastEntity;
        
        private void OnCreate(ref SystemState state)
        {
            cameraOffset = SettingManager.PlayerSetting.CameraOffset;
            
        }
        
        private void OnUpdate(ref SystemState state)
        {
            if (rayCastEntity == Entity.Null)
            {
                EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<RayColliderPrefabType>()
                    .Build(ref state);
                rayCastEntity = query.ToEntityArray(Allocator.Temp)[0];
            }

 
            CameraForward cameraForward =
                state.EntityManager.GetComponentData<CameraForward>(PlayerDataContainer.cameraEntity);
            cameraOffset = SettingManager.PlayerSetting.CameraOffset;
            LocalTransform playerTransform = state.EntityManager.GetComponentData<LocalTransform>(PlayerDataContainer.playerEntity);
            float3 cameraPosition = playerTransform.Position + cameraOffset;

            float3 end = cameraPosition+cameraForward.direction * SettingManager.PlayerSetting.rayDistance;
            
            int length = 1
                         + (int)(math.floor(cameraPosition.x+SettingManager.PlayerSetting.rayDistance)) 
                         -(int)( math.floor(cameraPosition.x-SettingManager.PlayerSetting.rayDistance));
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
 

            state.Dependency = new CreateRaycastCollider()
            {
                start = cameraPosition,
                end = end,
                ecbp =ecb.AsParallelWriter() ,
                rayDistance = SettingManager.PlayerSetting.rayDistance,
                rayCastEntity = rayCastEntity,
 
            }.Schedule(length, 16, state.Dependency);
            state.Dependency.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
        
        
 
        bool LineIntersects (float3 start, float3 end, float3 Pos)
        {
            float3 min = Pos, max = Pos + 1;
            
            
            float tmin = 0f;
            float tmax = 1f;

 
            for (int i = 0; i < 3; i++)
            {
                if (math.abs(end[i] - start[i]) < math.EPSILON)
                {
 
                    if (start[i] < min[i] || start[i] > max[i])
                    {
                        return false;
                    }
                }
                else
                {
 
                    float t1 = (min[i] - start[i]) / (end[i] - start[i]);
                    float t2 = (max[i] - start[i]) / (end[i] - start[i]);

                    tmin = math.max(tmin, math.min(t1, t2));
                    tmax = math.min(tmax, math.max(t1, t2));

                    if (tmax < tmin)
                    {
 
                        return false;
                    }
                }
            }

            Aabb aabb = new Aabb();
            aabb.Min = min;
            aabb.Max = max;
            DrawAABB.draw(aabb,Color.cyan,0.1f);
            return true;
        }
        
    }
}