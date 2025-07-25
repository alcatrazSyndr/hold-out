using Unity.Entities;

namespace HoldOut
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public Entity EnemyPrefab;
        public int SpawnCount;
        public int Spawned;
        public float SpawnCooldown;
        public float Timer;
        public float SpawnRadius;
    }
}
