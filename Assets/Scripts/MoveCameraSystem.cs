using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
public partial class MoveCameraSystem : SystemBase
{
	protected override void OnStartRunning()
	{
		
	}

	protected override void OnUpdate()
	{
		Entities.ForEach((ref LocalTransform transform, ref Ship ship) =>
			{
				Transform cameraTransform = CameraManager.Instance.Camera.transform;
				cameraTransform.forward = transform.Forward();
				cameraTransform.position = transform.Position;
				cameraTransform.position += -(Vector3)transform.Forward() * 10f;
				cameraTransform.RotateAround(transform.Position, transform.Right(), 20);
			}).Run();
	}
}