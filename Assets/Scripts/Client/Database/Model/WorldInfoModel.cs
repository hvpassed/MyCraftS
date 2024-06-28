using System;
using Mono.Data.Sqlite;
using UnityEngine;

namespace MyCraftS.Database.Model
{
    public class WorldInfoModel
    {
        public string Name,SaveDir;
		public int Seed;


        public WorldInfoModel(string name, string saveDir, int seed)
        {
            Name = name;
            SaveDir = saveDir;
            Seed = seed;
        }


        public static void Upsert(WorldInfoModel data,SqliteConnection connection)
        {
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT OR REPLACE INTO WorldInfo (Name,Seed,SaveDir)
                                        VALUES (@Name,@Seed,@SaveDir)";
                    cmd.Parameters.AddWithValue("@Name", data.Name);
                    cmd.Parameters.AddWithValue("@Seed", data.Seed);
                    cmd.Parameters.AddWithValue("@SaveDir", data.SaveDir);
                    cmd.ExecuteNonQuery();
                }
                
                
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
}