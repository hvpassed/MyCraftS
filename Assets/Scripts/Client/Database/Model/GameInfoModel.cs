using System;
using Mono.Data.Sqlite;
using UnityEngine;

namespace MyCraftS.Database.Model
{
    public class GameInfoModel
    {
        public int ChunkID;
        public int Key = 0;
        public GameInfoModel(int _chunkID)
        {
            ChunkID = _chunkID;
        }
        public static GameInfoModel Read(SqliteConnection connection)
        {
            var sql = @"SELECT * FROM GameInfo";
                 
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int chunkID = Convert.ToInt32(reader["ChunkID"]);
                        
                        return new GameInfoModel(chunkID);
                    }
                }
                     
            }

            var insertSql = @"INSERT INTO GameInfo (Key,ChunkID) VALUES (0,0)";
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = insertSql;
                cmd.ExecuteNonQuery();
            }
            return new GameInfoModel(0);
        }
        
        public static void Update(int newVal,SqliteConnection connection)
        {
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE GameInfo SET ChunkID = @ChunkID WHERE Key = 0";
                    cmd.Parameters.AddWithValue("@ChunkID", newVal);
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