using System;
using EugeneC.ECS;
using EugeneC.Utilities;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

namespace ProjectionMapping {
	[UpdateInGroup(typeof(Eu_PreTransformSystemGroup))]
	public partial class HandGrabAnyInputSystemBase : SystemBase {
		private const float CastMagnitude = 1000f;

		public readonly NativeReference<GrabbableData>[] GrabRefs =
			new NativeReference<GrabbableData>[Enum.GetValues(typeof(ETrackingTarget)).Length];

		public readonly JobHandle[] PickJobHandles =
			new JobHandle[Enum.GetValues(typeof(ETrackingTarget)).Length];

		protected override void OnCreate() {
			RequireForUpdate<ColliderCastISingleton>();
			RequireForUpdate<PhysicsWorldSingleton>();
			RequireForUpdate<HandTrackingISingleton>();

			for (var i = 0; i < GrabRefs.Length; i++)
				GrabRefs[i] = new NativeReference<GrabbableData>(Allocator.Persistent);
		}

		protected override void OnUpdate() {
			if (CameraController.Instance is null) return;

			var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
			var cast = SystemAPI.GetSingleton<ColliderCastISingleton>();
			var tracking = SystemAPI.GetSingleton<HandTrackingISingleton>();
			var dir = (float3)CameraController.Instance.transform.forward;

			foreach (ETrackingTarget t in Enum.GetValues(typeof(ETrackingTarget))) {
				var (value, pos) = tracking.GetValue(t);
				var idx = (int)t;

				switch (value) {
					case -1f:
						continue;

					case < 1f when tracking.GetPrevious(t) > 1: {
						// Ensure any previous pick for this hand finished before rescheduling into the same nativeref.
						PickJobHandles[idx].Complete();

						Dependency = new GrabIJob {
							CollisionWorld = physicsWorld.CollisionWorld,
							IgnoreStatic = cast.IgnoreStatic,
							IgnoreTriggers = cast.IgnoreTriggers,

							GrabRef = GrabRefs[idx],
							Origin = pos,
							RayInput = new RaycastInput {
								Start = pos,
								End = pos + dir * CastMagnitude,
								Filter = CollisionFilter.Default
							}
						}.Schedule(Dependency);

						PickJobHandles[idx] = Dependency;
						break;
					}

					case > 1f when tracking.GetPrevious(t) < 1: {
						PickJobHandles[idx].Complete();
						GrabRefs[idx].Value = new GrabbableData { Valid = false };
						PickJobHandles[idx] = default;
						break;
					}
				}
			}
		}

		protected override void OnDestroy() {
			for (var i = 0; i < GrabRefs.Length; i++)
				if (GrabRefs[i].IsCreated)
					GrabRefs[i].Dispose();
		}
	}
}