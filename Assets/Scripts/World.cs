using System;
using System.Collections;
using System.Collections.Generic;
using Octree;
using Unity.Entities;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    public Octree.BoundsOctree<Entity> EntitiesOctTree;

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
}
