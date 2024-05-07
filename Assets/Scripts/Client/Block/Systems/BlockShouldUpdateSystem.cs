using MyCraftS.Block.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MyCraftS.Block.Update
{
    partial struct SetEntityShouldUpdateEnable:IJobEntity
    {
        [ReadOnly] public NativeArray<int3> positions;
        public EntityCommandBuffer.ParallelWriter ecbp;
        private void Execute([ChunkIndexInQuery] int sortKey, Entity entity,in LocalTransform transform)
        {
            if(positions.Contains((int3)math.floor(transform.Position)))
            {
                
                ecbp.SetComponentEnabled<BlockShouldUpdate>(sortKey,entity,true);
                //Debug.Log($"Set Entity Should Update Enable: {transform.Position}");
            }
             
        }
    }
    
    [UpdateInGroup(typeof(BlockUpdateSystemGroup))]
    [UpdateBefore(typeof(BlockUpdateSystem))]
    public partial class BlockShouldUpdateSystem:SystemBase
    {
        private EntityQuery _blockEntityQuery;
        protected override void OnCreate()
        {
            _blockEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockType>()
                .WithAll<BlockBelongToChunk>()
                .WithAll<LocalTransform>()
                .Build(this);
        }


        protected override void OnUpdate()
        {
            var enumerator = BlockHelper.updatePositions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var cecb = new EntityCommandBuffer(Allocator.TempJob);
                 BlockBelongToChunk blockBelongToChunk = enumerator.Current.Key;
                    NativeArray<int3> positions = new NativeArray<int3>(enumerator.Current.Value.ToArray(), Allocator.TempJob);
                 _blockEntityQuery.SetSharedComponentFilter<BlockBelongToChunk>(blockBelongToChunk);
                 this.Dependency = new SetEntityShouldUpdateEnable()
                 {
                     positions = positions,
                     ecbp = cecb.AsParallelWriter()
                 }.ScheduleParallel(_blockEntityQuery, this.Dependency);
                 this.Dependency.Complete();
                 cecb.Playback(EntityManager);
                 cecb.Dispose();
                 positions.Dispose();
            }
            
            BlockHelper.ClearUpdatePositions();
        }
    }
}