using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

class ShipAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public float ProjectileSpeed;
    public float ProjectileLifetime;
}

class ShipBaker : Baker<ShipAuthoring>
{
    public override void Bake(ShipAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Ship
        {
            ProjectilePrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            Time = default,
            FinishedPath = default,
            WaypointProgress = default,
            ProjectileSpeed = authoring.ProjectileSpeed,
            ProjectileLifetime = authoring.ProjectileLifetime
        });
        AddComponent(entity, new LocalTransform{Position = Vector3.zero, Scale = 5f, Rotation = quaternion.identity});
    }
}
