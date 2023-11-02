using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Octree;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    public Octree.BoundsOctree<Entity> EntitiesOctTree;

    public List<Entity> MovingAsteroidsInRange = new List<Entity>();
    
    public Octree.BoundingBox QueryBox = new Octree.BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero);
    
    [SerializeField] private float worldRadius;
    [SerializeField] private float minAsteroidVelocitySpeed;
    [SerializeField] private float maxAsteroidVelocitySpeed;

    public float WorldRadius => worldRadius;
    public float MinAsteroidVelocitySpeed => minAsteroidVelocitySpeed;
    public float MaxAsteroidVelocitySpeed => maxAsteroidVelocitySpeed;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        EntitiesOctTree = new BoundsOctree<Entity>(worldRadius * 1.5f, System.Numerics.Vector3.Zero, 1, 2f);
    }

    public List<Entity> Query(Octree.BoundingBox queryBox)
    {
        List<Entity> results = World.Instance.EntitiesOctTree.GetColliding(queryBox).ToList();

        return results;
    }
}
