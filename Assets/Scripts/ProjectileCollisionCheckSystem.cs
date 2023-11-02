using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

[BurstCompile]
[UpdateAfter(typeof(SpawnerData))]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct ProjectileCollisionCheckSystem : ISystem
{
	private static Octree.BoundingBox queryBox = new Octree.BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero);

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
		state.RequireForUpdate<Projectile>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
		
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var ecb = new EntityCommandBuffer(Allocator.TempJob);
		foreach (var (transform, projectile, projectileEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Projectile>>().WithEntityAccess())
		{
			queryBox = new Octree.BoundingBox(new System.Numerics.Vector3(transform.ValueRO.Position.x, transform.ValueRO.Position.y, transform.ValueRO.Position.z), new System.Numerics.Vector3(transform.ValueRO.Scale));
			queryBox.SetMinMax(new System.Numerics.Vector3(queryBox.Min.X,queryBox.Min.Y, queryBox.Min.Z), new Vector3(queryBox.Max.X,queryBox.Max.Y, queryBox.Max.Z));
			var results = World.Instance.Query(queryBox);
			
			/*var asteroids = new List<Entity>();
			asteroids.AddRange(World.Instance.MovingAsteroidsInRange);
			asteroids.AddRange(results);*/
			
			foreach (var asteroidEntity in results)
			{
				try
				{
					var asteroidTransform = state.EntityManager.GetComponentData<LocalTransform>(asteroidEntity);
					var asteroid = state.EntityManager.GetComponentData<Asteroid>(asteroidEntity);
					var projectileTransform = state.EntityManager.GetComponentData<LocalTransform>(projectileEntity);
					new CheckForCollisionsJob
					{
						ECB = ecb,
						AsteroidEntity = asteroidEntity,
						ProjectileEntity = projectileEntity,
						AsteroidTransform = asteroidTransform,
						ProjectileTransform = projectileTransform,
						Asteroid = asteroid
					}.Run();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
		ecb.Playback(state.EntityManager);
	}
	[BurstCompile]
	public partial struct CheckForCollisionsJob : IJobEntity
	{
		public EntityCommandBuffer ECB;
		public Entity AsteroidEntity;
		public Entity ProjectileEntity;
		public LocalTransform AsteroidTransform;
		public LocalTransform ProjectileTransform;
		public Asteroid Asteroid;
		
		private void Execute(ref Projectile projectile)
		{
            var dist = Vector3.Distance(new Vector3(ProjectileTransform.Position.x,ProjectileTransform.Position.y, ProjectileTransform.Position.z), new Vector3(AsteroidTransform.Position.x, AsteroidTransform.Position.y, AsteroidTransform.Position.z));
            if (dist <= ProjectileTransform.Scale + AsteroidTransform.Scale)
            {
                ECB.DestroyEntity(ProjectileEntity);
                ECB.DestroyEntity(AsteroidEntity);
            }
		}
	}
}