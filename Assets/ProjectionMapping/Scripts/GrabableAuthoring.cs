using Unity.Entities;
using UnityEngine;

namespace ProjectionMapping {
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public sealed class GrabableAuthoring : MonoBehaviour {
		private Collider _collider;
		private Rigidbody _rb;

		private void OnValidate() {
			_rb = GetComponent<Rigidbody>();
			_collider = GetComponent<Collider>();
		}

		private class GrabableAuthoringBaker : Baker<GrabableAuthoring> {
			public override void Bake(GrabableAuthoring authoring) {
				var e = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(e, new HandGrabbableIData {
					IsTrigger = authoring._collider.isTrigger
				});
				AddBuffer<GrabbableOverlapIBuffer>(e);
			}
		}
	}
}