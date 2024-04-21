using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Data
{
    [Serializable]
    public enum ColliderType { 
        StanderdBlock,
        Other
    }
    [Serializable]
    public class BlockInfo
    {
        public int blockId;

        public bool canCollide;

        public bool canBeDestroyed;

        public bool transparent;
        public ColliderType colliderType;

        public bool hasDirect;

        public bool isLiquid;
    }
}
