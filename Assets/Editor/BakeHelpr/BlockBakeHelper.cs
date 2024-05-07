using System.IO;
using MyCraftS.Bake.Baker;
using MyCraftS.Data.IO;
using Unity.VisualScripting;
using Unity.VisualScripting.YamlDotNet.Serialization.NamingConventions;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;
using NullNamingConvention = YamlDotNet.Serialization.NamingConventions.NullNamingConvention;

public class BlockBakeHelper : Editor
{
    [MenuItem("MC/Bake/Block")]
    public static void AddBlockTo()
    {
         GameObject BlockPrefabLoader = GameObject.Find("BlockPrefabLoader");
         if (BlockPrefabLoader ==null)
         {
             Debug.Log("Can't find BlockPrefabLoader");
             BlockPrefabLoader = new GameObject("BlockPrefabLoader");
         }
 
        var path = "Block";
        var deserializer = new DeserializerBuilder().WithNamingConvention(NullNamingConvention.Instance).Build();
        TextAsset blocksFiles = Resources.Load<TextAsset>(Path.Combine(path,"Blocks"));
        
        BlocksFileName blocksFileName = deserializer.Deserialize<BlocksFileName>(blocksFiles.text);
        
        var count  = blocksFileName.Blocks.Count;

        for (int i = 0; i < count; i++)
        {
            var blockInfoPath = Path.Combine(path, blocksFileName.Blocks[i], blocksFileName.Blocks[i] + "_info");
            var blockInfo = Resources.Load<TextAsset>(blockInfoPath);
            var blockInfoData = deserializer.Deserialize<BlockInfo>(blockInfo.text);
            var curObject = new GameObject(blocksFileName.Blocks[i]);
            curObject.transform.parent = BlockPrefabLoader.transform;
            var bake = curObject.AddComponent<BlockPrefabAuthoring>();
            var pa = Path.Combine(path, blocksFileName.Blocks[i], "Prefab", blocksFileName.Blocks[i]);
            
            Debug.Log(pa);
            bake.prefab = Resources.Load<GameObject>(pa);
            bake.id = blockInfoData.blockId;
        }
    }
}