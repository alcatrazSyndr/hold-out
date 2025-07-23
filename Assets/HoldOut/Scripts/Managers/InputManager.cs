using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldOut
{
    public class InputManager : Manager<InputManager>
    {
        [Header("Input Actions")]
        [SerializeField] private InputAction _backInput = new InputAction();
        [SerializeField] private InputAction _movementInput = new InputAction();

        #region Setup

        protected override void Setup()
        {
            base.Setup();

            SetupBackInput();
            SetupMovementInput();

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
    }
}
