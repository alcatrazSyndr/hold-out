using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

namespace HoldOut
{
    public struct HitInfo
    {
        public Entity Bullet;
        public Entity Enemy;
        public float Damage;
    }

    [BurstCompile]
    public partial struct BulletImpactJob : IJobEntity
    {
        [ReadOnly] public NativeParallelMultiHashMap<int, (Entity, float3)> EnemyPositions;
        public float ImpactDistSq;
        public float CellSize;
        public NativeQueue<HitInfo>.ParallelWriter HitResults;

        void Execute(Entity bulletEntity, in LocalTransform bulletTransform, in BulletImpactDamage impactDamage, in BulletTag bulletTag)
        {
            float3 bulletPos = bulletTransform.Position;
            bulletPos.y = 0f;
            int x = (int)math.floor(bulletPos.x / CellSize);
            int z = (int)math.floor(bulletPos.z / CellSize);

            Entity closestEnemy = Entity.Null;
            float minDistSq = ImpactDistSq;

            // Find closest enemy in 3x3 cell area
            for (int dx = -1; dx <= 1; dx++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    int neighborCell = (x + dx) * 10000 + (z + dz);

                    NativeParallelMultiHashMapIterator<int> it;
                    (Entity, float3) enemyData;
                    if (EnemyPositions.TryGetFirstValue(neighborCell, out enemyData, out it))
                    {
                        do
                        {
                            float distSq = math.distancesq(bulletPos, enemyData.Item2);
                            if (distSq < minDistSq)
                            {
                                minDistSq = distSq;
                                closestEnemy = enemyData.Item1;
                            }
                        }
                        while (EnemyPositions.TryGetNextValue(out enemyData, ref it));
                    }
                }

            if (closestEnemy != Entity.Null)
            {
                HitResults.Enqueue(new HitInfo
                {
                    Bullet = bulletEntity,
                    Enemy = closestEnemy,
                    Damage = impactDamage.Value
                });
            }
        }
    }

    [BurstCompile]
    public partial struct BulletImpactSystem : ISystem
    {
        NativeParallelMultiHashMap<int, (Entity, float3)> _enemyPositions;
        float _cellSize;

        public void OnCreate(ref SystemState state)
        {
            _cellSize = Constants.ENEMY_AVOIDANCE_CELL_SIZE;
            _enemyPositions = new NativeParallelMultiHashMap<int, (Entity, float3)>(1024, Allocator.Persistent);
        }

        public void OnUpdate(ref SystemState state)
        {
            _enemyPositions.Clear();

            // MAIN THREAD: Fill enemy positions map WITH ENTITY
            foreach (var (transform, enemyEntity) in SystemAPI.Query<RefRO<LocalTransform>>().WithEntityAccess().WithAll<EnemyTag>())
            {
                float3 pos = transform.ValueRO.Position;
                int x = (int)math.floor(pos.x / _cellSize);
                int z = (int)math.floor(pos.z / _cellSize);
                int cell = x * 10000 + z;
                pos.y = 0f;
                _enemyPositions.Add(cell, (enemyEntity, pos));
            }

            // Allocate a NativeQueue for hit results
            var hitResults = new NativeQueue<HitInfo>(Allocator.TempJob);

            // SCHEDULE JOB: Process all bullets in parallel
            var job = new BulletImpactJob
            {
                EnemyPositions = _enemyPositions,
                ImpactDistSq = 1f, // 1 unit squared (adjust as needed)
                CellSize = _cellSize,
                HitResults = hitResults.AsParallelWriter()
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();

            // MAIN THREAD: Apply results (damage and destroy)
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // Use a set to avoid double-destroying entities
            var deadEnemies = new NativeHashSet<Entity>(16, Allocator.Temp);

            var entityManager = state.EntityManager;

            while (hitResults.TryDequeue(out var hit))
            {
                // Destroy bullet
                ecb.DestroyEntity(hit.Bullet);

                // Only damage/destroy each enemy once per frame
                if (entityManager.HasComponent<EnemyHealth>(hit.Enemy) && !deadEnemies.Contains(hit.Enemy))
                {
                    var health = entityManager.GetComponentData<EnemyHealth>(hit.Enemy);
                    health.Value -= hit.Damage;

                    if (health.Value <= 0f)
                    {
                        ecb.DestroyEntity(hit.Enemy);
                        deadEnemies.Add(hit.Enemy);
                    }
                    else
                    {
                        entityManager.SetComponentData(hit.Enemy, health);
                    }
                }
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
            hitResults.Dispose();
            deadEnemies.Dispose();
        }

        public void OnDestroy(ref SystemState state)
        {
            if (_enemyPositions.IsCreated)
                _enemyPositions.Dispose();
        }
    }
}