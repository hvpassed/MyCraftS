using MyCraftS.Config;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace MyCraftS.Time
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(BeginVariableRateSimulationEntityCommandBufferSystem))]
    public partial class TickUpdateGroup: ComponentSystemGroup
    {
        private float elapsedTime = 0f;
        public static TickUpdateGroup Instance;


        protected override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            this.Enabled = false;
        }
        protected override void OnUpdate()
        {
            elapsedTime += SystemAPI.Time.DeltaTime;
            if (elapsedTime >= TimeConfig.tick)
            {
                
                 
                elapsedTime -= TimeConfig.tick;
                base.OnUpdate();
            }
 
        }
    }
}