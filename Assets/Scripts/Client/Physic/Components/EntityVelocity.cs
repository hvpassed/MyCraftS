using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Physic.Move
{
    public struct EntityVelocity:IComponentData
    {
        public float3 velocity;
    }
}