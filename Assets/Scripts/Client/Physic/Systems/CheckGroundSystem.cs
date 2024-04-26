using MyCraftS.Physic.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace MyCraftS.Physic
{
    [BurstCompile]
    public partial struct  CheckEntityGrounded:IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter _ecbp;

        [ReadOnly] public NativeArray<float3> deltaPos;
        [ReadOnly] public CollisionFilter filter;
        [ReadOnly] public float checkPos;
        [ReadOnly] public CollisionWorld _world;
        public ComponentType groundedType;
        private void Execute([ChunkIndexInQuery] int sortKey,Entity entity,in LocalTransform transform,in PhysicsVelocity physicsVelocity)
        {

            bool grounded = false;
            foreach (var delta in deltaPos)
            {
                float3 startPostion = transform.Position + delta;
                RaycastInput raycastInput = new RaycastInput()
                {
                    Start = startPostion,
                    End = startPostion + new float3(0, -1, 0) * checkPos,
                    Filter = filter
                };
                
                if (_world.CastRay(raycastInput, out RaycastHit hit))
                {
                    grounded = true;
                    break;
                }
            }

            if (grounded && physicsVelocity.Linear.y < 0)
            {
                 
                _ecbp.SetComponentEnabled(sortKey,entity,groundedType,true);
            }
            else
            {
                 
                _ecbp.SetComponentEnabled(sortKey,entity,groundedType,false);
            }
            
        }
        
            
        
    }
    
    
    
    [UpdateInGroup(typeof(RayCastSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial class CheckGroundSystem:SystemBase
    {
        private EntityQuery singletonQuery;
        private EntityQuery _physicsVAndIsGrounded;
        private float radius = 0.5f;
        private CollisionFilter _collisionFilter;
        private float _distance = 0.1f;
        private NativeArray<float3> start;
        private ComponentType _isGroundedType;
        protected override void OnCreate()
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();

              singletonQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(builder);
            _physicsVAndIsGrounded = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PhysicsVelocity>()
                .WithAll<IsGrounded>()
                .WithAll<LocalTransform>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build(this);
            _collisionFilter = new CollisionFilter()
            {
                BelongsTo = CollisionGroups.CreatureGroup,
                CollidesWith = CollisionGroups.BlockGroup
            };
            start = new NativeArray<float3>(4, Allocator.Persistent);
            start[0] = new float3(-0.5f, 0, -0.5f);
            start[1] = new float3(-0.5f, 0, 0.5f);
            start[2] = new float3(0.5f, 0, -0.5f);
            start[3] = new float3(0.5f, 0, 0.5f);
            _isGroundedType = typeof(IsGrounded);
        }
        [BurstCompile]
        protected override void OnUpdate()
        {
            var collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            this.Dependency = new CheckEntityGrounded()
            {
                _ecbp = ecb.AsParallelWriter(),
                deltaPos = start,
                _world = collisionWorld,
                checkPos = _distance,
                filter = _collisionFilter,
                groundedType = _isGroundedType
            }.ScheduleParallel(_physicsVAndIsGrounded, this.Dependency);
            this.Dependency.Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }


        // protected override void OnUpdate()
        // {
        //     var collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        //     var entities = _physicsVAndIsGrounded.ToEntityArray(Allocator.Temp);
        //     foreach (Entity entity in entities)
        //     {
        //         LocalTransform transform = EntityManager.GetComponentData<LocalTransform>(entity);
        //         
        //         float3[] start = new float3[4]
        //         {
        //             new float3(-0.5f, 0, -0.5f) + transform.Position,
        //             new float3(-0.5f, 0, 0.5f) + transform.Position,
        //             new float3(0.5f, 0, -0.5f) + transform.Position,
        //             new float3(0.5f, 0, 0.5f) + transform.Position,
        //         };
        //         bool ground = false;
        //         
        //         foreach (float3 startPoint in start)
        //         {
        //             var input = new RaycastInput()
        //             {
        //                 Start = startPoint,
        //                 End = startPoint + new float3(0, -1, 0) * _distance,
        //                 Filter = _collisionFilter
        //             };
        //             RaycastHit hit = new RaycastHit();
        //             if (collisionWorld.CastRay(input, out hit))
        //             {
        //                 ground = true;
        //                 break;
        //             }
        //         }
        //         
        //         EntityManager.SetComponentEnabled<IsGrounded>(entity,ground);
        //
        //     }
        //     
        // }
    }
}