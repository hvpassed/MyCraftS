using Importer.Meta;

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
public class RecordBlockResources:Editor
{
    [MenuItem("MC/Record Block Resources")]
    public static void Record()
    {
        //Application.dataPath '.../Asset/'
        string path = Path.Combine(Application.dataPath, "Resources/Block");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        var files = Directory.GetDirectories(path);

        BlocksFileName blocksFileName = new BlocksFileName();
        blocksFileName.Blocks = new List<string>(files.Length);
        var serializer = new SerializerBuilder().WithNamingConvention(NullNamingConvention.Instance).Build();
        foreach(var dir in files)
        {
            blocksFileName.Blocks.Add(Path.GetFileName(dir));
        }

        string blockFileNameYaml = serializer.Serialize(blocksFileName,typeof(BlocksFileName));
        File.WriteAllText(Path.Combine(path, "Blocks.txt"), blockFileNameYaml);


    }
    [MenuItem("MC/Get GUID")]
    public static void showGUID()
    {

        string guid = AssetDatabase.AssetPathToGUID("Assets/Resources/Block/bedrock/Prefab/bedrock.prefab");
        Debug.Log("GUID: " + guid);
    }
}
