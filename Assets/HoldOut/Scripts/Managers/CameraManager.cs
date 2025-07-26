using UnityEngine;

namespace HoldOut
{
    public class CameraManager : Manager<CameraManager>
    {
        [Header("Data")]
        [SerializeField] private float _cameraFollowSmoothing = 1f;
        [SerializeField] private float _maxCameraLinearTrackOffset = 10f;
        [SerializeField] private float _cameraLinearTrackSmoothing = 1f;

        [Header("Components")]
        [SerializeField] private Transform _cameraRootTransform = null;
        [SerializeField] private Transform _cameraLinearTrackTransform = null;

        [Header("Runtime")]
        [SerializeField] private Transform _cameraFollowTarget = null;
        [SerializeField] private float _currentCameraZoom = 0f;
        [SerializeField] private float _currentCameraTargetZoom = 0f;

        protected override void Setup()
        {
            base.Setup();

            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerControllerEvents.OnPlayerControllerSpawned += PlayerControllerSpawnedEventHandler;
                EventManager.Instance.PlayerControllerEvents.OnPlayerControllerDestroyed += PlayerControllerDestroyedEventHandler;

                EventManager.Instance.PlayerInputEvents.OnCameraScrollInputChanged += ScrollCameraInputChangedEventHandler;
            }

            _isSetup = true;
        }

        private void Update()
        {
            if (Ready && _cameraFollowTarget != null)
            {
                _cameraRootTransform.position = Vector3.Lerp(_cameraRootTransform.position, _cameraFollowTarget.position, Time.deltaTime * _cameraFollowSmoothing);
                
                if (Mathf.Abs(_currentCameraZoom - _currentCameraTargetZoom) > 0.05f)
                {
                    _currentCameraZoom = Mathf.Lerp(_currentCameraZoom, _currentCameraTargetZoom, Time.deltaTime * _cameraLinearTrackSmoothing);
                    _cameraLinearTrackTransform.localPosition = new Vector3(0f, 0f, -_currentCameraZoom);
                }
            }
        }

        private void PlayerControllerSpawnedEventHandler(PlayerController playerController)
        {
            if (playerController != null)
            {
                SetCameraFollowTarget(playerController.CameraFollowTargetTransform);
            }
        }

        private void PlayerControllerDestroyedEventHandler()
        {
            SetCameraFollowTarget(null);
        }

        private void ScrollCameraInputChangedEventHandler(float scrollDelta)
        {
            if (Ready && _cameraFollowTarget != null)
            {
                _currentCameraTargetZoom = Mathf.Clamp(_currentCameraTargetZoom - scrollDelta, 0f, _maxCameraLinearTrackOffset);
            }
        }

        private void SetCameraFollowTarget(Transform followTarget)
        {
            _cameraFollowTarget = followTarget;

            if (_cameraFollowTarget == null)
            {
                _cameraRootTransform.position = Vector3.zero;
            }

            _currentCameraZoom = 0f;
            _currentCameraTargetZoom = 0f;
            _cameraLinearTrackTransform.localPosition = Vector3.zero;
        }
    }
}
