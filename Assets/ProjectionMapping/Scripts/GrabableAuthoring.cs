using Unity.Entities;
using UnityEngine;

namespace ProjectionMapping
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public sealed class GrabableAuthoring : MonoBehaviour
	{
		private Rigidbody _rb;

		private void OnValidate()
		{
			_rb = GetComponent<Rigidbody>();
		}

		private class GrabableAuthoringBaker : Baker<GrabableAuthoring>
		{
			public override void Bake(GrabableAuthoring authoring)
			{
				var e = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent<HandGrabbableIData>(e);
				AddBuffer<GrabbableOverlapIBuffer>(e);
			}
		}
	}
}