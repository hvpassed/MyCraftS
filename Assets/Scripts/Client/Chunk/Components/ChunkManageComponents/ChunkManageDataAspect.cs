using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace MyCraftS.Chunk.Manage
{
    readonly partial struct ChunkManageDataAspect :IAspect
    {
        readonly public RefRW<ChunkLoaded> chunkLoaded;
        readonly public RefRW<ChunkNotLoaded> chunkNotLoaded;
        
    }
}
