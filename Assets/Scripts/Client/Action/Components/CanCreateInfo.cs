using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Action
{
    public struct CanCreateInfo:IComponentData
    {
        public int3 position;
        public int blockId;
    }
}
