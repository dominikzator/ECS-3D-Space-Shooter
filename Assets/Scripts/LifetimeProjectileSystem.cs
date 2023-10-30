using System;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct LifetimeProjectileSystem : ISystem
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

		foreach (var (projectile, entity) in SystemAPI.Query<RefRW<Projectile>>().WithEntityAccess())
		{
			projectile.ValueRW.ProjectileLifeTime -= deltaTime;
			if (projectile.ValueRO.ProjectileLifeTime < 0f)
			{
				Debug.Log("Kill Projectile!");
				var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
				var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
				ecb.DestroyEntity(entity);
			}
			
		}
	}
}