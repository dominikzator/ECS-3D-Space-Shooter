using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawnerData : IComponentData
{
	public Entity AsteroidPrefab;
	public float MinAsteroidRadius;
	public float MaxAsteroidRadius;
	public float MinAsteroidSpeed;
	public float MaxAsteroidSpeed;
	public int stationaryAsteroidsCount;
	public int movingAsteroidsCount;
	public Unity.Mathematics.Random Random;
}