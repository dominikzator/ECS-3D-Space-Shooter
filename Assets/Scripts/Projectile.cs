using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Projectile : IComponentData
{
    public float3 LinearVelocity;
    public float Radius;
}
