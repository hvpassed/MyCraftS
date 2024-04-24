 
using System.IO;
using MyCraftS.Bake.Baker;
using MyCraftS.Block;
using Unity.Entities;
using UnityEngine;
using YamlDotNet.Serialization;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using YamlDotNet.Serialization.NamingConventions;
using MyCraftS.Initializer.Creator;
using Unity.Scenes;
using Unity.Entities.Serialization;
using MyCraftS.Data.IO;
using Unity.Collections;
using UnityEditor;
using Hash128 = Unity.Entities.Hash128;


namespace MyCraftS.Initializer
{
    [UpdateInGroup(typeof(GameResourceRegisteSystemGroup))]
    public partial class BlockRegisteSystem : SystemBase
    {

         
 
        private float timer = 0f;
        private int loaded = 0;
        private EntityQuery query;
        public BlockRegisteSystem()
        {
             

        }

 
        protected override void OnCreate()
        {

            query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlockEntityPrefab>()
                .WithAll<BlockID>()
                .Build(this);
            }



    

        protected override void OnUpdate()
        {
            if (query.CalculateEntityCount() != 0)
            {
                Debug.Log($"Block Registe System:Find {query.CalculateEntityCount()} Block Prefab To register");
                NativeArray<Entity> entityPrefabOwnerList = query.ToEntityArray(Allocator.Temp);                
                BlockEntityGetSystem.constuctEntity = new Entity[entityPrefabOwnerList.Length];
                var deserializer = new DeserializerBuilder().WithNamingConvention(NullNamingConvention.Instance)
                .Build();
                string rootSource = "Block";
                BlockDataManager.Init(entityPrefabOwnerList.Length);
                BlocksFileName blocksFileName = deserializer.Deserialize<BlocksFileName>(
                    Resources.Load<TextAsset>(Path.Combine(rootSource, "Blocks")).text);

                foreach (string blockDirName in blocksFileName.Blocks)
                {
                    string blockPath = Path.Combine(rootSource, blockDirName);
                    string blockInfoFileName = $"{blockDirName}_info";
                    string blockResFileName = $"{blockDirName}_res";
                    BlockInfoForYaml blockInfo = deserializer.Deserialize<BlockInfoForYaml>(
                        Resources.Load<TextAsset>(Path.Combine(blockPath, blockInfoFileName)).text);
                    MCForYaml mc = deserializer.Deserialize<MCForYaml>(
                        Resources.Load<TextAsset>(Path.Combine(blockPath, blockResFileName)).text);
                    RegisterToBlockDataManager(mc,blockInfo);
                    int blockID = mc.id;
                    Entity current = GetPrefabEntity(entityPrefabOwnerList, blockID);
                    BlockEntityPrefab blockEntityPrefab = EntityManager.GetComponentData<BlockEntityPrefab>(current);
                    BlockEntityGetSystem.constuctEntity[blockID - 1] = EntityManager.CreateEntity();
                    Debug.Log($"Block Registe System: Registed Block {blockID},Waiting for Prefab Loaded");
                    EntityManager.AddComponentData<RequestEntityPrefabLoaded>(BlockEntityGetSystem.constuctEntity[blockID - 1],
                        new RequestEntityPrefabLoaded
                    {
                        Prefab = blockEntityPrefab.Prefab
                    });
                }
                Debug.Log("Block Registe System:Closed");
                BlockEntityGetSystem.blockEntityGetSystem.Enabled = true;
                Debug.Log("Block Registe System:Enable Block Entity Get System");
                this.Enabled = false;
            }
        }


        private Entity GetPrefabEntity(NativeArray<Entity> entitylist, int id)
        {
            foreach (var entity in entitylist)
            {
                BlockID blockID = EntityManager.GetComponentData<BlockID>(entity);
                if (blockID.Id == id)
                {
                    return entity;
                }
            }

            return Entity.Null;
        }


        private void RegisterToBlockDataManager(MCForYaml mc,BlockInfoForYaml blockinfo)
        {
            BlockDataManager.BlockIDLookUp.Add($"MyCraftS:{mc.name}",mc.id);
            BlockDataManager.BlockNameToInfoLookUp.Add($"MyCraftS:{mc.name}",BlockInfoCreator.createBlockInfo(blockinfo));
            BlockDataManager.BlockIDToInfoLookUp.Add(mc.id,BlockInfoCreator.createBlockInfo(blockinfo));
        }
    }
}
