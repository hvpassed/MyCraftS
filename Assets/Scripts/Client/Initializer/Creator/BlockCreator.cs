using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 创建正方体方块模版的类
/// </summary>
namespace MyCraftS.Initializer.Creator
{
    public class BlockCreator : Singleton<BlockCreator>
    {
        private List<Vector3> VertexList;
        private List<int> Index;
        private List<Vector2> UV;
        public GameObject cubeParent;
        //public Dictionary<int, GameObject> cubeDict;
        public string Layer
        {
            get
            {
                return "Block";
            }
        }
        public string TransparentLayer
        {
            get
            {
                return "TransparentBlock";
            }
        }
        public string Tag
        {
            get
            {
                return "Block";
            }
        }
        #region data
        public BlockCreator()
        {
            cubeParent = new GameObject("CubeParent");
            VertexList = new List<Vector3>
        {
            new Vector3(0,0,0),//后 z较小
            new Vector3(0,1,0),
            new Vector3(1,1,0),
            new Vector3(1,0,0),


            new Vector3(1,0,0),//右
            new Vector3(1,1,0),
            new Vector3(1,1,1),
            new Vector3(1,0,1),

            new Vector3(0,1,0),//顶
            new Vector3(0,1,1),
            new Vector3(1,1,1),
            new Vector3(1,1,0),

            new Vector3(0,0,0),//底
            new Vector3(1,0,0),
            new Vector3(1,0,1),
            new Vector3(0,0,1),

            new Vector3(0,0,1),//前
            new Vector3(1,0,1),
            new Vector3(1,1,1),
            new Vector3(0,1,1),

            new Vector3(0,0,0),//左
            new Vector3(0,0,1),
            new Vector3(0,1,1),
            new Vector3(0,1,0),
        };

            Index = new List<int>
        {
            0,1,2,//后
            0,2,3,

            4,5,6,//右
            4,6,7,

            8,9,10,//顶
            8,10,11,

            12,13,14,//底
            12,14,15,

            16,17,18,//前
            16,18,19,

            20,21,22,//左
            20,22,23
        };

            UV = new List<Vector2>
        {
            new Vector2(0/6f,0),
            new Vector2(0/6f,1),
            new Vector2(1/6f,1),
            new Vector2(1/6f,0),

            new Vector2(1/6f,0),
            new Vector2(1/6f,1),
            new Vector2(2/6f,1),
            new Vector2(2/6f,0),


            new Vector2(2/6f,0),
            new Vector2(2/6f,1),
            new Vector2(3/6f,1),
            new Vector2(3/6f,0),

            new Vector2(3/6f,1),
            new Vector2(4/6f,1),
            new Vector2(4/6f,0),
            new Vector2(3/6f,0),


            new Vector2(5/6f,0),
            new Vector2(4/6f,0),
            new Vector2(4/6f,1),
            new Vector2(5/6f,1),


            new Vector2(6/6f,0),
            new Vector2(5/6f,0),
            new Vector2(5/6f,1),
            new Vector2(6/6f,1),

        };
            Debug.Log("CubeCreator Created");
        }

        #endregion
        public GameObject CreateObjectTempate(string resoucePath)
        {
            throw new System.NotImplementedException();
        }


        public GameObject CreateObjectTemplate()
        {
            GameObject block = new GameObject("None");

            MeshFilter meshFilter = block.AddComponent<MeshFilter>();

            MeshRenderer meshRenderer = block.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = block.AddComponent<MeshCollider>();
            meshFilter.mesh = DrawCube();
            meshCollider.sharedMesh = meshFilter.mesh;
            block.layer = LayerMask.NameToLayer("Block");
            block.tag = "Block";
            block.transform.parent = cubeParent.transform;
            block.SetActive(false);

            return block;
        }
#if UNITY_EDITOR
        public void SaveMeshAsset(Mesh mesh, string name)
        {
            string path = $"Assets/Resources/Mesh/{name}.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"Mesh saved: {path}");
        }
#endif
        Mesh DrawCube()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = VertexList.ToArray();
            mesh.triangles = Index.ToArray();
            mesh.uv = UV.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();

            return mesh;
        }

        public GameObject CreateObjectTemplate(string name, Material material, int id,bool isTransparent)
        {
            Debug.Log($"Creating {name}@{id}");
            GameObject block = new GameObject(name);

            MeshFilter meshFilter = block.AddComponent<MeshFilter>();

            MeshRenderer meshRenderer = block.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = block.AddComponent<MeshCollider>();

            //meshFilter.mesh = DrawCube();
            meshFilter.sharedMesh = DrawCube();
            meshCollider.sharedMesh = meshFilter.mesh;
            //SaveMeshAsset(meshFilter.sharedMesh, name);
            if (isTransparent)
            {
                block.layer = LayerMask.NameToLayer("TransparentBlock");
            }
            else
            {
                block.layer = LayerMask.NameToLayer("Block");
            }
            block.tag = "Block";
            block.transform.parent = cubeParent.transform;

            meshRenderer.material = material;
            block.SetActive(false);
            //cubeDict.Add(id, block);
            return block;
        }


        public GameObject CreateObjectTemplate(string name, Material material, int id, bool isTransparent,Mesh mesh)
        {
            Debug.Log($"Creating {name}@{id}");
            GameObject block = new GameObject(name);

            MeshFilter meshFilter = block.AddComponent<MeshFilter>();

            MeshRenderer meshRenderer = block.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = block.AddComponent<MeshCollider>();

            //meshFilter.mesh = DrawCube();
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = meshFilter.mesh;
            //SaveMeshAsset(meshFilter.sharedMesh, name);
            if (isTransparent)
            {
                block.layer = LayerMask.NameToLayer("TransparentBlock");
            }
            else
            {
                block.layer = LayerMask.NameToLayer("Block");
            }
            block.tag = "Block";
            //block.transform.parent = cubeParent.transform;

            meshRenderer.material = material;
            block.SetActive(false);
            //cubeDict.Add(id, block);
            return block;
        }
    }
}