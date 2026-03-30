using EugeneC.ECS;
using EugeneC.Utilities;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

namespace ProjectionMapping {
	
	[UpdateInGroup(typeof(Eu_PreTransformSystemGroup))]
	public partial class HandGestureSystemGroup : ComponentSystemGroup { }

	[UpdateInGroup(typeof(HandGestureSystemGroup), OrderFirst = true)]
	public partial class HandGestureInteractSystemBase : SystemBase {
		
		private static readonly float2 ScreenSize = new(1600f, 1200f);
		private const float CastMagnitude = 1000f;

		public NativeReference<GestureData> LeftDataRef = new(Allocator.Persistent);
		public JobHandle LeftDependency;
		public NativeReference<GestureData> RightDataRef = new(Allocator.Persistent);
		public JobHandle RightDependency;

		protected override void OnCreate() {
			RequireForUpdate<ColliderCastISingleton>();
			RequireForUpdate<PhysicsWorldSingleton>();
			RequireForUpdate<HandSettingISingleton>();
			RequireForUpdate<HandPoseISingleton>();
			RequireForUpdate<HandScreenISingleton>();
		}

		protected override void OnUpdate() {
			if (CameraController.Instance is null) return;

			var settings = SystemAPI.GetSingleton<HandSettingISingleton>();
			var pose = SystemAPI.GetSingleton<HandPoseISingleton>();
			var screen = SystemAPI.GetSingleton<HandScreenISingleton>();

			var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
			var cast = SystemAPI.GetSingleton<ColliderCastISingleton>();
			var dir = (float3)CameraController.Instance.transform.forward;

			if (pose.LeftCurrentHandPose == settings.PickGesture && pose.LeftPreviousHandPose != settings.PickGesture) {
				var pos = pose.LeftLocalPosition;

				var leftHandle = new EntityCastIJob {
					CollisionWorld = physicsWorld.CollisionWorld,
					IgnoreStatic = cast.IgnoreStatic,
					IgnoreTriggers = cast.IgnoreTriggers,

					GrabLookup = SystemAPI.GetComponentLookup<HandGrabbableIData>(true),
					DataRef = LeftDataRef,
					RayInput = new RaycastInput {
						Start = pos,
						End = pos + dir * CastMagnitude,
						Filter = CollisionFilter.Default
					}
				}.Schedule(Dependency);

				LeftDependency = leftHandle;
				Dependency = JobHandle.CombineDependencies(Dependency, leftHandle);
			}
			else if (pose.LeftCurrentHandPose != settings.PickGesture &&
			         pose.LeftPreviousHandPose == settings.PickGesture) {
				CompleteHands();

				if (LeftDataRef.Value.GrabEntity != Entity.Null) {
					var grab = SystemAPI.GetComponent<HandGrabbableIData>(LeftDataRef.Value.GrabEntity);
					grab.IsGrabbed = false;
					SystemAPI.SetComponent(LeftDataRef.Value.GrabEntity, grab);
				}

				LeftDataRef.Value = new GestureData { Valid = false };
			}

			if (pose.RightCurrentHandPose == settings.PickGesture &&
			    pose.RightPreviousHandPose != settings.PickGesture) {
				var pos = pose.RightLocalPosition;

				var rightHandle = new EntityCastIJob {
					CollisionWorld = physicsWorld.CollisionWorld,
					IgnoreStatic = cast.IgnoreStatic,
					IgnoreTriggers = cast.IgnoreTriggers,

					GrabLookup = SystemAPI.GetComponentLookup<HandGrabbableIData>(true),
					DataRef = RightDataRef,
					RayInput = new RaycastInput {
						Start = pos,
						End = pos + dir * CastMagnitude,
						Filter = CollisionFilter.Default
					}
				}.Schedule(Dependency);

				RightDependency = rightHandle;
				Dependency = JobHandle.CombineDependencies(Dependency, rightHandle);
			}
			else if (pose.RightCurrentHandPose != settings.PickGesture &&
			         pose.RightPreviousHandPose == settings.PickGesture) {
				CompleteHands();

				if (RightDataRef.Value.GrabEntity != Entity.Null) {
					var grab = SystemAPI.GetComponent<HandGrabbableIData>(RightDataRef.Value.GrabEntity);
					grab.IsGrabbed = false;
					SystemAPI.SetComponent(RightDataRef.Value.GrabEntity, grab);
				}

				RightDataRef.Value = new GestureData { Valid = false };
			}

			SystemAPI.SetSingleton(settings);
		}

		protected override void OnDestroy() {
			CompleteHands();

			LeftDataRef.Dispose();
			RightDataRef.Dispose();
		}

		private void CompleteHands() {
			JobHandle.CombineDependencies(LeftDependency, RightDependency).Complete();
			LeftDependency = default;
			RightDependency = default;
		}
	}
}