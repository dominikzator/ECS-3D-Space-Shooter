using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

class ShipAuthoring : MonoBehaviour
{
    public GameObject Prefab;
}

class ShipBaker : Baker<ShipAuthoring>
{
    public override void Bake(ShipAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Ship
        {
            ProjectilePrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
        });
        AddComponent(entity, new LocalTransform{Position = Vector3.zero, Scale = 5f, Rotation = quaternion.identity});
    }
}
