using Unity.Entities;
using Unity.Transforms;

public readonly partial struct ShipAspect : IAspect
{
	public readonly Entity Entity;
    
	public readonly RefRW<LocalTransform> Transform;
	public readonly RefRW<Ship> Ship;
}