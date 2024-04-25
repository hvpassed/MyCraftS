using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Data.Component
{
    public struct WorldCoord:IComponentData
    {
        public int3 coord;
    }
}