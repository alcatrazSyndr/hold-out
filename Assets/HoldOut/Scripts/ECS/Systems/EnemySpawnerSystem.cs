using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using HoldOut;

[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemySpawnerComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float deltaTime = SystemAPI.Time.DeltaTime;
        float3 playerPos = GameManager.Instance.PlayerControllerPosition;

        foreach (var (spawner, entity) in SystemAPI.Query<RefRW<EnemySpawnerComponent>>().WithEntityAccess())
        {
            spawner.ValueRW.Timer -= deltaTime;

            if (spawner.ValueRO.Spawned >= spawner.ValueRO.SpawnCount)
                continue;

            if (spawner.ValueRW.Timer <= 0f)
            {
                // Random position around player (off screen)
                float angle = UnityEngine.Random.Range(0f, 2f * math.PI);
                float radius = spawner.ValueRO.SpawnRadius;
                float3 offset = new float3(math.cos(angle), 0f, math.sin(angle)) * radius;
                float3 spawnPos = playerPos + offset;
                spawnPos.y = 0.8f;

                // Spawn enemy
                Entity enemy = ecb.Instantiate(spawner.ValueRO.EnemyPrefab);
                ecb.SetComponent(enemy, LocalTransform.FromPosition(spawnPos));

                spawner.ValueRW.Spawned++;
                spawner.ValueRW.Timer = spawner.ValueRO.SpawnCooldown;
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
