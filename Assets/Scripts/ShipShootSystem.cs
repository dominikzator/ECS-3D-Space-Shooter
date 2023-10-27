using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Vector3 = System.Numerics.Vector3;

public partial class ShipShootSystem : SystemBase
{
	private bool initialized;
	private EntityManager manager;
	private NativeArray<Asteroid> nativeAsteroids;
	
	private static Octree.BoundingBox queryBox = new Octree.BoundingBox(Vector3.Zero, Vector3.Zero);
	public static List<Asteroid> asteroids = new List<Asteroid>();
	public static float ShipShootingRange = 50f;
    
	protected override void OnStartRunning()
	{
		var world = World.DefaultGameObjectInjectionWorld;
		manager = world.EntityManager;
	}

	protected override void OnUpdate()
	{
		var shipAspect = SystemAPI.GetAspect<ShipAspect>(Spawner.ShipEntity);
		float3 shipTransform = shipAspect.Transform.ValueRO.Position;
		
		queryBox = new Octree.BoundingBox(new Vector3(shipTransform.x, shipTransform.y, shipTransform.z), new Vector3(ShipShootingRange));
		queryBox.SetMinMax(new Vector3(queryBox.Min.X,queryBox.Min.Y, queryBox.Min.Z), new Vector3(queryBox.Max.X,queryBox.Max.Y, queryBox.Max.Z));
		if (Spawner.EntitiesOctTree.Count == 0)
		{
			return;
		}
		var results = Spawner.EntitiesOctTree.GetColliding(queryBox);
	}
}