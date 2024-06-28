 
using System.Collections.Generic;
using MyCraftS.Time;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace MyCraftS.SystemManage
{
    public enum ManagedSystem
    {
        TickSystemGroup
    }
    
    
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SystemManager:SystemBase
    {
        public static int TickSystemDependency = 2;
        private static int TickSystemDependencyCount = 0;
        private static TickUpdateGroup tickUpdateGroup;
        
        private static int AllManaged = 2;
        private static int started = 0;

        #region  Game_Initial

        public delegate void SystemStartDelegate();

        public static SystemStartDelegate GameInitialedEvent=null;

        public static void GameInitialized()
        {
            if (GameInitialedEvent!= null)
            {
                GameInitialedEvent();
            }
        }

        #endregion

        
        
        
        #region Debug_Event

        


        public delegate void DebugSystemStartDelegate();
        public delegate void DebugSystemCloseDelegate();

        public delegate void DebugSystemStartOnceDelegate(string pressKey);
        public static DebugSystemStartDelegate DebugSystemStartEvent=null;
        
        public static DebugSystemCloseDelegate DebugSystemCloseEvent=null;

        public static DebugSystemStartOnceDelegate DebugSystemStartOnceEvent = null;

        public static void StartOnceDebugSystem(string pressKey)
        {
            if (DebugSystemStartOnceEvent != null)
            {
                DebugSystemStartOnceEvent(pressKey);
            }
            
        }
        public static void StartDebugSystem()
        {
            if (DebugSystemStartEvent != null)
                DebugSystemStartEvent();
        }
        
        public static void CloseDebugSystem()
        {
            if (DebugSystemCloseEvent != null)
                DebugSystemCloseEvent();
        }
        #endregion
        protected override void OnUpdate()
        {
            if (tickUpdateGroup == null)
            {
                tickUpdateGroup = TickUpdateGroup.Instance;
            }
            
            if (started == AllManaged)
            {
                this.Enabled = false;
            }
        }
    
        
        
        
        
        public static void CanStartSystem(ManagedSystem whichSystem)
        {
            switch (whichSystem)
            {
                case ManagedSystem.TickSystemGroup:
                    TickSystemDependencyCount++;
                    if (TickSystemDependencyCount == TickSystemDependency)
                    {
                        tickUpdateGroup.Enabled = true;
                        Debug.Log("System Manager:TickUpdateGroup enable");
                        started++;
                    }
                    break;
            }

 
        }
 
    }
}