using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

namespace HoldOut
{
    [BurstCompile]
    public partial struct EnemyMovementJob : IJobEntity
    {
        [ReadOnly] public NativeParallelMultiHashMap<int, float3> EntityPositions;
        public float3 PlayerPos;
        public float DeltaTime;
        public float CellSize;
        public float MinPlayerOffset;
        public float PushStrength;

        void Execute(ref LocalTransform transform, in MoveSpeed speed, in StopDistance stopDist, in EnemyAvoidanceRadius avoidance)
        {
            float3 myPos = transform.Position;
            float3 toPlayer = PlayerPos - myPos;
            float distToPlayer = math.length(toPlayer);

            float3 moveDir = float3.zero;

            // Player Push Force
            if (distToPlayer < MinPlayerOffset)
            {
                float pushAmount = MinPlayerOffset - distToPlayer;
                float3 pushDir = math.normalizesafe(myPos - PlayerPos);
                moveDir += pushDir * pushAmount * PushStrength;
            }

            if (distToPlayer > stopDist.Value)
            {
                moveDir += math.normalize(toPlayer);
            }

            float3 avoidanceForce = float3.zero;
            int x = (int)math.floor(myPos.x / CellSize);
            int z = (int)math.floor(myPos.z / CellSize);
            int myCell = x * 10000 + z;

            for (int dx = -1; dx <= 1; dx++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    int neighborCell = (x + dx) * 10000 + (z + dz);
                    NativeParallelMultiHashMapIterator<int> it;
                    float3 otherPos;
                    if (EntityPositions.TryGetFirstValue(neighborCell, out otherPos, out it))
                    {
                        do
                        {
                            if (!otherPos.Equals(myPos))
                            {
                                float dist = math.distance(myPos, otherPos);
                                if (dist < avoidance.Value && dist > 0.01f)
                                {
                                    float strength = 1f - (dist / avoidance.Value);
                                    avoidanceForce += math.normalize(myPos - otherPos) * strength;
                                }
                            }
                        } while (EntityPositions.TryGetNextValue(out otherPos, ref it));
                    }
                }

            moveDir += avoidanceForce * 1.5f;

            if (!moveDir.Equals(float3.zero))
            {
                float3 finalDir = math.normalize(moveDir);
                transform.Position += finalDir * speed.Value * DeltaTime;
                transform.Rotation = quaternion.LookRotationSafe(finalDir, math.up());
            }
        }
    }

    [BurstCompile]
    public partial struct EnemyMovementSystem : ISystem
    {
        NativeParallelMultiHashMap<int, float3> _entityPositions;
        float _cellSize;

        public void OnCreate(ref SystemState state)
        {
            _cellSize = Constants.ENEMY_AVOIDANCE_CELL_SIZE;
            _entityPositions = new NativeParallelMultiHashMap<int, float3>(1024, Allocator.Persistent);
        }

        public void OnUpdate(ref SystemState state)
        {
            _entityPositions.Clear();

            var query = SystemAPI.QueryBuilder().WithAll<EnemyTag>().Build();
            int enemyCount = query.CalculateEntityCount();

            if (_entityPositions.Capacity < enemyCount)
                _entityPositions.Capacity = enemyCount;

            float3 playerPos = GameManager.Instance.PlayerControllerPosition;
            playerPos.y = 0.8f;
            float deltaTime = SystemAPI.Time.DeltaTime;
            float minPlayerOffset = Constants.ENEMY_AVOIDANCE_MIN_PLAYER_OFFSET;
            float pushStrength = Constants.ENEMY_AVOIDANCE_PLAYER_PUSH_STRENGTH;

            foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<EnemyTag>())
            {
                float3 pos = transform.ValueRO.Position;
                int x = (int)math.floor(pos.x / _cellSize);
                int z = (int)math.floor(pos.z / _cellSize);
                int cell = x * 10000 + z;
                _entityPositions.Add(cell, pos);
            }

            var job = new EnemyMovementJob
            {
                EntityPositions = _entityPositions,
                PlayerPos = playerPos,
                DeltaTime = deltaTime,
                CellSize = _cellSize,
                MinPlayerOffset = minPlayerOffset,
                PushStrength = pushStrength
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        public void OnDestroy(ref SystemState state)
        {
            if (_entityPositions.IsCreated)
                _entityPositions.Dispose();
        }
    }
}