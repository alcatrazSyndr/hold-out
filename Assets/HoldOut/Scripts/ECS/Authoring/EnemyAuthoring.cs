using HoldOut;
using Unity.Entities;
using UnityEngine;

namespace HoldOut
{
    public class EnemyAuthoring : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _stopDistance = 1.5f;
        [SerializeField] private float _avoidanceRadius = 1.2f;
        [SerializeField] private float _health = 10f;

        class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyTag());
                AddComponent(entity, new EnemyMoveSpeed { Value = authoring._moveSpeed });
                AddComponent(entity, new EnemyStopDistance { Value = authoring._stopDistance });
                AddComponent(entity, new EnemyAvoidanceRadius { Value = authoring._avoidanceRadius });
                AddComponent(entity, new EnemyHealth { Value = authoring._health });
            }
        }
    }
}
