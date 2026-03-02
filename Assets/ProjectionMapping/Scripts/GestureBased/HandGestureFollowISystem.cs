using EugeneC.ECS;
using EugeneC.Utilities;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
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
			
			if (CameraController.Instance is null) return;
			
			var dt = SystemAPI.Time.DeltaTime;
			var dir = (float3)CameraController.Instance.transform.forward;
			var pose = SystemAPI.GetSingleton<HandPoseISingleton>();
			

			if (_interactSystemBase.LeftDataRef.Value.Valid)
			{
				var e = _interactSystemBase.LeftDataRef.Value.GrabEntity;
				var grab = SystemAPI.GetComponent<HandGrabbableIData>(e);
				if (!grab.IsGrabbed)
				{
					grab.IsGrabbed = true;
					SystemAPI.SetComponent(e, grab);
				}
				var lt = SystemAPI.GetComponent<LocalTransform>(e);
				MoveEntity(pose.LeftLocalPosition, dir, _interactSystemBase.LeftDataRef.Value.Distance * 0.9f, 
					 ref lt, dt);
				SystemAPI.SetComponent(e, lt);
			}

			if (_interactSystemBase.RightDataRef.Value.Valid)
			{
				var e = _interactSystemBase.RightDataRef.Value.GrabEntity;
				var grab = SystemAPI.GetComponent<HandGrabbableIData>(e);
				if (!grab.IsGrabbed)
				{
					grab.IsGrabbed = true;
					SystemAPI.SetComponent(e, grab);
				}
				var lt = SystemAPI.GetComponent<LocalTransform>(e);
				MoveEntity(pose.RightLocalPosition, dir, _interactSystemBase.RightDataRef.Value.Distance * 0.9f, 
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