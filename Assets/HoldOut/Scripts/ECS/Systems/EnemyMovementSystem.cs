using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using HoldOut;

[BurstCompile]
public partial struct EnemyMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float3 playerPos = GameManager.Instance.PlayerControllerPosition;
        playerPos.y = 0.8f;
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Get all enemy positions for avoidance
        var positions = new NativeList<float3>(Allocator.Temp);
        foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<EnemyTag>())
        {
            positions.Add(transform.ValueRO.Position);
        }

        foreach (var (transform, speed, stopDist, avoidance, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>, RefRO<StopDistance>, RefRO<EnemyAvoidanceRadius>>()
                          .WithEntityAccess().WithAll<EnemyTag>())
        {
            float3 myPos = transform.ValueRO.Position;
            float3 toPlayer = playerPos - myPos;
            float distToPlayer = math.length(toPlayer);

            if (distToPlayer > stopDist.ValueRO.Value)
            {
                float3 moveDir = math.normalize(toPlayer);

                // avoidance logic
                float3 avoidanceForce = float3.zero;
                foreach (var otherPos in positions)
                {
                    if (otherPos.Equals(myPos)) continue;

                    float3 offset = myPos - otherPos;
                    float dist = math.length(offset);

                    if (dist < avoidance.ValueRO.Value && dist > 0.01f)
                    {
                        float strength = 1f - (dist / avoidance.ValueRO.Value); // Stronger the closer they are
                        avoidanceForce += math.normalize(offset) * strength;
                    }
                }

                float3 finalDir = math.normalize(moveDir + avoidanceForce * 1.5f); // weight avoidance

                transform.ValueRW.Position += finalDir * speed.ValueRO.Value * deltaTime;

                // rotate to face player
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(finalDir, math.up());
            }
        }

        positions.Dispose();
    }
}
