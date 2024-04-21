
using System.Globalization;
namespace Importer.Meta
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
    public struct TexturePath
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
    public struct Texture
    {
        public int size;
        public TexturePath diffuse;
        public TexturePath normal;
        public TexturePath specular;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "diffuse: {0}, \nnormal: {1}, \nspecular: {2}", diffuse, normal, specular);
        }
    }
    [System.Serializable]
    public class MC
    {
        public MCType type;
        public int id;
        public string name;
        public MCShape shape;
        public Texture texture;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "type: {0}, \nid: {1}, \nname: {2}, \nshape: {3}, \ntexture:\n {4}", type, id, name, shape, texture);
        }

    }
}