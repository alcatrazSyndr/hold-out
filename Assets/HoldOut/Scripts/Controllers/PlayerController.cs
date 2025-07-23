using UnityEngine;

namespace HoldOut
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _lowerRootRotationSmoothing = 1f;

        [Header("Components")]
        [SerializeField] private CharacterController _characterController = null;
        [SerializeField] private Transform _lowerRootTransform = null;
        [SerializeField] private Transform _upperRootTransform = null;

        [Header("Runtime")]
        [SerializeField] private Vector2 _currentMovementInput = Vector2.zero;
        [SerializeField] private Vector3 _lastTargetMovementDirection = Vector3.zero;

        private void Update()
        {
            if (_lastTargetMovementDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(_lowerRootTransform.position + _lastTargetMovementDirection);
                _lowerRootTransform.rotation = Quaternion.Lerp(_lowerRootTransform.rotation, targetRotation, Time.deltaTime * _lowerRootRotationSmoothing);
            }
        }

        private void OnEnable()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.OnMovementInputChanged += MovementInputChangeEventHandler;
            }
        }

        private void OnDisable()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.OnMovementInputChanged -= MovementInputChangeEventHandler;
            }
        }

        private void MovementInputChangeEventHandler(Vector2 movementInput)
        {
            if (_currentMovementInput != movementInput)
            {
                _currentMovementInput = movementInput;
            }

            if (movementInput != Vector2.zero)
            {
                _lastTargetMovementDirection = new Vector3(movementInput.x, 0f, movementInput.y);
            }
        }
    }
}
