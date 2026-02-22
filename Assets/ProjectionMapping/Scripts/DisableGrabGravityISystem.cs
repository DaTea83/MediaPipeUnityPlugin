using EugeneC.ECS;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace ProjectionMapping
{
	[UpdateInGroup(typeof(Eu_PhysicsSystemGroup))]
	public partial struct DisableGrabGravityISystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			foreach (var (grab, gravity) 
			         in SystemAPI.Query<RefRO<HandGrabbableIData>, RefRW<PhysicsGravityFactor>>())
			{
				gravity.ValueRW.Value = grab.ValueRO.IsGrabbed ? 0 : 1;
			}
		}
	}
}