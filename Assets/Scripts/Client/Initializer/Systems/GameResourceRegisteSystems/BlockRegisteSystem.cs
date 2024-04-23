 
using System.IO;
using Unity.Entities;
using UnityEngine;
using YamlDotNet.Serialization;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using YamlDotNet.Serialization.NamingConventions;
using MyCraftS.Initializer.Creator;
using Unity.Scenes;
using Unity.Entities.Serialization;
using MyCraftS.Data.IO;


namespace MyCraftS.Initializer
{
    [UpdateInGroup(typeof(GameResourceRegisteSystemGroup))]
    public partial class BlockRegisteSystem : SystemBase
    {

         
 
        private float timer = 0f;
        private int loaded = 0;
        public BlockRegisteSystem()
        {
             

        }

 
        protected override void OnCreate()
        {


            Debug.Log("Creating Block Prfab");
            var deserializer = new DeserializerBuilder().WithNamingConvention(NullNamingConvention.Instance)
            .Build();       // .WithNamingConvention(CamelCaseNamingConvention.Instance)
            string rootPath = "Block";
            TextAsset filenameRecord = Resources.Load<TextAsset>("Block/Blocks");


            BlocksFileName blocksFileName = deserializer.Deserialize< BlocksFileName>(filenameRecord.text);

            BlockCreator blockCreator = BlockCreator.GetInstance();
            //blockCreator.cubeDict = new Dictionary<int, GameObject>();

            var cubeNum = blocksFileName.Blocks.Count;
            BlockEntityGetSystem.constuctEntity = new Entity[cubeNum];
 
            BlockDataManager.Instance.Init(cubeNum);
            int count = 0;

            foreach (var dirName in blocksFileName.Blocks)
            {



                string curPath = Path.Combine(rootPath, dirName);
                string filename = $"{dirName}_res";
                string blockInfoFileName = $"{dirName}_info";
                TextAsset textAsset = Resources.Load<TextAsset>(Path.Combine(curPath, filename));
                TextAsset blockInfoAsset = Resources.Load<TextAsset>(Path.Combine(curPath, blockInfoFileName));
                MCForYaml mc = deserializer.Deserialize<MCForYaml>(textAsset.text);
                BlockInfoForYaml blockInfo = deserializer.Deserialize<BlockInfoForYaml>(blockInfoAsset.text);
                string blockName = $"MyCraftS:{mc.name}";
                
                //Material material = Resources.Load(Path.Combine(curPath, "Materials", mc.name.ToString())) as Material;

                BlockDataManager.Instance.BlockIDToInfoLookUp.Add(mc.id, BlockInfoCreator.createBlockInfo(blockInfo));
                BlockDataManager.Instance.BlockIDLookUp.Add(blockName,mc.id);
                BlockDataManager.Instance.BlockNameToInfoLookUp.Add(blockName, BlockInfoCreator.createBlockInfo(blockInfo));

                //var go = BlockCreator.Instance.CreateObjectTemplate(mc.name.ToString(), material, mc.id, blockInfo.transparent);
                GameObject go = Resources.Load<GameObject>(Path.Combine(rootPath,mc.name,"Prefab",mc.name));
                BlockDataManager.Instance.BlockPrefabLookUp.Add(mc.id, go);
                BlockEntityGetSystem.constuctEntity[mc.id-1] = EntityManager.CreateEntity();
                EntityManager.AddComponentData<RequestEntityPrefabLoaded>(BlockEntityGetSystem.constuctEntity[mc.id - 1], new RequestEntityPrefabLoaded { 
                    Prefab = new EntityPrefabReference(go)
 
                });



                count++;
            }

             



                
        }

        protected override void OnUpdate()
        {
             
        }
    }
}
