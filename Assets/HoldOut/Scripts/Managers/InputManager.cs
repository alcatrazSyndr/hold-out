using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldOut
{
    public class InputManager : Manager<InputManager>
    {
        [Header("Input Actions")]
        [SerializeField] private InputAction _backInput = new InputAction();
        [SerializeField] private InputAction _movementInput = new InputAction();
        [SerializeField] private InputAction _cameraScrollInput = new InputAction();
        [SerializeField] private InputAction _primaryAttackInput = new InputAction();

        #region Setup

        protected override void Setup()
        {
            base.Setup();

            SetupBackInput();
            SetupMovementInput();
            SetupCameraScrollInput();
            SetupPrimaryAttackInput();

            _isSetup = true;
        }

        #endregion

        #region Back Input Logic

        private void SetupBackInput()
        {
            _backInput.performed += BackInputPerformedHandler;
            ToggleBackInput(true);
        }

        private void ToggleBackInput(bool toggle)
        {
            if (toggle)
            {
                _backInput.Enable();
            }
            else
            {
                _backInput.Disable();
            }
        }

        private void BackInputPerformedHandler(InputAction.CallbackContext obj)
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.RaiseBackInput();
            }
        }

        #endregion

        #region Movement Input Logic

        private void SetupMovementInput()
        {
            _movementInput.started += MovementInputChangedHandler;
            _movementInput.performed += MovementInputChangedHandler;
            _movementInput.canceled += MovementInputChangedHandler;
            ToggleMovementInput(true);
        }

        private void ToggleMovementInput(bool toggle)
        {
            if (toggle)
            {
                _movementInput.Enable();
            }
            else
            {
                _movementInput.Disable();
            }
        }

        private void MovementInputChangedHandler(InputAction.CallbackContext obj)
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.RaiseMovementInputChange(obj.ReadValue<Vector2>());
            }
        }

        #endregion

        #region Camera Scroll Input Logic

        private void SetupCameraScrollInput()
        {
            _cameraScrollInput.performed += CameraScrollInputChangedHandler;
            ToggleCameraScrollInput(true);
        }

        private void ToggleCameraScrollInput(bool toggle)
        {
            if (toggle)
            {
                _cameraScrollInput.Enable();
            }
            else
            {
                _cameraScrollInput.Disable();
            }
        }

        private void CameraScrollInputChangedHandler(InputAction.CallbackContext obj)
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.RaiseCameraScrollInputChange(obj.ReadValue<Vector2>().y);
            }
        }

        #endregion

        #region Primary Attack Input Logic

        private void SetupPrimaryAttackInput()
        {
            _primaryAttackInput.started += PrimaryAttackInputStartedHandler;
            _primaryAttackInput.canceled += PrimaryAttackInputCanceledHandler;
            TogglePrimaryAttackInput(true);
        }

        private void TogglePrimaryAttackInput(bool toggle)
        {
            if (toggle)
            {
                _primaryAttackInput.Enable();
            }
            else
            {
                _primaryAttackInput.Disable();
            }
        }

        private void PrimaryAttackInputStartedHandler(InputAction.CallbackContext obj)
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.RaisePrimaryAttackInputChange(true);
            }
        }

        private void PrimaryAttackInputCanceledHandler(InputAction.CallbackContext obj)
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.RaisePrimaryAttackInputChange(false);
            }
        }

        #endregion
    }
}
