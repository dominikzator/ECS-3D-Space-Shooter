using Unity.Burst;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct MoveAsteroidsSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		
	}
	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var deltaTime = SystemAPI.Time.DeltaTime;

		new MoveAsteroidsJob()
		{
			DeltaTime = deltaTime,
			Random = Random.CreateFromIndex((uint)UnityEngine.Random.Range(0, 100))
		}.ScheduleParallel();
	}
}

[BurstCompile]
public partial struct MoveAsteroidsJob : IJobEntity
{
	public float DeltaTime;
	public Random Random;
    
	[BurstCompile]
	private void Execute(AsteroidAspect asteroid, [EntityIndexInQuery] int sortKey)
	{
		asteroid.Move(DeltaTime, Random);
	}
}