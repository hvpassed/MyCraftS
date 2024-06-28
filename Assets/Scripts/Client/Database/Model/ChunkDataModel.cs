using System;
using Mono.Data.Sqlite;
using UnityEngine;

namespace MyCraftS.Database.Model
{
    public class ChunkDataModel
    {
        public int X, Z;
        public int ChunkID;
        public byte[] BlocksData;

        public static void Upsert(ChunkDataModel data,SqliteConnection connection)
        {
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    string sql = @"INSERT OR REPLACE INTO ChunkInfo (X,Y,ChunkID,BlocksData)
                                        VALUES (@X,@Y,@ChunkID,@BlocksData)";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@X", data.X);
                    cmd.Parameters.AddWithValue("@Y", data.Z);
                    cmd.Parameters.AddWithValue("@ChunkID", data.ChunkID);
                    cmd.Parameters.AddWithValue("@BlocksData", data.BlocksData);
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