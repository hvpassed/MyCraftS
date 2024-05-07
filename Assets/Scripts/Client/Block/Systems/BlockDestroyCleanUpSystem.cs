 
using System.Collections.Generic;
using MyCraftS.Block.Utils;
using MyCraftS.Chunk.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MyCraftS.Block.Update
{
 

    
    
    
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(BlockUpdateSystemGroup))]
    [UpdateAfter(typeof(BlockDestroySystem))]
    public partial class BlockDestroyCleanUpSystem:SystemBase
    {
        private EntityQuery _destroyedCleanup;
        
        private int3[] deltaPoses;
        protected override void OnCreate()
        {
            _destroyedCleanup = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockDestroyCleanUp>()
                .Build(this);

            deltaPoses = new int3[6]
            {
                new int3(1,0,0),
                new int3(-1,0,0),
                new int3(0,1,0),
                new int3(0,-1,0),
                new int3(0,0,1),
                new int3(0,0,-1)
            };
            
        }
        
        protected override void OnUpdate()
        {
            //Dictionary<BlockBelongToChunk,List<int3>> updatePositions = new Dictionary<BlockBelongToChunk, List<int3>>();
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            Entities
                .WithStoreEntityQueryInField(ref _destroyedCleanup)
                .ForEach(
                    (in Entity entity,in BlockDestroyCleanUp blockDestroyCleanUp) =>
                    {

                        int3 BlockPos = blockDestroyCleanUp.position;
                        BlockHelper.AddShouldUpdateBlock(BlockPos);

                        ecb.RemoveComponent<BlockDestroyCleanUp>(entity);
                         
                        
                    })
                .WithoutBurst()
                .Run();
                ecb.Playback(EntityManager);
                ecb.Dispose();

        }
    }
}