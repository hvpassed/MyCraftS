using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Block
{
    public struct BlockDestroyCleanUp:IComponentData,ICleanupComponentData
    {
        public int3 position;
    }
}