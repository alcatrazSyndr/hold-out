using UnityEngine;

namespace HoldOut
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _lowerRootRotationSmoothing = 1f;
        [SerializeField] private float _upperRootRotationSmoothing = 1f;
        [SerializeField] private float _movementAccelerationSmoothing = 1f;
        [SerializeField] private float _movementVelocityDeadzone = 1f;
        [SerializeField] private float _movementVelocityModifier = 1f;
        [SerializeField] private float _cameraFollowTargetLerpAmount = 0.15f;
        [SerializeField] private float _cameraFollowTargetDistanceLimit = 15f;

        [Header("Components")]
        [SerializeField] private CharacterController _characterController = null;
        [SerializeField] private Transform _lowerRootTransform = null;
        [SerializeField] private Transform _upperRootTransform = null;
        [SerializeField] private Transform _gunLevelTransform = null;
        [SerializeField] private Transform _cameraFollowTargetTransform = null;

        [Header("Runtime")]
        [SerializeField] private Vector3 _currentTargetMovementVelocity = Vector3.zero;
        [SerializeField] private Vector3 _currentMovementVelocity = Vector3.zero;

        private void Update()
        {
            if (Time.timeScale <= 0f)
            {
                return;
            }

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

                _upperRootTransform.rotation = Quaternion.Lerp(_upperRootTransform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _upperRootRotationSmoothing);

                Vector3 fromPosition = _lowerRootTransform.position;
                Vector3 toPosition = hitPoint;
                toPosition.y = 0f;

                Vector3 offset = toPosition - fromPosition;
                float maxDistance = _cameraFollowTargetDistanceLimit;

                if (offset.magnitude > maxDistance)
                {
                    offset = offset.normalized * maxDistance;
                    toPosition = fromPosition + offset;
                }

                _cameraFollowTargetTransform.position = Vector3.Lerp(fromPosition, toPosition, _cameraFollowTargetLerpAmount);
            }

            _currentMovementVelocity = Vector3.Lerp(_currentMovementVelocity, _currentTargetMovementVelocity, Time.timeScale * Time.deltaTime * _movementAccelerationSmoothing);

            if (_currentMovementVelocity.sqrMagnitude >= _movementVelocityDeadzone)
            {
                _characterController.Move(_currentMovementVelocity * _movementVelocityModifier * Time.deltaTime * Time.timeScale);
            }
        }

        private void OnEnable()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.OnMovementInputChanged += MovementInputChangeEventHandler;
            }

            if (CameraManager.Instance != null && CameraManager.Instance.Ready)
            {
                CameraManager.Instance.SetCameraFollowTarget(_cameraFollowTargetTransform);
            }
        }

        private void OnDisable()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.OnMovementInputChanged -= MovementInputChangeEventHandler;
            }

            if (CameraManager.Instance != null && CameraManager.Instance.Ready)
            {
                CameraManager.Instance.SetCameraFollowTarget(null);
            }
        }

        private void MovementInputChangeEventHandler(Vector2 movementInput)
        {
            _currentTargetMovementVelocity = new Vector3(movementInput.x, 0f, movementInput.y);
        }
    }
}
