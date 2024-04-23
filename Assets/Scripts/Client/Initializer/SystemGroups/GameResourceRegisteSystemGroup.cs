using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace MyCraftS.Initializer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameResourceRegisteSystemGroup: ComponentSystemGroup
    {
    }
}
