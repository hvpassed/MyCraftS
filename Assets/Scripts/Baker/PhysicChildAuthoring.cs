using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace  MyCraftS.Bake
{
    public class PhysicChildAuthoring : MonoBehaviour
    {
    }
    
    
    
    public class PhysicChildBaker : Baker<PhysicChildAuthoring>
    {
        public override void Bake(PhysicChildAuthoring authoring)
        {
            var  Entity = GetEntity(TransformUsageFlags.Dynamic);
 
            PhysicChild physicChild = new PhysicChild
            {
            };
            
            AddComponent(Entity,physicChild);
        }
    }
}

