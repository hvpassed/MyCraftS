using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace MyCraftS.Bake
{
    public class PhysicParentAuthoring : MonoBehaviour
    {
        public GameObject child;
    }


    public class PhysicParentBaker : Baker<PhysicParentAuthoring>
    {
        public override void Bake(PhysicParentAuthoring authoring)
        {
            var parentEntity = GetEntity(TransformUsageFlags.Dynamic);
            var childEntity = GetEntity(authoring.child, TransformUsageFlags.Dynamic);
            PhysicParent physicParent = new PhysicParent
            {
                child = childEntity
            };
            
            AddComponent(parentEntity,physicParent);
        }
    }
}
