using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace MyCraftS.Chunk
{
    public struct ChunkBlocks : IComponentData
    {
        public int bufferIndex;
        public NativeSlice<int> blocks;
        
    }
}
