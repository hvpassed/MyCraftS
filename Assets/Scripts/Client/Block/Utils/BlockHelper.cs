using MyCraftS.Data.IO;
using MyCraftS.Utils;
using Unity.Burst;
using UnityEngine;

namespace MyCraftS.Block.Utils
{
    public static class BlockHelper
    {
 
        public static bool CanBlockByOther(int self,int other)
        {
            if (other == 0)
                return false;
            var selfinfo =BlockDataManager.BlockIDToInfoLookUp[self];
            var otherinfo =BlockDataManager.BlockIDToInfoLookUp[other];
            //如果是液体，液体可以被液体任何方块遮挡（除了空气）
            if (selfinfo.isLiquid == MyCraftsBoolean.True)
            {
 
                return true;
            }
            else
            {
                if(otherinfo.isLiquid == MyCraftsBoolean.True)
                {
                    return false;
                }
 
                return true;
            }
 
        }
        
        
        
        
        
    }
}