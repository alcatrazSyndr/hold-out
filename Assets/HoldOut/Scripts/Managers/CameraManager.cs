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

            _isSetup = true;
        }

        private void Update()
        {
            if (Ready && _cameraFollowTarget != null)
            {
                _cameraRootTransform.position = Vector3.Lerp(_cameraRootTransform.position, _cameraFollowTarget.position, Time.deltaTime * _cameraFollowSmoothing);
            }
        }

        public void SetCameraFollowTarget(Transform followTarget)
        {
            _cameraFollowTarget = followTarget;
        }
    }
}
