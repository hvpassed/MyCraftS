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
    }
}
