using MyCraftS.Action;
using MyCraftS.Block.Utils;
using MyCraftS.Data.IO;
using Unity.Collections;
using Unity.Entities;

namespace MyCraftS.Block.Update
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(BlockUpdateSystemGroup))]
    [UpdateAfter(typeof(BlockDestroyCleanUpSystem))]
    public partial class BlockCreateSystem:SystemBase
    {
        private EntityQuery _queryWantCreate;
        
        protected override void OnCreate()
        {
            _queryWantCreate = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<WantCreate>()
                .Build(this);
        }


        protected override void OnUpdate()
        {
            if (_queryWantCreate.IsEmpty)
            {
                return;
            }

            var entities = _queryWantCreate.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                WantCreate wantCreate = EntityManager.GetComponentData<WantCreate>(entity);
                BlockHelper.CreateBlock(EntityManager,wantCreate.position,wantCreate.blockId);
                BlockHelper.AddShouldUpdateBlock(wantCreate.position);
                EntityManager.DestroyEntity(entity);
            }
            

        }
    }
}