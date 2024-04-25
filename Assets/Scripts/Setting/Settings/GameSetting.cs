using UnityEngine;

namespace MyCraftS.Setting
{
    [CreateAssetMenu(fileName = "GameSetting",menuName = "MyCraftS/Setting/GameSetting")]
    public class GameSetting:ScriptableObject
    {
        public float Gravity = -9.8f;
    }
}