using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpawnerAuthoring : MonoBehaviour
{
	[SerializeField] private float minAsteroidRadius;
	[SerializeField] private float maxAsteroidRadius;
	[SerializeField] private float minAsteroidSpeed;
	[SerializeField] private float maxAsteroidSpeed;
	[SerializeField] private GameObject asteroidPrefab;
	[SerializeField] private int stationaryAsteroidsCount;
	[SerializeField] private int movingAsteroidsCount;

	public float MinAsteroidRadius => minAsteroidRadius;
	public float MaxAsteroidRadius => maxAsteroidRadius;

	public float MinAsteroidSpeed => minAsteroidSpeed;
	public float MaxAsteroidSpeed => maxAsteroidSpeed;
	public GameObject AsteroidPrefab => asteroidPrefab;
	public int StationaryAsteroidsCount => stationaryAsteroidsCount;
	public int MovingAsteroidsCount => movingAsteroidsCount;
}

class SpawnerBaker : Baker<SpawnerAuthoring>
{
	public override void Bake(SpawnerAuthoring authoring)
	{
		var entity = GetEntity(TransformUsageFlags.Dynamic);
		AddComponent(entity, new LocalTransform{Position = Vector3.zero, Scale = UnityEngine.Random.Range(authoring.MinAsteroidRadius, authoring.MaxAsteroidRadius), Rotation = quaternion.identity});
		var asteroidEntity = GetEntity(authoring.AsteroidPrefab, TransformUsageFlags.Dynamic);
		AddComponent(entity, new SpawnerData
		{
			AsteroidPrefab = asteroidEntity,
			MinAsteroidSpeed = authoring.MinAsteroidSpeed,
			MaxAsteroidSpeed = authoring.MaxAsteroidSpeed,
			MinAsteroidRadius = authoring.MinAsteroidRadius,
			MaxAsteroidRadius = authoring.MaxAsteroidRadius,
			stationaryAsteroidsCount = authoring.StationaryAsteroidsCount,
			movingAsteroidsCount = authoring.MovingAsteroidsCount,
			Random = Random.CreateFromIndex((uint)UnityEngine.Random.Range(-100, 100))
		});
	}
}
