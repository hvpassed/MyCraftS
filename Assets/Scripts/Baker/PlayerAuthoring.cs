using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;

namespace MyCraftS.Bake
{
    public class PlayerAuthoring : MonoBehaviour
    {

    }

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var playerEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(playerEntity, new MyCraftS.Player.PlayerType());
            //AddComponent(playerEntity, new PhysicsVelocity());
        }
    }
}

