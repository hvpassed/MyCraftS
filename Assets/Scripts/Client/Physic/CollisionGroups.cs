using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCraftS.Physic
{
    public static class CollisionGroups
    {
        public static readonly uint BlockGroup = 1<<1;
        public static readonly uint LiquidGroup = 1<<2;
        public static readonly uint CreatureGroup = 1<<4;
    }
}
