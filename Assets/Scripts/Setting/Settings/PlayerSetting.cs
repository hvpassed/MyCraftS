using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace MyCraftS.Setting
{
    [CreateAssetMenu(fileName = "PlayerSetting",menuName = "MyCraftS/Setting/PlayerSetting")]
    public class PlayerSetting:ScriptableObject
    {
        public  int ViewDistance = 2;

        public float RunSpeed = 5f;
        public float WalkSpeed = 2f;
        
        public float MouseSensitivity = 2f;


        public float minAngle = -87f;
        public float maxAngle = 87f;
        
        
        public float3 CameraOffset = new float3(0,2f,0);
        
        
        public float rayDistance = 5f;

        public float PlaceGapTime = 0.15f;
    }
}
