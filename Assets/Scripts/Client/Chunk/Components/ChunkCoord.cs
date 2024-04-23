using MyCraftS.Config;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace MyCraftS.Chunk
{
    public struct ChunkCoord : IComponentData
    {
        /// <summary>
        /// chunk的世界坐标
        /// </summary>
        public int3 chunkCoord;


 
    }


    public static class ChunkDataHelper
    {

        
        public static int IndexGetterXZ(int x,int z)
        {   
            if(x<0||x>=TerrianConfig.ChunkSize||z<0||z>=TerrianConfig.ChunkSize)
            {
                return -1;
            }
            return x + z * TerrianConfig.ChunkSize;
        }
        public static int IndexGetter(int x,int y,int z)
        {
            if(!CheckCoord(new int3(x, y, z)))
            {
                throw new System.Exception("坐标越界");
            }

            return x+y* TerrianConfig.ChunkSize+z* TerrianConfig.ChunkSize* TerrianConfig.MaxHeight;
 
        }

        public static bool CoordGetter(int index,out int x,out int y,out int z)
        {
            if(index<0||index >=(TerrianConfig.ChunkSize* TerrianConfig.ChunkSize* TerrianConfig.MaxHeight))
            {
                x = -1;
                y = -1;
                z = -1;
                return false;
            }
            else
            {
                int area = TerrianConfig.ChunkSize * TerrianConfig.MaxHeight;
                z = index / area;
                int remainder = index % area;
                y = remainder / TerrianConfig.ChunkSize;
                x = remainder % TerrianConfig.ChunkSize;
                return true;
            }
        } 
        private static bool CheckCoord(int3 coord)
        {
            if (coord.x < 0 || coord.x >= TerrianConfig.ChunkSize ||
                               coord.y < 0 || coord.y >= TerrianConfig.MaxHeight ||
                                              coord.z < 0 || coord.z >= TerrianConfig.ChunkSize)
            {
                return false;
            }
            return true;
        }


        public static int MyMod(int x,int m)
        {
            int r = x % m;
            if (r < 0 && m > 0)
            {
                r += m;
            }
            else if (r > 0 && m < 0)
            {
                r += m; 
            }
            return r;
        }
        /// <summary>
        /// 获取entity所在的chunk坐标
        /// </summary>
        /// <param name="entityPos"></param>
        /// <returns></returns>
        public static int3 GetChunkCoord(int3 entityPos)
        {
            int x = entityPos.x - (MyMod(entityPos.x, TerrianConfig.ChunkSize));
            int z = entityPos.z - (MyMod(entityPos.z, TerrianConfig.ChunkSize));


            return new int3(x, 0, z);
        }

        public static int3 GetChunkCoord(float3 entityPos)
        {
            int fx = Mathf.FloorToInt(entityPos.x);
            int fz = Mathf.FloorToInt(entityPos.z);
            int x = fx - (MyMod(fx, TerrianConfig.ChunkSize));
            int z = fz - (MyMod(fz, TerrianConfig.ChunkSize));
            return new int3(x, 0, z);
        }
    }
}
