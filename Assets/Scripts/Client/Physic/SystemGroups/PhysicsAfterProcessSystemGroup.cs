using Unity.Entities;
using Unity.Physics.Systems;

namespace MyCraftS.Physic.SystemGroups
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial class PhysicsAfterProcessSystemGroup:ComponentSystemGroup
    {
        
    }
}