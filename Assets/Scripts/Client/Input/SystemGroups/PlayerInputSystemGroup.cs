using Unity.Entities;
using Unity.Physics.Systems;

namespace MyCraftS.Input
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial class PlayerInputSystemGroup:ComponentSystemGroup
    {
        
    }
}