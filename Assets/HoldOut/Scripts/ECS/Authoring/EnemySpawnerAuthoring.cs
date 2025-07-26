using Unity.Entities;
using UnityEngine;

namespace HoldOut
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public GameObject EnemyPrefab;
        public int SpawnCount = 100;
        public float SpawnCooldown = 0.1f;
        public float SpawnRadius = 60f;

        class Baker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new EnemySpawnerComponent
                {
                    EnemyPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Renderable),
                    SpawnCount = authoring.SpawnCount,
                    SpawnCooldown = authoring.SpawnCooldown,
                    SpawnRadius = authoring.SpawnRadius,
                    Timer = 0f,
                    Spawned = 0
                });
            }
        }
    }
}