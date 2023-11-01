using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct AsteroidAspect : IAspect
{
	public readonly Entity Entity;
    
	private readonly RefRW<LocalTransform> _transform;
	private readonly RefRW<Asteroid> asteroid;
    
	public float3 Position => _transform.ValueRO.Position;
	public float3 LinearVelocity => asteroid.ValueRO.LinearVelocity;

	public Asteroid Asteroid => asteroid.ValueRO;

	public void Move(float deltaTime, Random random)
	{
		if (!LinearVelocity.Equals(float3.zero))
		{
			_transform.ValueRW.Position += LinearVelocity * deltaTime;
			if (Vector3.Distance(_transform.ValueRW.Position, Vector3.zero) > World.Instance.WorldRadius)
			{
				_transform.ValueRW.Position = random.NextFloat3Direction() * World.Instance.WorldRadius;
				var newVelo = random.NextFloat3Direction() * random.NextFloat(asteroid.ValueRO.MinAsteroidSpeed,
					asteroid.ValueRO.MaxAsteroidSpeed);
				asteroid.ValueRW.LinearVelocity = newVelo;
			}
		}
	}
}