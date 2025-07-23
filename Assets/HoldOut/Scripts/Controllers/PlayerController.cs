using UnityEngine;

namespace HoldOut
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _lowerRootRotationSmoothing = 1f;
        [SerializeField] private float _movementAccelerationSmoothing = 1f;
        [SerializeField] private float _movementVelocityDeadzone = 1f;
        [SerializeField] private float _movementVelocityModifier = 1f;

        [Header("Components")]
        [SerializeField] private CharacterController _characterController = null;
        [SerializeField] private Transform _lowerRootTransform = null;
        [SerializeField] private Transform _upperRootTransform = null;
        [SerializeField] private Transform _gunLevelTransform = null;

        [Header("Runtime")]
        [SerializeField] private Vector3 _currentTargetMovementVelocity = Vector3.zero;
        [SerializeField] private Vector3 _currentMovementVelocity = Vector3.zero;

        private void Update()
        {
            if (_currentTargetMovementVelocity != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(_currentTargetMovementVelocity);
                _lowerRootTransform.rotation = Quaternion.Lerp(_lowerRootTransform.rotation, targetRotation, Time.deltaTime * _lowerRootRotationSmoothing);
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0f, _gunLevelTransform.position.y, 0f));
            if (plane.Raycast(ray, out float distance))
            {
                var hitPoint = ray.GetPoint(distance);
                var direction = hitPoint - _upperRootTransform.position;
                direction.y = 0f;

                _upperRootTransform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private void FixedUpdate()
        {
            _currentMovementVelocity = Vector3.Lerp(_currentMovementVelocity, _currentTargetMovementVelocity, Time.fixedDeltaTime * _movementAccelerationSmoothing);

            if (_currentMovementVelocity.sqrMagnitude >= _movementVelocityDeadzone)
            {
                _characterController.Move(_currentMovementVelocity * _movementVelocityModifier * Time.fixedDeltaTime);
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
            _currentTargetMovementVelocity = new Vector3(movementInput.x, 0f, movementInput.y);
        }
    }
}
