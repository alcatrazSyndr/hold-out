using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace HoldOut
{
    [BurstCompile]
    public partial struct BulletMovementSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transform, velocity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<BulletVelocity>>())
            {
                transform.ValueRW.Position += velocity.ValueRO.Value * deltaTime;
            }
        }
    }
}