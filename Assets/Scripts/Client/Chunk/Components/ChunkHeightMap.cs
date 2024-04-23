using MyCraftS.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace MyCraftS.Chunk
{
    [InternalBufferCapacity(TerrianConfig.ChunkSize*TerrianConfig.ChunkSize)]
    public struct ChunkHeightMap :IBufferElementData
    {
        public int height;
        public static implicit operator int(ChunkHeightMap e) => e.height;
        public static implicit operator ChunkHeightMap(int e) => new ChunkHeightMap { height = e };
    }
        
    
}
