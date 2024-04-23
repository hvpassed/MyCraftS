using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace MyCraftS.Data
{
    public enum MyCraftSType
    {
        Block,
        Drop,
        Chunk,
        Creature,
        
    }


    public struct MyCraftSEntityType:IComponentData
    {
        public MyCraftSType type;
    }
}
