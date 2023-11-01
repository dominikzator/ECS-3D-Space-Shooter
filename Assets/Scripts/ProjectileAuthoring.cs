using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour
{
    
}
class ProjectileBaker : Baker<ProjectileAuthoring>
{
	public override void Bake(ProjectileAuthoring authoring)
	{
		//var entity = GetEntity(TransformUsageFlags.Dynamic);
		//AddComponent(entity, new Asteroid{});
		//AddComponent(entity, new LocalTransform{Position = Vector3.zero, Scale = 5f, Rotation = quaternion.identity});
	}
}