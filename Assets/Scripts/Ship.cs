using System.Linq;
using Unity.Entities;
using Unity.Mathematics;

public struct Ship : IComponentData
{
    public float3 Position;
    public float Time;
    public int WaypointProgress;
    public bool FinishedPath;
}
