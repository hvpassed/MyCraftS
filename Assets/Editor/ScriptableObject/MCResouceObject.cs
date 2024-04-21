
using Importer.Data;
using Importer.Meta;

using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;



[CreateAssetMenu(fileName = "MCResouceObject", menuName = "MC/ResoucesObject")]
public class MCResouceObject : ScriptableObject
{
    private enum TextureType
    {
        Diffuse,
        Normal,
        Specular
    };



    public MC mc;
    public BlockInfo blockInfo;
    private int size = 128;
    public MCResouceObject()
    {
        mc = new MC();
        blockInfo = new BlockInfo();
 
    }

    public void ProcessTexture()
    {
        var trueDir = getCurrntDir();
        CreateMaterialDir(trueDir);
        size = mc.texture.size;
        var diffpath = Path.Combine(trueDir, "diffuse.png");
        var normpath = Path.Combine(trueDir, "normal.png");
        var specpath = Path.Combine(trueDir, "specular.png");
        mergeImg(mc.texture.diffuse.ConstrctPath(trueDir), Path.Combine(trueDir, "diffuse.png"));
        mergeImg(mc.texture.normal.ConstrctPath(trueDir), Path.Combine(trueDir, "normal.png"));
        mergeImg(mc.texture.specular.ConstrctPath(trueDir), Path.Combine(trueDir, "specular.png"));
        AssetDatabase.Refresh();
        changeTextureProp(TextureType.Diffuse, diffpath);
        changeTextureProp(TextureType.Normal,normpath);
        changeTextureProp(TextureType.Specular,specpath);
        Debug.Log("MCResouceObject:Process Texture Done");
    }

    public void Process()
    {
        var serializer = new SerializerBuilder()
        .WithNamingConvention(NullNamingConvention.Instance)
        .Build();
        var name = mc.name;
        var mcYaml = serializer.Serialize(mc, typeof(MC));
        var blockInfoYaml = serializer.Serialize(blockInfo, typeof(BlockInfo));
        File.WriteAllText($"{getCurrntDir()}/{name}_res.txt", mcYaml);
        File.WriteAllText($"{getCurrntDir()}/{name}_info.txt", blockInfoYaml);

        AssetDatabase.Refresh();
        Debug.Log("MCResouceObject:Process Done");
    }

    private string getCurrntDir()
    {
        string fullPath = AssetDatabase.GetAssetPath(this);
        return Path.GetDirectoryName(fullPath);
    }




    private void CreateMaterialDir(string dir)
    {
        string path = Path.Combine(dir, "Materials");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);

        }
    }

    private void mergeImg(string[] inputImgs, string outputImg)
    {
        int totalWidth = 0;
        int maxHeight = 0;

        // 加载图片并计算总宽度和最大高度
        Texture2D[] textures = new Texture2D[inputImgs.Length];
        for (int i = 0; i < inputImgs.Length; i++)
        {
            textures[i] = LoadTexture(inputImgs[i]);
            totalWidth += textures[i].width;
            maxHeight = Mathf.Max(maxHeight, textures[i].height);
        }

        // 创建输出图片
        Texture2D outputTexture = new Texture2D(totalWidth, maxHeight);
        int currentWidth = 0;
        for (int i = 0; i < textures.Length; i++)
        {
            for (int y = 0; y < textures[i].height; y++)
            {
                for (int x = 0; x < textures[i].width; x++)
                {
                    outputTexture.SetPixel(currentWidth + x, y, textures[i].GetPixel(x, y));
                }
            }
            currentWidth += textures[i].width;
        }

        outputTexture.Apply();

        // 保存输出图片
        File.WriteAllBytes(outputImg, outputTexture.EncodeToPNG());


    }

    // 加载图片辅助方法
    private Texture2D LoadTexture(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(size, size);
            tex.LoadImage(fileData); //..这将自动调整图片的大小
        }
        return tex;
    }

    private void changeTextureProp(TextureType textureType,string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        if (textureType == TextureType.Normal)
        {
            importer.textureType = TextureImporterType.NormalMap;
        }

    }
}
