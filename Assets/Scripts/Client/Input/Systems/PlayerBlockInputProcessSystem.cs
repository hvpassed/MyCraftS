using Client.SystemManage;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyCraftS.Input
{
    [UpdateInGroup(typeof(PlayerInputSystemGroup))]
    public partial class PlayerBlockInputProcessSystem:SystemBase
    {
        public InputActionAsset inputActionAsset;
        public InputAction _mouseLeft, _mouseRight;

        public static Entity destroyEntity, placeEntity;
        private bool mouseLeftDown, mouseRightDown;
        protected override void OnCreate()
        {
            SystemManager.GameInitialedEvent += StartSystem;
 
            this.Enabled = false;
        }

        private void StartSystem()
        {            
            inputActionAsset = Resources.Load<InputActionAsset>("Input/PlayerInputSystemActions");
            RegisterMouseLeft();
            RegisterMouseRight();
            this.Enabled = true;
        }

        #region  Mouse_Left

        private void RegisterMouseLeft()
        {
            _mouseLeft = inputActionAsset.FindActionMap("Player").FindAction("Destroy");
            _mouseLeft.performed += MouseLeftPress;
            _mouseLeft.canceled += MouseLeftUp;
        }

        private void MouseLeftPress(InputAction.CallbackContext obj)
        {
            if (!mouseRightDown)
            {
                mouseLeftDown = true;
            }

        }

        private void MouseLeftUp(InputAction.CallbackContext ctx)
        {
            mouseLeftDown = false;
 
        }

        #endregion

        #region Mose_Right

        
        private void RegisterMouseRight()
        {
            _mouseRight = inputActionAsset.FindActionMap("Player").FindAction("Place");
            _mouseRight.performed += MouseRightPress;
            _mouseRight.canceled += MouseRightUp;
        }

        private void MouseRightPress(InputAction.CallbackContext obj)
        {
            if (!mouseLeftDown)
            {
                mouseRightDown = true;
            }

        }

        private void MouseRightUp(InputAction.CallbackContext ctx)
        {
            mouseRightDown = false;
 
        }

        #endregion
        
        protected override void OnUpdate()
        {
            EntityManager.SetComponentEnabled<DestroyAction>(destroyEntity, mouseLeftDown);
            EntityManager.SetComponentEnabled<PlaceAction>(placeEntity, mouseRightDown);
        }
    }
}