using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace MyCraftS.Bake
{
    [BakingType]
    public struct PhysicParent : IComponentData
    {
        public Entity child;
    }

    [BakingType]
    public struct PhysicChild : IComponentData
    {
 
    }
}