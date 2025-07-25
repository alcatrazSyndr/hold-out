using Unity.Entities;
using UnityEngine;

namespace HoldOut
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public int spawnCount = 100;
        public float spawnCooldown = 0.1f;
        public float spawnRadius = 30f;

        class Baker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new EnemySpawnerComponent
                {
                    EnemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Renderable),
                    SpawnCount = authoring.spawnCount,
                    SpawnCooldown = authoring.spawnCooldown,
                    SpawnRadius = authoring.spawnRadius,
                    Timer = 0f,
                    Spawned = 0
                });
            }
        }
    }
}