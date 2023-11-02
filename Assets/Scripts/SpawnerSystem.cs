using System;
using Octree;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct SpawnerSystem : ISystem
{
	private static int stationarySpawned = 0;
	private static int movingSpawned = 0;
	private bool finishedSpawning;
	private bool structureInitialized;
	private int stationaryCount;
    
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
		state.RequireForUpdate<SpawnerData>();
	}

	public void OnDestroy(ref SystemState state) { }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		if (finishedSpawning)
		{
			if (!structureInitialized)
			{
				var tempECB = new EntityCommandBuffer(Allocator.TempJob);
				foreach (var (transform, asteroid, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Asteroid>>().WithEntityAccess())
				{
					if (entity.Index < 0)
					{
						continue;
					}
					if (asteroid.ValueRO.LinearVelocity.x == 0 && asteroid.ValueRO.LinearVelocity.y == 0 && asteroid.ValueRO.LinearVelocity.z == 0)
					{
						stationaryCount++;
						var box = new BoundingBox(new System.Numerics.Vector3(transform.ValueRO.Position.x, transform.ValueRO.Position.y, transform.ValueRO.Position.z), new System.Numerics.Vector3(asteroid.ValueRO.Radius, asteroid.ValueRO.Radius, asteroid.ValueRO.Radius));
						World.Instance.EntitiesOctTree.Add(entity, box);
					}
					else
					{
						tempECB.AddComponent<MovingAsteroid>(entity);
					}
				}
                structureInitialized = true;
                tempECB.Playback(state.EntityManager);
			}
			return;
		}
        
		var ecb = new EntityCommandBuffer(Allocator.TempJob);
		foreach (var spawner in SystemAPI.Query<RefRW<SpawnerData>>())
		{
			var stationaryToSpawn = stationarySpawned < spawner.ValueRO.stationaryAsteroidsCount
				? (spawner.ValueRO.stationaryAsteroidsCount - stationarySpawned >= 1000)
					? 1000
					: spawner.ValueRO.stationaryAsteroidsCount - stationarySpawned
				: 0;
            
			if (stationaryToSpawn > 0)
			{
				for (int i = 0; i < stationaryToSpawn; i++)
				{
					new SpawnAsteroidJob
					{
						ECB = ecb,
						Moving = false
					}.Run();
				}
			}

			stationarySpawned += stationaryToSpawn;
			var movingToSpawn = movingSpawned < spawner.ValueRO.movingAsteroidsCount
				? (spawner.ValueRO.movingAsteroidsCount - movingSpawned >= 1000)
					? 1000
					: spawner.ValueRO.movingAsteroidsCount - movingSpawned
				: 0;
			if (movingToSpawn > 0)
			{
				for (int i = 0; i < movingToSpawn; i++)
				{
					new SpawnAsteroidJob
					{
						ECB = ecb,
						Moving = true
					}.Run();
				}
			}
			movingSpawned += movingToSpawn;
			if (movingToSpawn == 0 && stationaryToSpawn == 0)
			{
				finishedSpawning = true;
			}
		}
		ecb.Playback(state.EntityManager);
	}
	[BurstCompile]
	public partial struct SpawnAsteroidJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public bool Moving;
        
		private void Execute(ref SpawnerData spawner)
		{
			Entity newEntity = ECB.Instantiate(spawner.AsteroidPrefab);
			var randomRadius = spawner.Random.NextFloat(spawner.MinAsteroidRadius, spawner.MaxAsteroidRadius);
			var pos = spawner.Random.NextFloat3Direction() * spawner.Random.NextFloat(100f, World.Instance.WorldRadius);
			ECB.AddComponent<Asteroid>(newEntity);
			ECB.SetComponent(newEntity, new LocalTransform{Position = pos, Scale = randomRadius, Rotation = quaternion.identity});
			var velocity = Moving ? spawner.Random.NextFloat3() * spawner.Random.NextFloat(spawner.MinAsteroidSpeed, spawner.MaxAsteroidSpeed) : float3.zero;

			ECB.SetComponent(newEntity,
				new Asteroid
				{
					Position = pos,
					Radius = randomRadius,
					LinearVelocity = velocity,
					MinAsteroidSpeed = spawner.MinAsteroidSpeed,
					MaxAsteroidSpeed = spawner.MaxAsteroidSpeed
				});
		}
	}
}