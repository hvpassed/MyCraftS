using MyCraftS.Chunk.Data;
using MyCraftS.Data.IO;
using MyCraftS.Utils;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace MyCraftS.Block.Utils
{
    public static class BlockHelper
    {
        private static int3[] deltaPoses;

        static BlockHelper()
        {
            deltaPoses = new int3[6]
            {
                new int3(1, 0, 0),
                new int3(-1, 0, 0),
                new int3(0, 1, 0),
                new int3(0, -1, 0),
                new int3(0, 0, 1),
                new int3(0, 0, -1)
            };
        }
        public static bool CanBlockByOther(int self,int other)
        {
            if (other == 0||other==-1)
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

        public static bool IsBlocked(int3 WorldPosition)
        {
            int selfId = ChunkDataContainer.getBlockid(WorldPosition);
            bool isBlocked = true;
            foreach (int3 deltaPose in deltaPoses)
            {
                int otherId = ChunkDataContainer.getBlockid(WorldPosition + deltaPose);
                if (!CanBlockByOther(selfId, otherId))
                {
                    isBlocked = false;
                    break;
                }
            }

            return isBlocked;
        }
        public static bool IsBlocked(float3 WorldPosition)
        {
            float3 floord = math.floor(WorldPosition);
            int3 int3 = (int3) floord;
            return IsBlocked(int3);
        }
        
        
    }
}