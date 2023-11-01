using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Vector3 = System.Numerics.Vector3;

[BurstCompile]
[UpdateAfter(typeof(MoveShipSystem))]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct ShipShootSystem : ISystem
{
	private bool initialized;
	private EntityManager manager;
	private NativeArray<Asteroid> nativeAsteroids;
	
	private static Octree.BoundingBox queryBox = new Octree.BoundingBox(Vector3.Zero, Vector3.Zero);
	public static float ShipShootingRange = 100f;

	private static float shootInterval = 0.4f;
	private static double lastTimeShoot = 0f;

	private Entity projectileEntity;
	private ShipAspect shipAspect;
	private float3 shipPos;

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
        
		Entity[] results = default;
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		foreach (var (transform, ship)  in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Ship>>())
		{
			if (ship.ValueRO.FinishedPath)
			{
				return;
			}
			shipPos = transform.ValueRO.Position;
			queryBox = new Octree.BoundingBox(new Vector3(shipPos.x, shipPos.y, shipPos.z), new Vector3(ShipShootingRange));
			queryBox.SetMinMax(new Vector3(queryBox.Min.X,queryBox.Min.Y, queryBox.Min.Z), new Vector3(queryBox.Max.X,queryBox.Max.Y, queryBox.Max.Z));
			results = World.Instance.EntitiesOctTree.GetColliding(queryBox);
            
			foreach (var target in results.Where(p => !shootedObjects.Contains(p)))
			{
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
						if (t <= 10f)
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