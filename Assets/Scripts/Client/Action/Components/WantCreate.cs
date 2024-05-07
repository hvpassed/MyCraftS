using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Action
{
    public struct WantCreate:IComponentData
    {
        public int3 position;
        public bool canCollide;
        public bool hasDirection;
        public int blockId;
        

    }
}