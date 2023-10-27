using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct ShipAspect : IAspect
{
	public readonly Entity Entity;
    
	public readonly RefRW<LocalTransform> Transform;
	private readonly RefRW<Ship> ship;
}