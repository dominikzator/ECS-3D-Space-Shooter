using System;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct MoveProjectiles : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<Projectile>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var deltaTime = SystemAPI.Time.DeltaTime;
		var timeElapsed = SystemAPI.Time.ElapsedTime;

		foreach (var (transform, projectile) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Projectile>>())
		{
			transform.ValueRW.Position += projectile.ValueRO.LinearVelocity * deltaTime * projectile.ValueRO.ProjectileSpeed;
		}
	}
}