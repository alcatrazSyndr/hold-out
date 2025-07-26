using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using System.Collections;

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
        [SerializeField] private float _bulletFlightSpeed = 10f;
        [SerializeField] private float _bulletLifetime = 10f;

        [Header("Components")]
        [SerializeField] private CharacterController _characterController = null;
        [SerializeField] private Transform _lowerRootTransform = null;
        [SerializeField] private Transform _upperRootTransform = null;
        [SerializeField] private Transform _gunLevelTransform = null;
        [SerializeField] private Transform _cameraFollowTargetTransform = null;
        public Transform CameraFollowTargetTransform
        {
            get
            {
                return _cameraFollowTargetTransform;
            }
        }
        [SerializeField] private Transform _bulletFlightOriginRightTransform = null;
        [SerializeField] private Transform _bulletFlightOriginLeftTransform = null;

        [Header("Runtime")]
        [SerializeField] private Vector3 _currentTargetMovementVelocity = Vector3.zero;
        [SerializeField] private Vector3 _currentMovementVelocity = Vector3.zero;
        [SerializeField] private float3 _lastPosition = float3.zero;
        public float3 LastPosition
        {
            get
            {
                return _lastPosition;
            }
        }
        [SerializeField] private int _previousBulletOrigin = 0;

        #region Update Logic

        private void Update()
        {
            _lastPosition = transform.position;

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

        #endregion

        #region Setup/Unsetup

        private void OnEnable()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.OnMovementInputChanged += MovementInputChangeEventHandler;
                EventManager.Instance.PlayerInputEvents.OnPrimaryAttackInputChanged += PrimaryAttackInputChangeEventHandler;

                EventManager.Instance.PlayerControllerEvents.RaisePlayerControllerSpawned(this);
            }
        }

        private void OnDisable()
        {
            if (EventManager.Instance != null && EventManager.Instance.Ready)
            {
                EventManager.Instance.PlayerInputEvents.OnMovementInputChanged -= MovementInputChangeEventHandler;
                EventManager.Instance.PlayerInputEvents.OnPrimaryAttackInputChanged -= PrimaryAttackInputChangeEventHandler;

                EventManager.Instance.PlayerControllerEvents.RaisePlayerControllerDestroyed();
            }
        }

        #endregion

        #region Input Handlers

        private void MovementInputChangeEventHandler(Vector2 movementInput)
        {
            _currentTargetMovementVelocity = new Vector3(movementInput.x, 0f, movementInput.y);
        }

        private void PrimaryAttackInputChangeEventHandler(bool attackInput)
        {
            if (attackInput)
            {
                var origin = _previousBulletOrigin == 0 ? _bulletFlightOriginRightTransform : _bulletFlightOriginLeftTransform;
                FirePrimaryAttack(origin.position, origin.forward, _bulletFlightSpeed, _bulletLifetime);
            }
        }

        #endregion

        #region Primary Attack Logic

        private Entity GetBulletPrefabEntity()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(BulletPrefabEntity));
            if (query.CalculateEntityCount() > 0)
            {
                var prefabEntity = entityManager.GetComponentData<BulletPrefabEntity>(query.GetSingletonEntity()).Value;

                // Step A: Validate prefab entity
                if (!entityManager.Exists(prefabEntity))
                {
                    Debug.LogError("Bullet prefab entity does not exist in the EntityManager!");
                    return Entity.Null;
                }
                if (!entityManager.HasComponent<Prefab>(prefabEntity))
                {
                    Debug.LogError("Bullet prefab entity is not marked as a Prefab! Check your baker/subscene setup.");
                    return Entity.Null;
                }

                return prefabEntity;
            }
            else
            {
                Debug.LogWarning("Bullet prefab entity not ready!");
                return Entity.Null;
            }
        }

        private void FirePrimaryAttack(Vector3 bulletOriginPosition, Vector3 bulletFlightDirection, float bulletSpeed, float bulletLifetime)
        {
            var bulletPrefabEntity = GetBulletPrefabEntity();
            if (bulletPrefabEntity == Entity.Null)
            {
                Debug.LogWarning("Bullet Prefab Entity is not ready yet!");
                return;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;
            var bullet = entityManager.Instantiate(bulletPrefabEntity);

            entityManager.SetComponentData(bullet, new LocalTransform
            {
                Position = bulletOriginPosition,
                Rotation = quaternion.LookRotationSafe(bulletFlightDirection, math.up()),
                Scale = 1f
            });

            entityManager.SetComponentData(bullet, new BulletVelocity
            {
                Value = (float3)bulletFlightDirection * bulletSpeed
            });

            entityManager.SetComponentData(bullet, new BulletLifetime
            {
                Value = bulletLifetime
            });

            if (_previousBulletOrigin == 0)
            {
                _previousBulletOrigin = 1;
            }
            else
            {
                _previousBulletOrigin = 0;
            }
        }

        #endregion
    }
}
