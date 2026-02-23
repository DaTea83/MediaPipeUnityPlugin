using Unity.Entities;
using UnityEngine;

namespace ChessGame
{
	[DisallowMultipleComponent]
	public sealed class SpawnPlayerTimerAuthoring : MonoBehaviour
	{
		[SerializeField] private float delay;
		
		private class SpawnPlayerTimerAuthoringBaker : Baker<SpawnPlayerTimerAuthoring>
		{
			public override void Bake(SpawnPlayerTimerAuthoring authoring)
			{
				if (GameController.Instance.playerPrefab is null) return;
				
				var e = GetEntity(TransformUsageFlags.Dynamic);
				var prefab = GetEntity(GameController.Instance.playerPrefab, TransformUsageFlags.Dynamic);
			}
		}
	}
}