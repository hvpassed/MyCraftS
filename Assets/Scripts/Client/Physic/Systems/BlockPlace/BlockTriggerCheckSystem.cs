using MyCraftS.Action;
using MyCraftS.Bake.Baker;
using MyCraftS.Physic.SystemGroups;
using MyCraftS.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace MyCraftS.Physic
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateBefore(typeof(RayCastSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial class BlockTriggerCheckSystem : SystemBase
    {


        [BurstCompile]
        public partial struct CheckBlockTriggerJob : ITriggerEventsJob
        {
             
            public Entity CanCreateEntity;
            public ComponentLookup<PlayerType> playerTypeLookUp;
            public ComponentLookup<BlockTriggerType> blcokTriggerTypeLookUp;
            public NativeReference<bool> canCreate;
            public void Execute(TriggerEvent triggerEvent)
            {
                 
                if (playerTypeLookUp.HasComponent(triggerEvent.EntityA)&& blcokTriggerTypeLookUp.HasComponent(triggerEvent.EntityB))
                {
                    canCreate.Value = false;
                    
                }
                else if (playerTypeLookUp.HasComponent(triggerEvent.EntityB) && blcokTriggerTypeLookUp.HasComponent(triggerEvent.EntityA))
                {
                    canCreate.Value = false;
                }
                Debug.Log("Trigger");
            }
        }

 
        private ComponentLookup<PlayerType> _playerTypeLookUp;
        private ComponentLookup<BlockTriggerType> _blockTriggerTypeLookUp;
 
        protected override void OnCreate()
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();

            
 
            _playerTypeLookUp = SystemAPI.GetComponentLookup<PlayerType>();

            _blockTriggerTypeLookUp = SystemAPI.GetComponentLookup<BlockTriggerType>();
        }



        protected override void OnUpdate()
        {
            if (!BlockTriggerAddSystem.CheckOnce)
            {
                return;
            }
            _playerTypeLookUp.Update(this);
            _blockTriggerTypeLookUp.Update(this);
            NativeReference<bool> canc = new NativeReference<bool>(Allocator.TempJob);

            canc.Value = true;
            this.Dependency = new CheckBlockTriggerJob()
            {
                 
                 CanCreateEntity=BlockTriggerAddSystem.CanCreateEntity,
                  
                playerTypeLookUp = _playerTypeLookUp,
                blcokTriggerTypeLookUp = _blockTriggerTypeLookUp,
                canCreate = canc
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(),this.Dependency);
            this.Dependency.Complete();
            EntityManager.SetComponentEnabled<CanCreate>(BlockTriggerAddSystem.CanCreateEntity,canc.Value);
            canc.Dispose();
            BlockTriggerAddSystem.CanDestroy = true;
            BlockTriggerAddSystem.CheckOnce = false;
        }
    }
}
