using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

[BurstCompile]
[UpdateAfter(typeof(MoveShipSystem))]
public partial struct ShipShootSystem : ISystem
{
	private bool initialized;
	private EntityManager manager;
	private NativeArray<Asteroid> nativeAsteroids;
	
	private static Octree.BoundingBox queryBox = new Octree.BoundingBox(Vector3.Zero, Vector3.Zero);
	public static float ShipShootingRange = 100f;

	private static float shootInterval = 1f;
	private static double lastTimeShoot = 0f;

	private EntityCommandBuffer ecb;
	private Entity projectileEntity;
	private ShipAspect shipAspect;
	private float3 shipPos;

	private bool CanShoot(ref SystemState state)
	{
		return SystemAPI.Time.ElapsedTime >= lastTimeShoot + shootInterval;
	}
	
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
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

		foreach (var (transform, ship)  in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Ship>>())
		{
			//SpawnProjectile(ref state, ship);
			shipPos = transform.ValueRO.Position;
			queryBox = new Octree.BoundingBox(new Vector3(shipPos.x, shipPos.y, shipPos.z), new Vector3(ShipShootingRange));
			queryBox.SetMinMax(new Vector3(queryBox.Min.X,queryBox.Min.Y, queryBox.Min.Z), new Vector3(queryBox.Max.X,queryBox.Max.Y, queryBox.Max.Z));
			var results = Spawner.EntitiesOctTree.GetColliding(queryBox);
			
			var deltaTime = SystemAPI.Time.DeltaTime;
			var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
			var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
			Entity projectile = ecb.Instantiate(ship.ValueRW.ProjectilePrefab);
			ecb.SetComponent(projectile, new LocalTransform{Position = transform.ValueRO.Position, Rotation = quaternion.identity, Scale = 1f});
			foreach (var entity in results)
			{
				
			}
			Debug.Log("results.Length: " + results.Length);
		}
		lastTimeShoot = SystemAPI.Time.ElapsedTime;
	}
}