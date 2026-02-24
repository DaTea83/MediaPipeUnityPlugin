using EugeneC.ECS;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ChessGame
{
	[DisallowMultipleComponent]
	public sealed class SpawnDelayEntityAuthoring : MonoBehaviour
	{
		[SerializeField] private float delay = 3f;
		[SerializeField] private ESpawnType spawnType = ESpawnType.Player;
		[SerializeField] private GameObject playerPrefab;
		[SerializeField] private GameObject guardPrefab;
		[SerializeField] private float3 offset = new float3(0f, 2f, 0f);
		
		private class SpawnPlayerTimerAuthoringBaker : Baker<SpawnDelayEntityAuthoring>
		{
			public override void Bake(SpawnDelayEntityAuthoring authoring)
			{
				var e = GetEntity(TransformUsageFlags.Dynamic);

				Entity prefab;
				switch (authoring.spawnType)
				{
					case ESpawnType.Player:
						prefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic);
						break;
					case ESpawnType.Guards:
						prefab = GetEntity(authoring.guardPrefab, TransformUsageFlags.Dynamic);
						break;
					default:
						prefab = Entity.Null;
						return;
				}
				
				AddComponent(e, new SpawnDelayEntityIData
				{
					Time = authoring.delay,
					Prefab = prefab,
					Offset = authoring.offset
				});
			}
		}
	}

	public struct SpawnDelayEntityIData : IComponentData
	{
		public float Time;
		public Entity Prefab;
		public float3 Offset;
	}

	[BurstCompile]
	[UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
	public partial struct SpawnDelayEntityISystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

			foreach (var (spawn, ltw, entity) 
			         in SystemAPI.Query<RefRW<SpawnDelayEntityIData>, RefRO<LocalToWorld>>().WithEntityAccess())
			{
				spawn.ValueRW.Time -= SystemAPI.Time.DeltaTime;
				if (spawn.ValueRO.Time > 0) continue;
				
				var newSpawn = ecb.Instantiate(spawn.ValueRO.Prefab);
				if (newSpawn == Entity.Null) continue;
				var nLt = LocalTransform.FromPositionRotation(ltw.ValueRO.Position + spawn.ValueRO.Offset, ltw.ValueRO.Rotation);
				
				ecb.SetComponent(newSpawn, nLt);
				ecb.RemoveComponent<SpawnDelayEntityIData>(entity);
			}
			ecb.Playback(state.EntityManager);
		}
	}
}