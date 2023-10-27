using System;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[RequireMatchingQueriesForUpdate]
public partial class MoveShipSystem : SystemBase
{
	protected override void OnUpdate()
	{
		Entities.ForEach((ref LocalTransform transform, ref Ship ship) =>
		{
			float dT = SystemAPI.Time.DeltaTime;
			if (ship.FinishedPath)
			{
				return;
			}
			transform.Rotation = quaternion.LookRotation(PathManager.Instance.Velocities[ship.WaypointProgress], transform.Up());
			ship.Time += dT;
			var timeSum = PathManager.Instance.SegmentTimes.Sum() + 5f;
			var progress = Math.Clamp(SystemAPI.Time.ElapsedTime / timeSum, 0f, 1f);
			var formatString = String.Format("{0:0.##}", progress * 100f);
			float.TryParse(formatString, out float progressParsed);
			UIManager.Instance.SetProgressText(progressParsed);
			if (ship.Time >= 5f)
			{
				var segmentTime = PathManager.Instance.SegmentTimes[ship.WaypointProgress];
            
				transform.Position += PathManager.Instance.Velocities[ship.WaypointProgress] * dT;
				if (ship.Time >= segmentTime + 5f)
				{
					ship.WaypointProgress++;
					if (ship.WaypointProgress == PathManager.Instance.Velocities.Count)
					{
						ship.FinishedPath = true;
						return;
					}
					ship.Time = 5f;
				}
			}
		}).Run();
	}
}