using System;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct MoveShipSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<Ship>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var deltaTime = SystemAPI.Time.DeltaTime;
		var timeElapsed = SystemAPI.Time.ElapsedTime;

		foreach (var (transform, ship) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Ship>>())
		{
			if (ship.ValueRO.FinishedPath)
			{
				return;
			}
			
			transform.ValueRW.Rotation = quaternion.LookRotation(PathManager.Instance.Velocities[ship.ValueRW.WaypointProgress], transform.ValueRO.Up());
			ship.ValueRW.Time += deltaTime;
			var timeSum = PathManager.Instance.SegmentTimes.Sum() + 5f;
			var progress = Math.Clamp(timeElapsed / timeSum, 0f, 1f);
			var formatString = String.Format("{0:0.##}", progress * 100f);
			float.TryParse(formatString, out float progressParsed);
			UIManager.Instance.SetProgressText(progressParsed);
			if (ship.ValueRO.Time >= 5f)
			{
				var segmentTime = PathManager.Instance.SegmentTimes[ship.ValueRO.WaypointProgress];
            
				transform.ValueRW.Position += PathManager.Instance.Velocities[ship.ValueRO.WaypointProgress] * deltaTime;
				if (ship.ValueRO.Time >= segmentTime + 5f)
				{
					ship.ValueRW.WaypointProgress++;
					if (ship.ValueRO.WaypointProgress == PathManager.Instance.Velocities.Count)
					{
						ship.ValueRW.FinishedPath = true;
						return;
					}
					ship.ValueRW.Time = 5f;
				}
			}
		}
	}
}