using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Input
{
    public struct CameraForward:IComponentData
    {
        public float3 direction;
    }
}