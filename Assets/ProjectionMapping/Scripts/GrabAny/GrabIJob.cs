using EugeneC.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using static Unity.Physics.Math;

namespace ProjectionMapping {
	[BurstCompile]
	public struct GrabIJob : IJob {
		[ReadOnly] public CollisionWorld CollisionWorld;
		[ReadOnly] public bool IgnoreTriggers;
		[ReadOnly] public bool IgnoreStatic;
		[ReadOnly] public float3 Origin;

		public NativeReference<GrabbableData> GrabRef;
		public RaycastInput RayInput;

		public void Execute() {
			var pickCollector = new PhysicsColliderICollector(CollisionWorld.NumDynamicBodies) {
				IgnoreTriggers = IgnoreTriggers,
				IgnoreStatic = IgnoreStatic
			};

			if (CollisionWorld.CastRay(RayInput, ref pickCollector)) {
				var hitBody = CollisionWorld.Bodies[pickCollector.Hit.RigidBodyIndex];

				//Grab that specific point on the body instead of the center
				float3 pointOnBody;
				{
					//Convert world transform to local transform
					var localTrans = Inverse(new MTransform(hitBody.WorldFromBody));
					pointOnBody = Mul(localTrans, pickCollector.Hit.Position);
				}

				GrabRef.Value = new GrabbableData {
					Valid = true,
					Origin = Origin,
					Target = hitBody.Entity,
					PointOnBody = pointOnBody
				};
			}
			else {
				GrabRef.Value = new GrabbableData {
					Valid = false
				};
			}
		}
	}
}