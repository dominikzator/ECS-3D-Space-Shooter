using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.U2D;
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

	private static float shootInterval = 0.4f;
	private static double lastTimeShoot = 0f;

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
		Debug.Log("ShipShoot OnUpdate 2");
		foreach (var (transform, ship)  in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Ship>>())
		{
			if (ship.ValueRO.FinishedPath)
			{
				return;
			}
			//SpawnProjectile(ref state, ship);
			shipPos = transform.ValueRO.Position;
			queryBox = new Octree.BoundingBox(new Vector3(shipPos.x, shipPos.y, shipPos.z), new Vector3(ShipShootingRange));
			queryBox.SetMinMax(new Vector3(queryBox.Min.X,queryBox.Min.Y, queryBox.Min.Z), new Vector3(queryBox.Max.X,queryBox.Max.Y, queryBox.Max.Z));
			var results = Spawner.EntitiesOctTree.GetColliding(queryBox);
			SpawnProjectile(ref state, transform, ship, transform.ValueRO.Forward());
			foreach (var entity in results)
			{
				
			}
			Debug.Log("results.Length: " + results.Length);
		}
	}

	private void SpawnProjectile(ref SystemState state, RefRW<LocalTransform> transform, RefRW<Ship> ship, float3 direction)
	{
		Debug.Log("SpawnProjectile");
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