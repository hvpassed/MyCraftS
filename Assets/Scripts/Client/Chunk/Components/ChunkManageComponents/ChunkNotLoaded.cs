using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Chunk.Manage
{
    public struct ChunkNotLoaded:IComponentData
    {
        public NativeHashSet<int3> waitForLoaded;

    }
}
