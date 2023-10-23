using Unity.Entities;
using Unity.Mathematics;

public struct Asteroid : IComponentData
{
    public float3 Position;
    public float3 LinearVelocity;
    public float Radius;
}
