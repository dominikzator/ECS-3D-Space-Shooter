using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[RequireMatchingQueriesForUpdate]
public partial class MoveCameraSystem : SystemBase
{
	protected override void OnStartRunning()
	{
		var world = World.DefaultGameObjectInjectionWorld;
	}

	protected override void OnUpdate()
	{
		Entities.ForEach((ref LocalTransform transform, ref Ship ship) =>
			{
				CameraManager.Instance.AlignCamera(transform.Position, transform.Forward(), transform.Right());
			}).Run();
	}
}