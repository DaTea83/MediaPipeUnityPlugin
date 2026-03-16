using EugeneC.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

namespace ProjectionMapping {
	[BurstCompile]
	public struct EntityCastIJob : IJob {
		[ReadOnly] public CollisionWorld CollisionWorld;
		[ReadOnly] public bool IgnoreTriggers;
		[ReadOnly] public bool IgnoreStatic;
		[ReadOnly] public ComponentLookup<HandGrabbableIData> GrabLookup;

		public NativeReference<GestureData> DataRef;
		public RaycastInput RayInput;

		public void Execute() {
			var pickCollector = new PhysicsColliderICollector(CollisionWorld.NumDynamicBodies) {
				IgnoreTriggers = IgnoreTriggers,
				IgnoreStatic = IgnoreStatic
			};

			if (!CollisionWorld.CastRay(RayInput, ref pickCollector)) return;
			var hitBody = CollisionWorld.Bodies[pickCollector.Hit.RigidBodyIndex];
			if (!GrabLookup.HasComponent(hitBody.Entity)) return;
			// Make sure only one hand can access the entity at the same time
			if (GrabLookup[hitBody.Entity].IsGrabbed) return;

			DataRef.Value = new GestureData {
				GrabEntity = hitBody.Entity,
				Distance = math.distance(hitBody.WorldFromBody.pos, RayInput.Start),
				Valid = true
			};
		}
	}

	public struct GestureData {
		public bool Valid;
		public Entity GrabEntity;
		public float Distance;
	}
}