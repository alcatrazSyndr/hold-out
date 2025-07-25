using UnityEngine;

namespace HoldOut
{
    public class CameraManager : Manager<CameraManager>
    {
        [Header("Data")]
        [SerializeField] private float _cameraFollowSmoothing = 1f;

        [Header("Components")]
        [SerializeField] private Transform _cameraRootTransform = null;

        [Header("Runtime")]
        [SerializeField] private Transform _cameraFollowTarget = null;

        protected override void Setup()
        {
            base.Setup();

            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerControllerEvents.OnPlayerControllerSpawned += PlayerControllerSpawnedEventHandler;
                EventManager.Instance.PlayerControllerEvents.OnPlayerControllerDestroyed += PlayerControllerDestroyedEventHandler;
            }

            _isSetup = true;
        }

        private void Update()
        {
            if (Ready && _cameraFollowTarget != null)
            {
                _cameraRootTransform.position = Vector3.Lerp(_cameraRootTransform.position, _cameraFollowTarget.position, Time.deltaTime * _cameraFollowSmoothing);
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

        private void SetCameraFollowTarget(Transform followTarget)
        {
            _cameraFollowTarget = followTarget;

            if (_cameraFollowTarget == null)
            {
                _cameraRootTransform.position = Vector3.zero;
            }
        }
    }
}
