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
			_rb.useGravity = false;
		}

		private class GrabableAuthoringBaker : Baker<GrabableAuthoring>
		{
			public override void Bake(GrabableAuthoring authoring)
			{
				var e = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent<HandGrabbableITag>(e);
			}
		}
	}
}