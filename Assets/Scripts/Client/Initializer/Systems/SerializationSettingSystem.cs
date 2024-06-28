using MyCraftS.Utils;
using ProtoBuf.Meta;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace MyCraftS.Initializer
{
    [UpdateInGroup(typeof(OtherInitializeSystemGroup))]
    public partial class SerializationSettingSystem:SystemBase
    {
        protected override void OnCreate()
        {
            
            RuntimeTypeModel.Default.Add(typeof(int3),false).SetSurrogate(typeof(Int3Surrogate));
            this.Enabled = false;
        }

        protected override void OnUpdate()
        {
             
        }
    }
}