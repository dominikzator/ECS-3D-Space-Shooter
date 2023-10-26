using System.IO;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct ShipAspect : IAspect
{
	public readonly Entity Entity;
    
	private readonly RefRW<LocalTransform> _transform;
	private readonly RefRW<Ship> ship;
    
	public float3 Position => _transform.ValueRO.Position;
	public float3 ForwardVector => _transform.ValueRO.Forward();
	public float3 RightVector => _transform.ValueRO.Right();
	public int WaypointProgress => ship.ValueRW.WaypointProgress;
	public float Time => ship.ValueRW.Time;

	public bool FinishedPath => ship.ValueRW.FinishedPath;

	public void Move(float deltaTime)
	{
		if (FinishedPath)
		{
			return;
		}
		SetRotation();
		ship.ValueRW.Time += deltaTime;
		var timeSum = PathManager.Instance.SegmentTimes.Sum() + 5f;
		if (Time >= 5f)
		{
			var progress = ship.ValueRW.Time / timeSum;
			UIManager.Instance.SetProgressText(progress);
			var segmentTime = PathManager.Instance.SegmentTimes[WaypointProgress];
            
			_transform.ValueRW.Position += PathManager.Instance.Velocities[WaypointProgress] * deltaTime;
			if (Time >= segmentTime + 5f)
			{
				ship.ValueRW.WaypointProgress++;
				if (ship.ValueRW.WaypointProgress == PathManager.Instance.Velocities.Count)
				{
					ship.ValueRW.FinishedPath = true;
					return;
				}
				ship.ValueRW.Time = 5f;
			}
		}
	}
	private void SetRotation()
	{
		_transform.ValueRW.Rotation = quaternion.LookRotation(PathManager.Instance.Velocities[WaypointProgress], _transform.ValueRW.Up());
	}
}