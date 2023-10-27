using System.Collections.Generic;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;
using Random = Unity.Mathematics.Random;
using Vector3 = System.Numerics.Vector3;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial class ShipShootSystem : SystemBase
{
	private bool initialized;
	private EntityManager manager;
	private NativeArray<Asteroid> nativeAsteroids;
	
	private static Octree.BoundingBox queryBox = new Octree.BoundingBox(Vector3.Zero, Vector3.Zero);
	public static float ShipShootingRange = 100f;

	private float shootInterval = 1f;
	private double lastTimeShoot = 0f;
	private bool CanShoot => SystemAPI.Time.ElapsedTime >= lastTimeShoot + shootInterval;

	private EntityCommandBuffer ecb;
	private Entity projectileEntity;
	private ShipAspect shipAspect;
	private float3 shipPos;
    
	protected override void OnStartRunning()
	{
		RequireForUpdate<Ship>();
		
		var world = World.DefaultGameObjectInjectionWorld;
		manager = world.EntityManager;
		ecb = new EntityCommandBuffer(Allocator.Persistent);
	}

	protected override void OnUpdate()
	{
		if (!CanShoot)
		{
			return;
		}
		Debug.Log("Shoot!");
		shipAspect = SystemAPI.GetAspect<ShipAspect>(Spawner.ShipEntity);
		shipPos = shipAspect.Transform.ValueRO.Position;
		SpawnProjectile();
		lastTimeShoot = SystemAPI.Time.ElapsedTime;
		
		queryBox = new Octree.BoundingBox(new Vector3(shipPos.x, shipPos.y, shipPos.z), new Vector3(ShipShootingRange));
		queryBox.SetMinMax(new Vector3(queryBox.Min.X,queryBox.Min.Y, queryBox.Min.Z), new Vector3(queryBox.Max.X,queryBox.Max.Y, queryBox.Max.Z));
		if (Spawner.EntitiesOctTree.Count == 0)
		{
			return;
		}
		var results = Spawner.EntitiesOctTree.GetColliding(queryBox);
		Debug.Log("results.Length: " + results.Length);
		foreach (var asteroid in results)
		{
			//var projectileMeshArray = new RenderMeshArray(new []{new Material(Spawner.Instance.ProjectileMaterial)}, new []{Spawner.Instance.ProjectileMesh});
			//if (manager.HasComponent<LocalTransform>(asteroid))
			//{
			//	manager.SetComponentData(asteroid, new LocalTransform{Position = float3.zero, Scale = 0.2f, Rotation = quaternion.identity});
			//}
            
			//var aspect = SystemAPI.GetAspect<AsteroidAspect>(asteroid);
			//Debug.Log("aspect.Position: " + aspect.Position);
			//var transform = SystemAPI.GetComponent<LocalTransform>(asteroid);
			//Debug.Log("transform.Position: " + transform.Position);
			//manager.DestroyEntity(asteroid);
		}
	}

	private void SpawnProjectile()
	{
		projectileEntity = manager.CreateEntity();
		
		var projectileMeshArray = new RenderMeshArray(new []{new Material(Spawner.Instance.ProjectileMaterial)}, new []{Spawner.Instance.ProjectileMesh});

		var desc = new RenderMeshDescription(
			shadowCastingMode: ShadowCastingMode.Off,
			receiveShadows: false);

		RenderMeshUtility.AddComponents(
			projectileEntity,
			manager,
			desc,
			projectileMeshArray,
			MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
		
		manager.AddComponentData(projectileEntity, new LocalTransform {Position = shipPos, Scale = 2f, Rotation = quaternion.identity});
		manager.AddComponentData(projectileEntity, new Projectile{LinearVelocity = float3.zero, Radius = 5f});
		ecb = new EntityCommandBuffer(Allocator.TempJob);
	}
}