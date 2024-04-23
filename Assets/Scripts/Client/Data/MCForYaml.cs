
using System.Globalization;
using Unity.Collections;
namespace MyCraftS.Data.IO
{
    [System.Serializable]
    public enum MCType
    {
        Block,Creature,Item,Other
    }

    [System.Serializable]
    public enum MCShape
    {
        Cube,Other
    }

 


    [System.Serializable]
    public struct TexturePathForYaml
    {
        public string back;
        public string right;
        public string top;
        public string bottom;
        public string front;
        public string left;

        #region Indexer
        public string this[int i]
        {
            get
            {
                string ret = back;
                switch (i)
                {
                    case 0:
                        ret = back;
                        break;
                    case 1:
                        ret = right;
                        break;
                    case 2:
                        ret = top;
                        break;
                    case 3:
                        ret = bottom;
                        break;
                    case 4:
                        ret = front;
                        break;
                    case 5:
                        ret = left;
                        break;

                }

                return ret;
            }
        }
        #endregion
        public string[] ConstrctPath(string dir)
        {
            string[] ret = new string[6];
            for (int i = 0; i < 6; i++)
            {
                ret[i] = System.IO.Path.Combine(dir, this[i]);
            }
            return ret;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "back: {0}, right: {1}, front: {2}, left: {3}, top: {4}, bottom: {5}", back, right, front, left, top, bottom);
        }
    }
    [System.Serializable]
    public struct TextureForYaml
    {
        public int size;
        public TexturePathForYaml diffuse;
        public TexturePathForYaml normal;
        public TexturePathForYaml specular;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "diffuse: {0}, \nnormal: {1}, \nspecular: {2}", diffuse, normal, specular);
        }
    }
    [System.Serializable]
    public class MCForYaml
    {
        public MCType type;
        public int id;
        public string name;
        public MCShape shape;
        public TextureForYaml texture;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "type: {0}, \nid: {1}, \nname: {2}, \nshape: {3}, \ntexture:\n {4}", type, id, name, shape, texture);
        }

    }

    public struct TexturePath
    {
        public FixedString512Bytes back;
        public FixedString512Bytes right;
        public FixedString512Bytes top;
        public FixedString512Bytes bottom;
        public FixedString512Bytes front;
        public FixedString512Bytes left;
    }


    public struct Texture
    {
        public int size;
        public TexturePath diffuse;
        public TexturePath normal;
        public TexturePath specular;
    }


    public struct MC
    {
        public MCType type;
        public int id;
        public FixedString512Bytes name;
        public MCShape shape;
        public Texture texture;
    }



    public static class MCInfoCreator
    {
        public static MC createMC(MCForYaml yamlType)
        {
 

            MC mc;
            mc.type = yamlType.type;
            mc.id = yamlType.id;
            mc.name = yamlType.name;
            mc.shape = yamlType.shape;
            mc.texture.size = yamlType.texture.size;
            mc.texture.diffuse.back = yamlType.texture.diffuse.back;
            mc.texture.diffuse.right = yamlType.texture.diffuse.right;
            mc.texture.diffuse.top = yamlType.texture.diffuse.top;
            mc.texture.diffuse.bottom = yamlType.texture.diffuse.bottom;
            mc.texture.diffuse.front = yamlType.texture.diffuse.front;
            mc.texture.diffuse.left = yamlType.texture.diffuse.left;

            mc.texture.normal.back = yamlType.texture.normal.back;
            mc.texture.normal.right = yamlType.texture.normal.right;
            mc.texture.normal.top = yamlType.texture.normal.top;
            mc.texture.normal.bottom = yamlType.texture.normal.bottom;
            mc.texture.normal.front = yamlType.texture.normal.front;
            mc.texture.normal.left = yamlType.texture.normal.left;

            mc.texture.specular.back = yamlType.texture.specular.back;
            mc.texture.specular.right = yamlType.texture.specular.right;
            mc.texture.specular.top = yamlType.texture.specular.top;
            mc.texture.specular.bottom = yamlType.texture.specular.bottom;
            mc.texture.specular.front = yamlType.texture.specular.front;
            mc.texture.specular.left = yamlType.texture.specular.left;

            return mc;
        }
    }
}