using System;
using System.Collections.Generic;
using System.Linq;
using Octree;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    [SerializeField] private Mesh asteroidMesh;
    [SerializeField] private Mesh shipMesh;
    [SerializeField] private int stationarySpawnCount;
    [SerializeField] private int movableSpawnCount;
    [SerializeField] private Material asteroidMaterial;
    [SerializeField] private Material shipMaterial;
    [SerializeField] private float AsteroidMinRadius;
    [SerializeField] private float AsteroidMaxRadius;
    public float WorldRadius;
    public float MaxAsteroidVelocitySpeed;
    
    private static Random random;

    private static float asteroidMinRadius;
    private static float asteroidMaxRadius;
    private static float worldRadius = 7500f;
    private static float maxAsteroidVelocitySpeed;
    
    public static Octree.BoundsOctree<Entity> EntitiesOctTree = new BoundsOctree<Entity>(worldRadius * 1.5f, System.Numerics.Vector3.Zero, 1, 2f);

    // Example Burst job that creates many entities
    [GenerateTestsForBurstCompatibility]
    public struct SpawnAsteroidJob : IJobParallelFor
    {
        public Entity Prototype;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public bool Moving;

        public void Execute(int index)
        {
            var e = Ecb.Instantiate(index, Prototype);
            Ecb.SetComponent(index, e, MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
            var pos = random.NextFloat3Direction() * random.NextFloat(100f, worldRadius);
            var velocity = Moving ? random.NextFloat3Direction() * maxAsteroidVelocitySpeed : float3.zero;
            var radius = random.NextFloat(asteroidMinRadius, asteroidMaxRadius);
            var asteroid = new Asteroid {Position = pos, Radius = radius, LinearVelocity = velocity};
            Ecb.SetComponent(index, e, new LocalTransform {Position = pos, Scale = radius, Rotation = quaternion.identity});
            Ecb.SetComponent(index, e, asteroid);

            if (!Moving)
            {
                var box = new BoundingBox(new System.Numerics.Vector3(pos.x, pos.y, pos.z),
                    new System.Numerics.Vector3(radius));
                
                EntitiesOctTree.Add(e, box);
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        random = Random.CreateFromIndex((uint)UnityEngine.Random.Range(0, 100));
        asteroidMinRadius = AsteroidMinRadius;
        asteroidMaxRadius = AsteroidMaxRadius;
        worldRadius = WorldRadius;
        maxAsteroidVelocitySpeed = MaxAsteroidVelocitySpeed;
    }

    private void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        var desc = new RenderMeshDescription(
            shadowCastingMode: ShadowCastingMode.Off,
            receiveShadows: false);
        
        var asteroidMeshArray = new RenderMeshArray(new []{new Material(asteroidMaterial)}, new []{asteroidMesh});
        var shipMeshArray = new RenderMeshArray(new []{new Material(shipMaterial)}, new []{shipMesh});
        
        var prototype = entityManager.CreateEntity();
        
        RenderMeshUtility.AddComponents(
            prototype,
            entityManager,
            desc,
            asteroidMeshArray,
            MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
        
        entityManager.AddComponentData(prototype, new LocalTransform{Position = default, Scale = random.NextFloat(asteroidMinRadius, asteroidMaxRadius), Rotation = quaternion.identity});
        entityManager.AddComponentData(prototype, new Asteroid {Position = Vector3.zero, Radius = 1f, LinearVelocity = Vector3.zero});
        
        var spawnStationaryJob = new SpawnAsteroidJob
        {
            Prototype = prototype,
            Ecb = ecb.AsParallelWriter(),
            Moving = false
        };

        var spawnStationaryHandle = spawnStationaryJob.Schedule(stationarySpawnCount,stationarySpawnCount);
        spawnStationaryHandle.Complete();
        
        var spawnMovingJob = new SpawnAsteroidJob
        {
            Prototype = prototype,
            Ecb = ecb.AsParallelWriter(),
            Moving = true
        };

        var spawnMovingHandle = spawnMovingJob.Schedule(movableSpawnCount,movableSpawnCount);
        spawnMovingHandle.Complete();

        ecb.Playback(entityManager);
        ecb.Dispose();
        entityManager.DestroyEntity(prototype);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, WorldRadius);
    }
}
