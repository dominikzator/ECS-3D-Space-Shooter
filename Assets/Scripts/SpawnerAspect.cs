using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct SpawnerAspect : IAspect
{
	public readonly Entity Entity;
    
	private readonly RefRW<LocalTransform> _transform;
	private readonly RefRW<SpawnerData> spawnerData;
	public float3 Position => _transform.ValueRO.Position;

	public SpawnerData SpawnerData => spawnerData.ValueRO;
}