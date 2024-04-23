using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using MyCraftS.Chunk.Data;
using MyCraftS.Config;
using UnityEngine;
namespace MyCraftS.Chunk.Manage
{
    [UpdateInGroup(typeof(ChunkInitializeSystemGroup))]
    public partial class ChunkManageEntityCreateSystem : SystemBase
    {
        bool isInit = true;
        Entity ChunkEntity;
        protected override void OnCreate()
        {
            base.OnCreate();
            Entity ChunkEntity = EntityManager.CreateEntity();
            ChunkDataContainer.ChunkManager = ChunkEntity;
            EntityManager.AddComponentData(ChunkEntity,new ChunkNotLoaded()
            {
                waitForLoaded = new NativeHashSet<int3>(TerrianConfig.MaxLoadedChunk,Allocator.Persistent)
            });

            EntityManager.AddComponentData(ChunkEntity, new ChunkLoaded()
            {
                LoadedSet = new NativeHashSet<int3>(TerrianConfig.MaxLoadedChunk, Allocator.Persistent)
            });
            Debug.Log("ChunkManager Created");
        }

        protected override void OnUpdate()
        {
            if (isInit)
            {
                isInit = false;

            }
            this.Enabled = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //EntityManager.GetComponentData<ChunkNotLoaded>(ChunkEntity).waitForLoaded.Dispose();
            //EntityManager.GetComponentData<ChunkLoaded>(ChunkEntity).LoadedSet.Dispose();
            EntityManager.DestroyEntity(ChunkEntity);
        }
    }
}
