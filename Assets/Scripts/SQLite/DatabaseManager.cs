using System;
using System.IO;

using MyCraftS.Database.Model;
using MyCraftS.Setting;
using Mono.Data.Sqlite;
using UnityEngine;


namespace MyCraftS.Database
{
    public static class DatabaseManager
    {
        public static SqliteConnection GlobalDatabase { get; private set; }
        public static SqliteConnection GameDatabase { get; private set; }

        static DatabaseManager()
        {
            try
            {
                GlobalDatabase = new SqliteConnection($"URI=file:{SettingManager.WorldDatabaseDir}");
                GlobalDatabase.Open();
                Debug.Log("Database Connected");
                CreateWorldTable(SQL: @"
                    create table IF NOT EXISTS WorldInfo 
                    (
                    Name    TEXT
                    constraint WorldInfo_pk
                    primary key,
                    Seed    integer not null,
                    SaveDir TEXT    not null
                    );
                    ");
                string floder = Path.Combine(Application.persistentDataPath, "saves",
                    SettingManager.GameSetting.WorldName);
                WorldInfoModel worldInfoModel = new WorldInfoModel(SettingManager.GameSetting.WorldName, GetLastTwoLevels(floder),
                    (int) SettingManager.GameSetting.Seed);
                WorldInfoModel.Upsert(worldInfoModel,GlobalDatabase);

                if (!Directory.Exists(floder))
                {
                    Directory.CreateDirectory(floder);
                }

                string databasePath = Path.Combine(floder, "data.db");
                GameDatabase = new SqliteConnection($"URI=file:{databasePath}");
                GameDatabase.Open();
                CreateChunkDataTble(SQL:@"
                create table IF NOT EXISTS ChunkInfo
                (
                    X          integer not null,
                    Y          integer not null,
                    ChunkID    integer not null,
                    BlocksData BLOB    not null,
                    constraint ChunkInfo_pk
                        primary key (X, Y)
                );
                ");
                ExecuteSQL(SQL:@"
                create table IF NOT EXISTS GameInfo
                (
                    Key     integer not null
                        constraint GameInfo_pk
                            primary key,
                    ChunkID integer not null
                );

                ",GameDatabase);
                
                


            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log("Database Open failed\nApplication will quit");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }


        private static void CreateChunkDataTble(string SQL)
        {
            try
            {
                using (var cmd = GameDatabase.CreateCommand())
                {
                    cmd.CommandText = SQL;
                    cmd.ExecuteNonQuery();
                    Debug.Log("Table ChunkData Created or Existed");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError("Table creation failed");
                
            }
        }
        private static void CreateWorldTable(string SQL)
        {
            try
            {
                using (var cmd = GlobalDatabase.CreateCommand())
                {
                    cmd.CommandText = SQL;
                    cmd.ExecuteNonQuery();
                    Debug.Log("Table WorldInfo Created or Existed");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError("Table creation failed");
                
            }
        }


        private static void ExecuteSQL(string SQL,SqliteConnection connection)
        {
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = SQL;
                    cmd.ExecuteNonQuery();
                     
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError($"SQL :{SQL} execute failed");
                
            }
        }
        public static void DatabaseInitialize()
        {
            Debug.Log("Database Initialized");
        }
        
        static string GetLastTwoLevels(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
        
            if (dirInfo.Parent != null && dirInfo.Parent.Parent != null)
            {
                string lastTwoLevels = Path.Combine(dirInfo.Parent.Name, dirInfo.Name);
                return lastTwoLevels;
            }
            else if (dirInfo.Parent != null)
            {
                return Path.Combine(dirInfo.Parent.Name, dirInfo.Name);
            }
            else
            {
                return dirInfo.Name;
            }
        }
    }
    
    
    
}