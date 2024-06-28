using System.IO;
using UnityEngine;

namespace MyCraftS.Setting
{
    public static class SettingManager
    {

        public static PlayerSetting PlayerSetting;
        public static GameSetting GameSetting;

        public static SaveSetting SaveSetting;
        public static bool DebugMode = false;

        public static string BaseSaveDir;

        public static string ChunkSaveDir;

        public static string WorldDatabaseDir;
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
            SaveSetting = Resources.Load<SaveSetting>("Setting/SaveSetting");
            if (SaveSetting == null)
            {
                SaveSetting = ScriptableObject.CreateInstance<SaveSetting>();
                SaveSetting.ChunkSave = "ChunkSave";
            }

            BaseSaveDir = Path.Combine(Application.persistentDataPath, "saves", GameSetting.WorldName);
            WorldDatabaseDir = Path.Combine(Application.persistentDataPath,"saves", "database.db");
            if (!Directory.Exists(BaseSaveDir))
            {
                Directory.CreateDirectory(BaseSaveDir);
            }
            ChunkSaveDir = Path.Combine(BaseSaveDir,SaveSetting.ChunkSave);
            if (!Directory.Exists(ChunkSaveDir))
            {
                Directory.CreateDirectory(ChunkSaveDir);
            }
        }
    }
}