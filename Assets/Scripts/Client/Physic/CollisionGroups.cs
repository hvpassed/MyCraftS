using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCraftS.Physic
{
    public static class CollisionGroups
    {
        
        public static readonly uint BlockGroup = 1;
        public static readonly uint LiquidGroup = 2;
        public static readonly uint CreatureGroup = 4;
        public static readonly uint RaycastGroup = 8;
        public static readonly uint NoneGroup = 16;



        public static readonly int RaycastGroupIndex = 1;
    }
}
