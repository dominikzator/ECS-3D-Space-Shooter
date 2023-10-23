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
    public List<Mesh> ParticleMeshes;
    public int ParticlesToSpawn;
    public Material ParticleMaterial;

    // Example Burst job that creates many entities
    [GenerateTestsForBurstCompatibility]
    public struct SpawnJob : IJobParallelFor
    {
        public Entity Prototype;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute(int index)
        {
            var e = Ecb.Instantiate(index, Prototype);
            
            Ecb.SetComponent(index, e, MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
            Ecb.SetComponent(index, e, new LocalTransform {Position = Vector3.zero, Scale = 1f, Rotation = quaternion.identity});
            Ecb.SetComponent(index, e, new Asteroid {Position = Vector3.zero, Radius = 1f, LinearVelocity = Vector3.zero});
        }
    }
    void Start()
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
        
        var spawnJob = new SpawnJob
        {
            Prototype = prototype,
            Ecb = ecb.AsParallelWriter(),
        };

        var spawnHandle = spawnJob.Schedule(ParticlesToSpawn,128);
        spawnHandle.Complete();

        ecb.Playback(entityManager);
        ecb.Dispose();
        entityManager.DestroyEntity(prototype);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
