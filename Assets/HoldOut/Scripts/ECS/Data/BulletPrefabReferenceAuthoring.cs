using UnityEngine;
using Unity.Entities;

namespace HoldOut
{
    public class BulletPrefabReferenceAuthoring : MonoBehaviour
    {
        public GameObject BulletPrefabGO;

        public class Baker : Baker<BulletPrefabReferenceAuthoring>
        {
            public override void Bake(BulletPrefabReferenceAuthoring authoring)
            {
                var prefabEntity = GetEntity(authoring.BulletPrefabGO, TransformUsageFlags.Dynamic);
                var singletonEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(singletonEntity, new BulletPrefabEntity { Value = prefabEntity });
            }
        }
    }

    public struct BulletPrefabEntity : IComponentData
    {
        public Entity Value;
    }
}