using EugeneC.ECS;
using EugeneC.Utilities;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

namespace ProjectionMapping
{
	[UpdateInGroup(typeof(Eu_PreTransformSystemGroup))]
	public partial class HandGestureSystemGroup : ComponentSystemGroup{ }
	
	[UpdateInGroup(typeof(HandGestureSystemGroup), OrderFirst = true)]
	public partial class HandGestureInteractSystemBase : SystemBase
	{
		public JobHandle LeftDependency;
		public JobHandle RightDependency;
		
		public NativeReference<GestureData> LeftDataRef = new(Allocator.Persistent);
		public NativeReference<GestureData> RightDataRef = new(Allocator.Persistent);
		
		private bool _leftScheduled;
		private bool _rightScheduled;
		
		private const float CastMagnitude = 1000f;

		protected override void OnCreate()
		{
			RequireForUpdate<ColliderCastISingleton>();
			RequireForUpdate<PhysicsWorldSingleton>();
			RequireForUpdate<HandSettingISingleton>();
			RequireForUpdate<HandPoseISingleton>();
		}

		protected override void OnUpdate()
		{
			if (CameraController.Instance is null) return;

			var settings = SystemAPI.GetSingleton<HandSettingISingleton>();
			var pose = SystemAPI.GetSingleton<HandPoseISingleton>();
			
			var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
			var cast = SystemAPI.GetSingleton<ColliderCastISingleton>();
			var dir = (float3)CameraController.Instance.transform.forward;
			
			if (_leftScheduled) LeftDependency.Complete();
			if (_rightScheduled) RightDependency.Complete();
			_leftScheduled = false;
			_rightScheduled = false;
			
			if (pose.LeftCurrentHandPose == settings.PickGesture && pose.LeftPreviousHandPose != settings.PickGesture)
			{
				var pos = pose.LeftOrigin;

				Dependency = new EntityCastIJob
				{
					CollisionWorld = physicsWorld.CollisionWorld,
					IgnoreStatic = cast.IgnoreStatic,
					IgnoreTriggers = cast.IgnoreTriggers,
					
					GrabLookup = SystemAPI.GetComponentLookup<HandGrabbableIData>(true),
					DataRef = LeftDataRef,
					RayInput = new RaycastInput
					{
						Start = pos,
						End = pos + dir * CastMagnitude,
						Filter = CollisionFilter.Default
					}

				}.Schedule(Dependency);
				
				LeftDependency = Dependency;
				_leftScheduled = true;
			}
			else if (pose.LeftCurrentHandPose != settings.PickGesture && pose.LeftPreviousHandPose == settings.PickGesture)
			{
				LeftDependency.Complete();
				if (LeftDataRef.Value.GrabEntity != Entity.Null)
				{
					var grab = SystemAPI.GetComponent<HandGrabbableIData>(LeftDataRef.Value.GrabEntity);
					grab.IsGrabbed = false;
					SystemAPI.SetComponent(LeftDataRef.Value.GrabEntity, grab);
				}
				
				LeftDataRef.Value = new GestureData()
				{
					Valid = false
				};
				LeftDependency = default;
			}
			
			if (pose.RightCurrentHandPose == settings.PickGesture && pose.RightPreviousHandPose != settings.PickGesture)
			{
				var pos = pose.RightOrigin;

				Dependency = new EntityCastIJob
				{
					CollisionWorld = physicsWorld.CollisionWorld,
					IgnoreStatic = cast.IgnoreStatic,
					IgnoreTriggers = cast.IgnoreTriggers,
					
					GrabLookup = SystemAPI.GetComponentLookup<HandGrabbableIData>(true),
					DataRef = RightDataRef,
					RayInput = new RaycastInput
					{
						Start = pos,
						End = pos + dir * CastMagnitude,
						Filter = CollisionFilter.Default
					}

				}.Schedule(Dependency);
				
				RightDependency = Dependency;
				_rightScheduled = true;
			}
			else if (pose.RightCurrentHandPose != settings.PickGesture && pose.RightPreviousHandPose == settings.PickGesture)
			{
				RightDependency.Complete();
				if (RightDataRef.Value.GrabEntity != Entity.Null)
				{
					var grab = SystemAPI.GetComponent<HandGrabbableIData>(RightDataRef.Value.GrabEntity);
					grab.IsGrabbed = false;
					SystemAPI.SetComponent(RightDataRef.Value.GrabEntity, grab);	
				}
				
				RightDataRef.Value = new GestureData()
				{
					Valid = false
				};
				RightDependency = default;
			}
			
			settings.CamDirection = dir;
			SystemAPI.SetSingleton(settings);
		}

		protected override void OnDestroy()
		{
			LeftDataRef.Dispose();
			RightDataRef.Dispose();
		}
	}
}