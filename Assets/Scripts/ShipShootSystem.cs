using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

[BurstCompile]
[UpdateAfter(typeof(MoveShipSystem))]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct ShipShootSystem : ISystem
{
	private bool initialized;
	private EntityManager manager;
	private NativeArray<Asteroid> nativeAsteroids;
    
	private static float shootInterval = 0.4f;
	private static double lastTimeShoot = 0f;

	private Entity projectileEntity;
	private ShipAspect shipAspect;
	private float3 shipPos;
	
	private static List<Entity> results = new List<Entity>();

	private static List<Entity> shootedObjects = new List<Entity>();
	private bool CanShoot(ref SystemState state)
	{
		return SystemAPI.Time.ElapsedTime >= lastTimeShoot + shootInterval;
	}
	
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
		state.RequireForUpdate<Ship>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		if (!CanShoot(ref state))
		{
			return;
		}
		results.Clear();
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		foreach (var (transform, ship) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Ship>>())
		{
			if (ship.ValueRO.FinishedPath)
			{
				return;
			}
			shipPos = transform.ValueRO.Position;
			Vector3 shipVelocity = new Vector3(PathManager.Instance.Velocities[ship.ValueRW.WaypointProgress].x,PathManager.Instance.Velocities[ship.ValueRW.WaypointProgress].y,PathManager.Instance.Velocities[ship.ValueRW.WaypointProgress].z);
			World.Instance.QueryBox = new Octree.BoundingBox(new Vector3(shipPos.x, shipPos.y, shipPos.z), new Vector3(shipVelocity.Length() + (PathManager.Instance.MaxRandomVelocity * ship.ValueRO.ProjectileSpeed)));
			World.Instance.QueryBox.SetMinMax(new Vector3(World.Instance.QueryBox.Min.X,World.Instance.QueryBox.Min.Y, World.Instance.QueryBox.Min.Z), new Vector3(World.Instance.QueryBox.Max.X,World.Instance.QueryBox.Max.Y, World.Instance.QueryBox.Max.Z));
			results = World.Instance.Query(World.Instance.QueryBox);

			/*var asteroids = new List<Entity>();
			asteroids.AddRange(World.Instance.MovingAsteroidsInRange);
			asteroids.AddRange(results);*/
            
			foreach (var target in results.Where(p => !shootedObjects.Contains(p)))
			{
				if (!state.EntityManager.HasComponent<Asteroid>(target))
				{
					continue;
				}
				var asteroid = state.EntityManager.GetComponentData<Asteroid>(target);
				var velocityDiffTemp = asteroid.LinearVelocity - PathManager.Instance.Velocities[ship.ValueRW.WaypointProgress];
				var velocityDiff = new Vector3(velocityDiffTemp.x, velocityDiffTemp.y, velocityDiffTemp.z);
				var targetDirTemp = asteroid.Position - transform.ValueRO.Position;
				var targetDir = new Vector3(targetDirTemp.x, targetDirTemp.y, targetDirTemp.z);
				var a = Vector3.Dot(velocityDiff, velocityDiff) - (ship.ValueRO.ProjectileSpeed * ship.ValueRO.ProjectileSpeed);
				var b = 2 * Vector3.Dot(velocityDiff, targetDir);
				var c = Vector3.Dot(targetDir, targetDir);
				var disc = b * b - (4 * a * c);

				if(disc >= 0)
				{
					float t1 = (float)((-1f * b + System.Math.Sqrt(disc)) / (2 * a));
					float t2 = (float)((-1f * b - System.Math.Sqrt(disc)) / (2 * a));

					if(t1 >= 0f || t2 >= 0f)
					{
						var t = System.Math.Max(t1, t2);
						if (t2 >= 0 && t2 < t)
						{
							t = t2;
						}
						if (t1 >= 0 && t1 < t)
						{
							t = t1;
						}
						if (t <= 3f)
						{
							var aimPoint = new Vector3(asteroid.Position.x, asteroid.Position.y, asteroid.Position.z) + velocityDiff * t;
							var shootDir = aimPoint - new Vector3(transform.ValueRO.Position.x, transform.ValueRO.Position.y, transform.ValueRO.Position.z);
							SpawnProjectile(ref state, transform, ship, new float3(shootDir.X, shootDir.Y, shootDir.Z));
							shootedObjects.Add(target);
							return;
						}
					}
				}
			}
		}
	}

	private void SpawnProjectile(ref SystemState state, RefRW<LocalTransform> transform, RefRW<Ship> ship, float3 direction)
	{
		Vector3 shootDir = new Vector3(direction.x, direction.y, direction.z);
		shootDir /= shootDir.Length();
		var v = PathManager.Instance.Velocities[ship.ValueRW.WaypointProgress];
		shootDir *= ship.ValueRO.ProjectileSpeed;
		shootDir += new Vector3(v.x, v.y, v.z);
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		var projectile = ecb.Instantiate(ship.ValueRW.ProjectilePrefab);
		ecb.SetComponent(projectile, new LocalTransform{Position = transform.ValueRO.Position, Rotation = quaternion.identity, Scale = 1f});
		ecb.AddComponent(projectile, new Projectile{LinearVelocity = new float3(shootDir.X, shootDir.Y, shootDir.Z), Radius = 1f, ProjectileSpeed = ship.ValueRO.ProjectileSpeed, ProjectileLifeTime = ship.ValueRO.ProjectileLifetime});
		lastTimeShoot = SystemAPI.Time.ElapsedTime;
	}
}