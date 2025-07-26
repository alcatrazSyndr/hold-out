using Unity.Burst;
using Unity.Entities;

namespace HoldOut
{
    [BurstCompile]
    public partial struct BulletLifetimeSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (lifetime, entity) in SystemAPI.Query<RefRW<BulletLifetime>>().WithEntityAccess())
            {
                lifetime.ValueRW.Value -= dt;
                if (lifetime.ValueRW.Value <= 0f)
                    ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}