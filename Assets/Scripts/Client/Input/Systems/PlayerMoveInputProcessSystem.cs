using System.Collections.Generic;
using Client.SystemManage;
using MyCraftS.Input;
using MyCraftS.Physic;
using MyCraftS.Player;
using MyCraftS.Player.Data;
using MyCraftS.Setting;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyCraftS.Input
{
    [UpdateInGroup(typeof(PlayerInputSystemGroup),OrderFirst = true)]
    public partial class PlayerMoveInputProcessSystem:SystemBase
    {
        public InputActionAsset inputActionAsset;
 
        public Entity playerEntity;
        public Entity cameraEntity;
        
        private InputAction _moveAction,_mouseMoveAction,_debugAction,_debugShowChunkBlocksAction,_runAction,_jumpAction;

        

        private float _xTargetSpeed, _zTargetSpeed;

        private Vector3 Speed;
        private bool isRun,wantJump;
        private float mouseX,mouseY;
    
        
        protected override void OnCreate()
        {
            

            SystemManager.GameInitialedEvent += StartSystem;
            this.Enabled = false;
        }

        public void StartSystem()
        {     
            Debug.Log("PlayerMoveInputProcessSystem OnCreate");
            inputActionAsset = Resources.Load<InputActionAsset>("Input/PlayerInputSystemActions");
            isRun = false;
            wantJump = false;
            Speed = new float3(0, 0, 0);
            RegisterMove();
            RegisterMouseLook();
            RegisterJump();
            
            
            
            RegisterDebug();
            RegisterDebugShowChunkBlocks();

            float2 screenCenter = new float2(Screen.width / 2, Screen.height / 2);
            Mouse.current.WarpCursorPosition(screenCenter);
            // 确保鼠标状态是更新的
            InputSystem.Update();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerEntity = PlayerDataContainer.playerEntity;
            cameraEntity = PlayerDataContainer.cameraEntity;
            this.Enabled = true;
        }

        private float jumpSpeed()
        {
            return math.sqrt(math.abs(SettingManager.GameSetting.Gravity) *
                             math.abs(SettingManager.GameSetting.jumpHeight) * 2);
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
            var transform = EntityManager.GetComponentData<LocalTransform>(playerEntity);
            var PhysicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(playerEntity);
            var xRotationComponent = EntityManager.GetComponentData<CameraStatus>(cameraEntity);
            var xr = xRotationComponent.xRotation;

            Speed.y = PhysicsVelocity.Linear.y;
            float3 targetSpeed = Move(transform);
            PhysicsVelocity.Linear = targetSpeed;
            if (SettingManager.DebugMode)
            {
                float3 dir = math.normalize(new float3(PhysicsVelocity.Linear.x, 0, PhysicsVelocity.Linear.z));
                
                Debug.DrawRay(transform.Position + new float3(0, 1, 0), 
                    dir*5, Color.blue);
 
            }
            EntityManager.SetComponentData(playerEntity,PhysicsVelocity);
            xr = MoveMouse(out float3 playerRotate,  xr);
            
            transform.Rotation = math.mul(transform.Rotation, quaternion.Euler(playerRotate));
            EntityManager.SetComponentData(playerEntity,transform);
            xRotationComponent.xRotation = xr;
            EntityManager.SetComponentData(cameraEntity, xRotationComponent);
            mouseX = 0;
            mouseY = 0;
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
            float2 mouseDelta = context.ReadValue<Vector2>();

            mouseX += mouseDelta.x;
            mouseY += mouseDelta.y;


        }
        
        
        private void OnMouseLookCanceled(InputAction.CallbackContext context)
        {
            
        }

        private float MoveMouse(out float3 playerRotate,  float xr)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            float deltaX = mouseX*deltaTime*SettingManager.PlayerSetting.MouseSensitivity;
            float deltaY = mouseY*deltaTime*SettingManager.PlayerSetting.MouseSensitivity*100;
            playerRotate = (new float3(0, 1, 0)) * deltaX;
            xr = math.clamp(xr - deltaY, SettingManager.PlayerSetting.minAngle, SettingManager.PlayerSetting.maxAngle);
            return xr;

        }
        #endregion

        #region Player_Jump
        
        private void RegisterJump()
        {
            _jumpAction = inputActionAsset.FindActionMap("Player").FindAction("Jump");
            _jumpAction.performed += OnJumpPerformed;
            
        }

        private void OnJumpPerformed(InputAction.CallbackContext obj)
        {
            if (CheckGrounded())
            {
                wantJump = true;
            }
        }

        #endregion
        
        #region Player_Move

        private float3 Move(LocalTransform transform )
        {
            float3 cameraForward = transform.Forward();
            float3 cameraRight = transform.Right();

            if (SettingManager.DebugMode)
            {
                Debug.DrawRay(transform.Position+new float3(0,2,0), cameraForward * 5, Color.blue);
                
                Debug.DrawRay(transform.Position+new float3(0,2,0), cameraRight * 5, Color.red);
                
            }
            float3 targetSpeed = new float3(0,0,0) ;
            
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraRight = math.normalize(cameraRight);
            cameraForward = math.normalize(cameraForward);
            
            
            
            Speed.z = math.lerp(Speed.z, _zTargetSpeed, 0.5f);
            Speed.x = math.lerp(Speed.x, _xTargetSpeed, 0.5f);
            
            targetSpeed = cameraForward * Speed.z + cameraRight * Speed.x;//修正方向

            targetSpeed.y = Speed.y;
            if (wantJump)
            {
                targetSpeed+= new float3(0,jumpSpeed(),0);
                wantJump = false;
            }
            
            return targetSpeed;


        }

        private bool CheckGrounded()
        {
            return EntityManager.IsComponentEnabled(playerEntity, typeof(IsGrounded));
        }
        private void RegisterMove()
        {
            _moveAction = inputActionAsset.FindActionMap("Player").FindAction("Move");
            _moveAction.performed += OnMovePerformed;
            _moveAction.canceled += OnMoveCanceled; 
            _runAction = inputActionAsset.FindActionMap("Player").FindAction("Run");
            _runAction.performed += OnRunPerformed;
            _runAction.canceled += OnRunCanceled;
        } 
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            // 处理移动逻辑
            _zTargetSpeed = moveInput.y*(isRun?SettingManager.PlayerSetting.RunSpeed:SettingManager.PlayerSetting.WalkSpeed);
            _xTargetSpeed = moveInput.x*(isRun?SettingManager.PlayerSetting.RunSpeed:SettingManager.PlayerSetting.WalkSpeed);
            
             
        }
        
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _zTargetSpeed = 0f;
            _xTargetSpeed = 0f;
        }
        
        private void OnRunPerformed(InputAction.CallbackContext context)
        {
            // 处理跑步逻辑
            isRun = true;
            _zTargetSpeed = _zTargetSpeed * SettingManager.PlayerSetting.RunSpeed / SettingManager.PlayerSetting.WalkSpeed;
            _xTargetSpeed = _xTargetSpeed * SettingManager.PlayerSetting.RunSpeed / SettingManager.PlayerSetting.WalkSpeed;
            
            
        }
        private void OnRunCanceled(InputAction.CallbackContext context)
        {   
            isRun = false;
            _zTargetSpeed = _zTargetSpeed * SettingManager.PlayerSetting.WalkSpeed / SettingManager.PlayerSetting.RunSpeed;
            _xTargetSpeed = _xTargetSpeed * SettingManager.PlayerSetting.WalkSpeed / SettingManager.PlayerSetting.RunSpeed;
        }
        
        
        
        
        #endregion
    }
}