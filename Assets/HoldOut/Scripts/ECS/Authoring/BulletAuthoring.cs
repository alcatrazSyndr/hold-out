using UnityEngine;
using Unity.Entities;

namespace HoldOut
{
    public class BulletAuthoring : MonoBehaviour
    {
        public class Baker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BulletTag());
                AddComponent(entity, new BulletVelocity());
                AddComponent(entity, new BulletLifetime());
                AddComponent(entity, new BulletImpactDamage());
            }
        }
    }
}
