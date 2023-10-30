using Unity.Entities;

public struct Ship : IComponentData
{
    public Entity ProjectilePrefab;
    public float Time;
    public int WaypointProgress;
    public bool FinishedPath;
    public float ProjectileSpeed;
    public float ProjectileLifetime;
}
