using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCraftS.Config
{
    public static class TerrianConfig
    {
        public const  int ChunkSize = 16;
        public const  int MaxHeight = 256;
        public const  int BufferCapacity = ChunkSize * ChunkSize * MaxHeight;
        public readonly static int SeaLevel = 64;
        
        public const int MaxLoadedChunk = 200;
    }
}
