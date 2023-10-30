using Unity.Entities;
using Unity.Mathematics;

public struct Projectile : IComponentData
{
    public float3 LinearVelocity;
    public float Radius;
    public float ProjectileSpeed;
    public float ProjectileLifeTime;
}
