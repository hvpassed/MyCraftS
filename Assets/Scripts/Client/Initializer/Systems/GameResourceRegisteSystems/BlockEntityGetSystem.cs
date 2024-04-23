
using MyCraftS.Bridge;
using MyCraftS.Chunk.Manage;
using MyCraftS.Data.IO;
using MyCraftS.Time;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;
using Debug = UnityEngine.Debug;
namespace MyCraftS.Initializer
{
    [UpdateInGroup(typeof(GameResourceRegisteUpdateSystemGroup))]
    public partial class BlockEntityGetSystem : SystemBase
    {
        public static Entity[] constuctEntity;
        private int loaded = 0;
        public BlockEntityGetSystem()
        {
           
        }


        protected  override void OnUpdate()
        {
            
            int id = 1;
            if (constuctEntity == null)
            {
                Debug.LogError($"Block Entity Get System:no constuctEntity found");
            }
            foreach(Entity entity in constuctEntity)
            {
                bool isLoaded = EntityManager.HasComponent<PrefabLoadResult>(entity);


                if (isLoaded)//如果加载完成
                {
                    
                    if (!BlockDataManager.Instance.BlockIdToEntityLookUp.ContainsKey(id))//如果没有加载过
                    {
                        Debug.Log($"Block Entity Get System:{id} Loaded");
                        var et = EntityManager.GetComponentData<PrefabLoadResult>(entity).PrefabRoot;
                        EntityManager.RemoveComponent<PhysicsCollider>(et);

                        BlockDataManager.Instance.BlockIdToEntityLookUp.Add(id, et);
                        loaded++;
                    }
                    
                }

                id++;
            }
            if (loaded == constuctEntity.Length)
            {
 
 
                this.Enabled = false;
                TickUpdateGroup.Instance.Enabled = true;
                Debug.Log("Block loaded.TickUpdateGroup enable");
                Triggers.BlockLoaded = 1;

            }


        }
    }
}
