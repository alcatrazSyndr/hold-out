using Unity.Entities;
using Unity.Mathematics;

namespace HoldOut
{
    public struct BulletVelocity : IComponentData
    {
        public float3 Value;
    }
}
