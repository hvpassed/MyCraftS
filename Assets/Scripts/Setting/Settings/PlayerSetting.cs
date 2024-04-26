using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
