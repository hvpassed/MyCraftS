using MyCraftS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCraftS.Data.IO
{
    [Serializable]
    public enum ColliderType { 
        StanderdBlock,
        Other
    }
    [Serializable]
    public class BlockInfoForYaml
    {
        public int blockId;

        public bool canCollide;

        public bool canBeDestroyed;

        public bool transparent;
        public ColliderType colliderType;

        public bool hasDirect;

        public bool isLiquid;
    }

    public struct BlockInfo
    {
        public int blockId;

        public MyCraftsBoolean canCollide;

        public MyCraftsBoolean canBeDestroyed;

        public MyCraftsBoolean transparent;

        public ColliderType colliderType;

        public MyCraftsBoolean hasDirect;

        public MyCraftsBoolean isLiquid;

    }


    public static class BlockInfoCreator
    {
        public static BlockInfo createBlockInfo(BlockInfoForYaml yamlType)
        {
            BlockInfo blockInfo;
            blockInfo.blockId = yamlType.blockId;
            blockInfo.canCollide = convertBoolean(yamlType.canCollide);
            blockInfo.canBeDestroyed = convertBoolean(yamlType.canBeDestroyed);
            blockInfo.transparent = convertBoolean(yamlType.transparent);
            blockInfo.colliderType = yamlType.colliderType;
            blockInfo.hasDirect = convertBoolean(yamlType.hasDirect);
            blockInfo.isLiquid = convertBoolean(yamlType.isLiquid);
                


            return blockInfo;
        }

        public static MyCraftsBoolean convertBoolean(bool b) {
        
            return b?MyCraftsBoolean.True:MyCraftsBoolean.False;
        
        }
    }
}
