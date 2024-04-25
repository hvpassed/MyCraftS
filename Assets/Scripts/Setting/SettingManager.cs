using UnityEngine;

namespace MyCraftS.Setting
{
    public static class SettingManager
    {

        public static PlayerSetting PlayerSetting;
        public static GameSetting GameSetting;
        
        public static bool DebugMode = false;
        static SettingManager()
        {
            PlayerSetting = Resources.Load<PlayerSetting>("Setting/PlayerSetting");
            if (PlayerSetting == null)
            {
                PlayerSetting = ScriptableObject.CreateInstance<PlayerSetting>();
                PlayerSetting.ViewDistance = 2;
            }
            GameSetting = Resources.Load<GameSetting>("Setting/GameSetting");
            if (GameSetting == null)
            {
                GameSetting = ScriptableObject.CreateInstance<GameSetting>();
                GameSetting.Gravity = -9.8f;
            }
            
        }
    }
}