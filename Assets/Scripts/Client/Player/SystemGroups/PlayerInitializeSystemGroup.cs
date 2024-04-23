using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace MyCraftS.Player
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PlayerInitializeSystemGroup:ComponentSystemGroup
    {
    }
}
