using EugeneC.ECS;
using Mediapipe.Unity;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ProjectionMapping
{
	[BurstCompile]
	[UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
    public partial struct PointSpawnISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
			state.RequireForUpdate<PointSpawnISingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
	        var dt = SystemAPI.Time.DeltaTime;
	        var em = state.EntityManager;
	        var spawn = SystemAPI.GetSingleton<PointSpawnISingleton>();

	        foreach (var (point, hand, lt, entity) 
	                 in SystemAPI.Query<RefRW<PointSpawnIData>, RefRO<HandPointIData>, RefRO<LocalTransform>>()
		                 .WithEntityAccess())
	        {
		        if(hand.ValueRO.EHand == EHand.None) continue;
		        point.ValueRW.CurrentTime += dt;
		        if(point.ValueRO.CurrentTime <= 1 / spawn.Frequency) continue;
		        
		        var p = em.Instantiate(spawn.Prefab);
		        var pLt = em.GetComponentData<LocalTransform>(p);
		        
		        pLt.Position = lt.ValueRO.Position;
		        pLt.Scale = spawn.Scale;
		        
		        em.SetComponentData(p, pLt);
		        point.ValueRW.CurrentTime = 0;
	        }
        }
    }
}
