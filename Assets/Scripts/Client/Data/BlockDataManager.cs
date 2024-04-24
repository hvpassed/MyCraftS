using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
namespace MyCraftS.Data.IO
{
    /// <summary>
    /// 查询方块信息
    /// </summary>
    public static class BlockDataManager 
    {
        /// <summary>
        /// 名称到唯一ID
        /// </summary>
        public static  NativeHashMap<FixedString512Bytes, int> BlockIDLookUp;

        //public static   Dictionary<int, GameObject> BlockPrefabLookUp;

        public  static NativeHashMap<int, BlockInfo> BlockIDToInfoLookUp;

        public  static NativeHashMap<FixedString512Bytes, BlockInfo> BlockNameToInfoLookUp;


        public static NativeHashMap<int, Entity> BlockIdToEntityLookUp;


        public static void Init(int blockCount)
        {
            BlockIDLookUp = new NativeHashMap<FixedString512Bytes, int>(blockCount, Allocator.Persistent);
            BlockIDToInfoLookUp = new NativeHashMap<int, BlockInfo>(blockCount, Allocator.Persistent);
            BlockNameToInfoLookUp = new NativeHashMap<FixedString512Bytes, BlockInfo>(blockCount, Allocator.Persistent);
            BlockIdToEntityLookUp = new NativeHashMap<int, Entity>(blockCount, Allocator.Persistent);
            //BlockPrefabLookUp = new Dictionary<int, GameObject>(blockCount);
        }
        public static void Clear()
        {
            BlockIDLookUp.Dispose();
            BlockIDToInfoLookUp.Dispose();
            BlockNameToInfoLookUp.Dispose();
        }

    }
}