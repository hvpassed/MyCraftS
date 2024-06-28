using System;
using System.IO;
using MyCraftS.Chunk;
using MyCraftS.Setting;
using ProtoBuf;
using ProtoBuf.Serializers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace MyCraftS.Block.Utils
{
 
    
    
    
    
    [ProtoContract]
    public struct BlockSerializationData
    {
        [ProtoMember(1)]
        public bool disableRendering;
        [ProtoMember(2)]
        public int blockId;
        [ProtoMember(3)]
        public int3 chunkPosition;

        [ProtoMember(4)] 
        public int3 localPosition;

        public override string ToString()
        {
            return
                $"DisableRendering:{disableRendering} BlockID:{blockId} ChunkPosition:({chunkPosition.x},{chunkPosition.y},{chunkPosition.z}) " +
                $"LocalPosition:({localPosition.x},{localPosition.y},{localPosition.z})";
        }
    }
    
    
    public static class BlockSerialization
    {
        public static BlockSerializationData GetSerializationData(EntityManager entityManager,Entity BlockEntity)
        {
            BlockInfoAspect blockInfoAspect = entityManager.GetAspect<BlockInfoAspect>(BlockEntity);
            BlockSerializationData data = new BlockSerializationData()
            {
                chunkPosition = blockInfoAspect.GetChunkPosition(),
                blockId = blockInfoAspect.BlockID
            };
            if (entityManager.HasComponent<DisableRendering>(BlockEntity))
            {
                data.disableRendering = true;
            }
            else
            {
                data.disableRendering = false;
            }
            return data;
        }

        public static byte[] Serialize(BlockSerializationData [] datas)
        {
            //var dir = Path.Combine(SettingManager.BaseSaveDir, "test.bin");
            // var memStream = new MemoryStream();
            // using (var file =File.Create(path))
            // {
            //     Debug.Log("Serializing data");
            //      Serializer.Serialize(file,datas);
            //      file.SetLength(file.Position);
            // }
            using (var memStream = new MemoryStream())
            {
                Serializer.Serialize(memStream,datas);
                return memStream.ToArray();
            }
        }
    }
}