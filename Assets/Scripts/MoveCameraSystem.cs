using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct MoveCameraSystem : ISystem
{
	private bool initialized;
	private EntityManager manager;
	private NativeArray<Asteroid> nativeAsteroids;
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		//var world = World.DefaultGameObjectInjectionWorld;
		//manager = world.EntityManager;
	}
	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		/*var query = SystemAPI.QueryBuilder().WithAll<Asteroid>().Build();
		var array = query.ToEntityArray(Allocator.TempJob);
		
		if (!initialized && array.Length > 0)
		{
			Debug.Log("Initialized!");
			initialized = true;

			List<Asteroid> asteroids = new List<Asteroid>();

			foreach (var entity in array)
			{
				var asteroid = manager.GetComponentData<Asteroid>(entity);
				asteroids.Add(asteroid);
			}

			nativeAsteroids = new NativeArray<Asteroid>(asteroids.ToArray(), Allocator.TempJob);
		}
        
		var job = new MoveCameraJob{Asteroids = nativeAsteroids};
		job.Run();*/
	}
}

[BurstCompile]
public partial struct MoveCameraJob : IJobEntity
{
	public NativeArray<Asteroid> Asteroids;
	
	[BurstCompile]
	private void Execute(ShipAspect ship, [EntityIndexInQuery] int sortKey)
	{
		List<Asteroid> closeAsteroids = new List<Asteroid>();
		foreach (var entity in Asteroids)
		{
			if (Vector3.Distance(entity.Position, ship.Position) <= 100f)
			{
				closeAsteroids.Add(entity);
			}
		}
		
		Debug.Log("closeAsteroids.Count: " + closeAsteroids.Count);
		
		//CameraManager.Instance.AlignCamera(ship.Position, ship.ForwardVector, ship.RightVector);
	}
}