using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using Random = Unity.Mathematics.Random;

[RequireMatchingQueriesForUpdate]
public partial class MoveShipSystem : SystemBase
{
	protected override void OnUpdate()
	{
		float dT = SystemAPI.Time.DeltaTime;
		Entities.ForEach((ref ShipAspect ship) =>
		{
			//ship.Move(dT);
		}).ScheduleParallel();
	}
}