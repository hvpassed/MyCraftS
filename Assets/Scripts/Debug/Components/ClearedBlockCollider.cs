using Unity.Entities;
using Unity.Physics;

namespace MyCraftS.DeBug
{
    public struct ClearedBlockCollider:ICleanupComponentData
    {
        public Aabb aabb;
    }
}