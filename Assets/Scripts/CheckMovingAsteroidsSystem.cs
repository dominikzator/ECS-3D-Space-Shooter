using Octree;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Vector3 = System.Numerics.Vector3;

[BurstCompile]
[UpdateAfter(typeof(ShipShootSystem))]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct CheckMovingAsteroidsSystem : ISystem
{
	private static Octree.BoundingBox queryBox = new Octree.BoundingBox(Vector3.Zero, Vector3.Zero);
	
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
		state.RequireForUpdate<Ship>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		World.Instance.MovingAsteroidsInRange.Clear();
		foreach (var (transform, movingAsteroid, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MovingAsteroid>>().WithEntityAccess())
		{
			queryBox = new BoundingBox(new Vector3(transform.ValueRO.Position.x, transform.ValueRO.Position.y, transform.ValueRO.Position.z),
				new Vector3(transform.ValueRO.Scale, transform.ValueRO.Scale, transform.ValueRO.Scale));

			if (World.Instance.QueryBox.Intersects(queryBox))
			{
				World.Instance.MovingAsteroidsInRange.Add(entity);
			}
		}
	}
}