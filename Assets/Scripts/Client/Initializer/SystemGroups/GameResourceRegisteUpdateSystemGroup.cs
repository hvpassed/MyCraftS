using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Physics.Systems;

namespace MyCraftS.Initializer
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial class GameResourceRegisteUpdateSystemGroup : ComponentSystemGroup
    {
    }
}
