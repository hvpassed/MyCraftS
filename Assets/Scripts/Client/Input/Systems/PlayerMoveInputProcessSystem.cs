using System.Collections.Generic;
using Client.SystemManage;
using MyCraftS.Input;
using MyCraftS.Player;
using MyCraftS.Setting;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Client.Input.Systems
{
    [UpdateInGroup(typeof(PlayerInputSystemGroup),OrderFirst = true)]
    public partial class PlayerMoveInputProcessSystem:SystemBase
    {
        public InputActionAsset inputActionAsset;
        public EntityQuery _playerQuery;
        public Entity playerEntity;
        private InputAction _moveAction,_mouseMoveAction,_debugAction,_debugShowChunkBlocksAction;
        
        
        
         
        
        
        
        protected override void OnCreate()
        {
            Debug.Log("PlayerMoveInputProcessSystem OnCreate");
            inputActionAsset = Resources.Load<InputActionAsset>("Input/PlayerInputSystemActions");
            RegisterMove();
            RegisterMouseLook();
            RegisterDebug();
            RegisterDebugShowChunkBlocks();
            _playerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerType>()
                .Build(this);
        }

        #region Debug_ShowChunkBlocks

        private void RegisterDebugShowChunkBlocks()
        {
            _debugShowChunkBlocksAction = inputActionAsset.FindActionMap("Player").FindAction("ConfigureCollision");
            _debugShowChunkBlocksAction.performed += OnDebugShowChunkBlocksPerformed;
        }
        
        private void OnDebugShowChunkBlocksPerformed(InputAction.CallbackContext context)
        {
            SystemManager.StartOnceDebugSystem("ConfigureCollision");
        }
        #endregion
        
        
        #region Debug

        private void RegisterDebug()
        {
            _debugAction = inputActionAsset.FindActionMap("Player").FindAction("Switch");
            _debugAction.performed += OnDebugPerformed;
        }
        
        private void OnDebugPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("press debug key");
            SettingManager.DebugMode = !SettingManager.DebugMode;
            if (SettingManager.DebugMode)
            {
                SystemManager.StartDebugSystem();
            }
            else
            {
                SystemManager.CloseDebugSystem();
            }
        }
        #endregion
        protected override void OnUpdate()
        {
            if (playerEntity == Entity.Null)
            {
                if (_playerQuery.CalculateEntityCount() != 0)
                {
                    playerEntity = _playerQuery.ToEntityArray(Allocator.Temp)[0];
                }
                else
                {
                    return;
                }
            }
            
            
        }

        #region Player_Mouse_Move
        private void RegisterMouseLook()
        {
            _moveAction = inputActionAsset.FindActionMap("Player").FindAction("Look");
            _moveAction.performed += OnMouseLookPerformed;
            _moveAction.canceled += OnMouseLookCanceled; 
        }
        
        private void OnMouseLookPerformed(InputAction.CallbackContext context)
        {
            Vector2 lookInput = context.ReadValue<Vector2>();
            
        }
        
        
        private void OnMouseLookCanceled(InputAction.CallbackContext context)
        {
            
        }
        #endregion
        
        
        
        #region Player_Move

        private void RegisterMove()
        {
            _moveAction = inputActionAsset.FindActionMap("Player").FindAction("Move");
            _moveAction.performed += OnMovePerformed;
            _moveAction.canceled += OnMoveCanceled; 
        } 
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            // 处理移动逻辑
             
        }
        
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
 
        }
        #endregion
    }
}