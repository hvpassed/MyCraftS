using System.IO;
using MyCraftS.Block;
using MyCraftS.Block.Utils;
using MyCraftS.Setting;
using ProtoBuf;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Test
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class TestSerializeSystem : SystemBase
    {
        private EntityQuery _query;
        protected override void OnCreate()
        {
            this.Enabled = false;
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAspect<BlockInfoAspect>()
                .WithNone<BlockPrefabType>()
                .Build(this);
        }

        protected override void OnUpdate()
        {

            if (_query.IsEmpty)
            {
                return;
            }

            var entities = _query.ToEntityArray(Allocator.Temp);
            if (entities.Length < 2)
            {
                return;
            }
            
            var entity1 = entities[0];
            var entity2 = entities[1];
            BlockSerializationData[] datas = new BlockSerializationData[2];
            datas[0] = BlockSerialization.GetSerializationData(EntityManager,entity1);
            datas[1] = BlockSerialization.GetSerializationData(EntityManager, entity2);
            Debug.Log($"Original:{datas[0]}\n{datas[1]}");
            string savepath = Path.Combine(SettingManager.ChunkSaveDir, "test.bin");
            //BlockSerialization.Serialize(datas,savepath);
            using (FileStream fs = File.OpenRead(savepath))
            {
                
                var ds = Serializer.Deserialize<BlockSerializationData[]>(fs);
                //var ds2 = Serializer.Deserialize<BlockSerializationData>(fs);
                
                Debug.Log($"After : {ds[0]} {ds[1]}");
            }
            
            
            this.Enabled = false;
        }
    }
}