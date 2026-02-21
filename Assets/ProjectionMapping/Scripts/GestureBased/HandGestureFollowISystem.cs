using EugeneC.ECS;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ProjectionMapping
{
	[UpdateInGroup(typeof(HandGestureSystemGroup))]
	[UpdateAfter(typeof(HandGestureInteractSystemBase))]
	public partial class HandGestureFollowISystem : SystemBase
	{
		private HandGestureInteractSystemBase _interactSystemBase;
		
		protected override void OnCreate()
		{
			_interactSystemBase = World.GetOrCreateSystemManaged<HandGestureInteractSystemBase>();
			RequireForUpdate<HandSettingISingleton>();
			RequireForUpdate<HandPoseISingleton>();
			RequireForUpdate<ColliderCastISingleton>();
		}
		
		protected override void OnUpdate()
		{
			var combined = Dependency;
			JobHandle.CombineDependencies(combined, _interactSystemBase.LeftDependency);
			JobHandle.CombineDependencies(combined, _interactSystemBase.RightDependency);
			
			_interactSystemBase.LeftDependency = default;
			_interactSystemBase.RightDependency = default;
			combined.Complete();
			
			var dt = SystemAPI.Time.DeltaTime;
			var settings = SystemAPI.GetSingleton<HandSettingISingleton>();
			var dir = settings.CamDirection;
			var pose = SystemAPI.GetSingleton<HandPoseISingleton>();
			

			if (_interactSystemBase.LeftDataRef.Value.Valid)
			{
				var e = _interactSystemBase.LeftDataRef.Value.GrabEntity;
				var lt = SystemAPI.GetComponent<LocalTransform>(e);
				MoveEntity(pose.LeftOrigin, dir, _interactSystemBase.LeftDataRef.Value.Distance, 
					 ref lt, dt);
				SystemAPI.SetComponent(e, lt);
			}

			if (_interactSystemBase.RightDataRef.Value.Valid)
			{
				var e = _interactSystemBase.RightDataRef.Value.GrabEntity;
				var lt = SystemAPI.GetComponent<LocalTransform>(e);
				MoveEntity(pose.RightOrigin, dir, _interactSystemBase.RightDataRef.Value.Distance, 
					 ref lt, dt);
				SystemAPI.SetComponent(e, lt);
			}
		}

		private void MoveEntity(float3 origin, float3 camDir, 
			float dis, ref LocalTransform lt, float dt, float deltaSpeed = 5f)
		{
			lt.Position = math.lerp(lt.Position, origin + camDir * dis, deltaSpeed * dt);
		}
	}
}