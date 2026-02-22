using EugeneC.ECS;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace ProjectionMapping
{
	[UpdateInGroup(typeof(Eu_PostTransformSystemGroup))]
	public partial class HandGrabAnyFollowSystemBase : SystemBase
	{
		private HandGrabAnyInputSystemBase _grabSystemBase;

		protected override void OnCreate()
		{
			_grabSystemBase = World.GetOrCreateSystemManaged<HandGrabAnyInputSystemBase>();
			RequireForUpdate<ColliderCastISingleton>();
			RequireForUpdate<HandTrackingISingleton>();
		}

		protected override void OnUpdate()
		{
			// Complete all scheduled pick jobs first, then read their results on the main thread.
			var combined = Dependency;
			foreach (var t in _grabSystemBase.PickJobHandles)
				combined = JobHandle.CombineDependencies(combined, t);

			combined.Complete();

			for (var i = 0; i < _grabSystemBase.PickJobHandles.Length; i++)
			{
				if (_grabSystemBase.PickJobHandles[i].Equals(default(JobHandle))) continue;
				_grabSystemBase.PickJobHandles[i] = default;
			}

			var cast = SystemAPI.GetSingleton<ColliderCastISingleton>();
			var refs = _grabSystemBase.GrabRefs;

			for (var i = 0; i < refs.Length; i++)
			{
				if (!refs[i].Value.Valid) continue;
				var entity = refs[i].Value.Target;
				var destination = refs[i].Value.Origin;

				if (cast.DeleteEntityOnClick)
				{
					EntityManager.DestroyEntity(entity);
					refs[i].Value = new GrabbableData
					{
						Valid = false
					};
					continue;
				}

				if (cast.DeleteTagEntityOnClick && SystemAPI.HasComponent<DestroyIEnableableTag>(entity))
				{
					SystemAPI.SetComponentEnabled<DestroyIEnableableTag>(entity, true);
					refs[i].Value = new GrabbableData
					{
						Valid = false
					};
					continue;
				}

				if (!SystemAPI.HasComponent<PhysicsMass>(entity)) continue;
				if (!SystemAPI.HasComponent<HandGrabbableIData>(entity)) continue;
				if (SystemAPI.HasComponent<PhysicsMassOverride>(entity)) continue;

				var mass = SystemAPI.GetComponent<PhysicsMass>(entity);
				var massOverride = SystemAPI.GetComponentLookup<PhysicsMassOverride>(true);
				var vel = SystemAPI.GetComponent<PhysicsVelocity>(entity);
				var lt = SystemAPI.GetComponent<LocalTransform>(entity);

				if (mass.HasInfiniteMass ||
				    massOverride.HasComponent(entity) && massOverride[entity].IsKinematic != 0) continue;
				var worldFromBody = new Math.MTransform(lt.Rotation, lt.Position);

				var bodyFromMotion = new Math.MTransform(mass.InertiaOrientation, mass.CenterOfMass);
				var worldFromMotion = Math.Mul(worldFromBody, bodyFromMotion);

				const float gain = 0.95f;
				vel.Linear *= gain;
				vel.Angular *= gain;

				var bodyCenterNPointWorldPos = Math.Mul(worldFromBody, refs[i].Value.PointOnBody);

				var bodyCenterNPointLocalPos = Math.Mul(Math.Inverse(bodyFromMotion), refs[i].Value.PointOnBody);
				float3 deltaVel;
				{
					var diff = bodyCenterNPointWorldPos - destination;
					float3 relativeVelInWorld;
					{
						var tangentVel = math.cross(vel.Angular, bodyCenterNPointLocalPos);
						var relativeVelInBody = vel.Linear + math.mul(worldFromMotion.Rotation, tangentVel);
						relativeVelInWorld = Math.Mul(worldFromMotion, relativeVelInBody);
					}

					const float elasticity = 0.1f;
					const float damping = 0.5f;
					deltaVel = -diff * (elasticity / SystemAPI.Time.DeltaTime) - damping * relativeVelInWorld;
				}

				float3x3 effectiveMassMatrix;
				{
					float3 arm = bodyCenterNPointWorldPos - worldFromMotion.Translation;
					var skew = new float3x3(
						new float3(0.0f, arm.z, -arm.y),
						new float3(-arm.z, 0.0f, arm.x),
						new float3(arm.y, -arm.x, 0.0f)
					);

					// world space inertia = worldFromMotion * inertiaInMotionSpace * motionFromWorld
					var invInertiaWs = new float3x3(
						mass.InverseInertia.x * worldFromMotion.Rotation.c0,
						mass.InverseInertia.y * worldFromMotion.Rotation.c1,
						mass.InverseInertia.z * worldFromMotion.Rotation.c2
					);
					invInertiaWs = math.mul(invInertiaWs, math.transpose(worldFromMotion.Rotation));

					float3x3 invEffMassMatrix = math.mul(math.mul(skew, invInertiaWs), skew);
					invEffMassMatrix.c0 = new float3(mass.InverseMass, 0.0f, 0.0f) - invEffMassMatrix.c0;
					invEffMassMatrix.c1 = new float3(0.0f, mass.InverseMass, 0.0f) - invEffMassMatrix.c1;
					invEffMassMatrix.c2 = new float3(0.0f, 0.0f, mass.InverseMass) - invEffMassMatrix.c2;

					effectiveMassMatrix = math.inverse(invEffMassMatrix);
				}

				// Calculate impulse to cause the desired change in velocity
				var impulse = math.mul(effectiveMassMatrix, deltaVel);

				// Clip the impulse
				const float maxAcceleration = 250.0f;
				float maxImpulse = math.rcp(mass.InverseMass) * SystemAPI.Time.DeltaTime * maxAcceleration;
				impulse *= math.min(1.0f, math.sqrt((maxImpulse * maxImpulse) / math.lengthsq(impulse)));
				{
					vel.Linear += impulse * mass.InverseMass;

					float3 impulseLs = math.mul(math.transpose(worldFromMotion.Rotation), impulse);
					float3 angularImpulseLs = math.cross(bodyCenterNPointLocalPos, impulseLs);
					vel.Angular += angularImpulseLs * mass.InverseInertia;
				}

				SystemAPI.SetComponent(entity, vel);
			}
		}
	}
}