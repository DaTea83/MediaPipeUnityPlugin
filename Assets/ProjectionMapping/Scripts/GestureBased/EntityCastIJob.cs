using EugeneC.ECS;
using Mediapipe.Unity;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using static Unity.Physics.Math;

namespace ProjectionMapping
{
	[BurstCompile]
	public struct EntityCastIJob : IJob
	{
		[ReadOnly] public CollisionWorld CollisionWorld;
		[ReadOnly] public bool IgnoreTriggers;
		[ReadOnly] public bool IgnoreStatic;
		[ReadOnly] public ComponentLookup<HandGrabbableITag> GrabLookup;
		
		public NativeReference<GestureData> DataRef;
		public RaycastInput RayInput;
		
		public void Execute()
		{
			var pickCollector = new PhysicsColliderICollector(CollisionWorld.NumDynamicBodies)
			{
				IgnoreTriggers = IgnoreTriggers,
				IgnoreStatic = IgnoreStatic
			};

			if (!CollisionWorld.CastRay(RayInput, ref pickCollector)) return;
			var hitBody = CollisionWorld.Bodies[pickCollector.Hit.RigidBodyIndex];
			if (!GrabLookup.HasComponent(hitBody.Entity)) return;
			
			float3 pointOnBody;
			{
				//Convert world transform to local transform
				var localTrans = Inverse(new MTransform(hitBody.WorldFromBody));
				pointOnBody = Mul(localTrans, pickCollector.Hit.Position);
			}
			
			DataRef.Value = new GestureData()
			{
				GrabEntity = hitBody.Entity,
				Distance = math.distance(pointOnBody, RayInput.Start),
				Valid = true
			};
		}
	}

	public struct GestureData
	{
		public bool Valid;
		public Entity GrabEntity;
		public float Distance;
	}
}