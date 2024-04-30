using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Input
{
    public struct CameraOffSet:IComponentData
    {
        public float3 offset;
    }
}