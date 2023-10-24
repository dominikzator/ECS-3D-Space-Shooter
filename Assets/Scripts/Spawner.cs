using System;
using System.Collections.Generic;
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
    
    public List<Mesh> ParticleMeshes;
    public int StationarySpawnCount;
    public int MovableSpawnCount;
    public Material ParticleMaterial;
    public float AsteroidMinRadius;
    public float AsteroidMaxRadius;
    public float WorldRadius;
    public float MaxAsteroidVelocitySpeed;
    
    private static Random random;

    private static float asteroidMinRadius;
    private static float asteroidMaxRadius;
    private static float worldRadius;
    private static float maxAsteroidVelocitySpeed;

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
            var pos = random.NextFloat3Direction() * random.NextFloat(10f, worldRadius);
            var velocity = Moving ? random.NextFloat3Direction() * maxAsteroidVelocitySpeed : float3.zero;
            Ecb.SetComponent(index, e, new LocalTransform {Position = pos, Scale = random.NextFloat(asteroidMinRadius, asteroidMaxRadius), Rotation = quaternion.identity});
            Ecb.SetComponent(index, e, new Asteroid {Position = pos, Radius = random.NextFloat(asteroidMinRadius, asteroidMaxRadius), LinearVelocity = velocity});
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
        var matList = new List<Material>();
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        var mat = new Material(ParticleMaterial);
        matList.Add(mat);
        
        var desc = new RenderMeshDescription(
            shadowCastingMode: ShadowCastingMode.Off,
            receiveShadows: false);
        
        var renderMeshArray = new RenderMeshArray(matList.ToArray(), ParticleMeshes.ToArray());
        
        var prototype = entityManager.CreateEntity();
        
        RenderMeshUtility.AddComponents(
            prototype,
            entityManager,
            desc,
            renderMeshArray,
            MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
        
        entityManager.AddComponentData(prototype, new LocalTransform{Position = default, Scale = default, Rotation = quaternion.identity});
        entityManager.AddComponentData(prototype, new Asteroid {Position = Vector3.zero, Radius = 1f, LinearVelocity = Vector3.zero});
        
        var spawnStationaryJob = new SpawnAsteroidJob
        {
            Prototype = prototype,
            Ecb = ecb.AsParallelWriter(),
            Moving = false
        };

        var spawnStationaryHandle = spawnStationaryJob.Schedule(StationarySpawnCount,128);
        spawnStationaryHandle.Complete();
        
        var spawnMovingJob = new SpawnAsteroidJob
        {
            Prototype = prototype,
            Ecb = ecb.AsParallelWriter(),
            Moving = true
        };

        var spawnMovingHandle = spawnMovingJob.Schedule(MovableSpawnCount,128);
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
