using System;
using System.Collections;
using System.Collections.Generic;
using MyCraftS.Physic;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace  MyCraftS.UI.DebugUI
{
    
    public struct RaycastInfoDebugUI : IComponentData
    {
        public float3 hitPos, blockPos;
        public HitSide hitSide;
    }
    
    
    public class RaycastInfo : MonoBehaviour
    {
        // Start is called before the first frame update
        
        public static Entity RaycastInfoEntity;
        private EntityManager _entityManager;
        public TextMeshProUGUI textMeshProObject;
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
             
        }

        private void FixedUpdate()
        {
            if (RaycastInfoEntity == Entity.Null)
            {
                return;
            }
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            RaycastInfoDebugUI raycastInfoDebugUi = entityManager.GetComponentData<RaycastInfoDebugUI>(RaycastInfoEntity);
            textMeshProObject.text = $"Raycast:{PointToString(raycastInfoDebugUi.hitPos)} BlockPosition:{PointToString(raycastInfoDebugUi.blockPos)}\n" +
                                     $"hit {raycastInfoDebugUi.hitSide}";
        }
        
        
        private string PointToString(float3 point)
        {
            return $"({point.x:F2},{point.y:F2},{point.z:F2})";
        }
    }
}