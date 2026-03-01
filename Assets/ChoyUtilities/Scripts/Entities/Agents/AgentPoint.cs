using System;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ECS
{
	[DisallowMultipleComponent]
	public sealed class AgentPoint : MonoBehaviour
	{
		public EAgentSpeed agentSpeed;
		public float overrideSpeed;
		public float pointThreshold = 0.2f;
		
		public float3 position => transform.position;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, pointThreshold);
		}
	}

	[Serializable]
	public struct PointSerializable
	{
		public Color color;
		public EBakingLineType bakingLineType;
		public AgentPoint[] agentPoints;
	}

	public enum EBakingLineType : byte
	{
		Straight,
		Curved
	}

	public enum EAgentSpeed : byte
	{
		Legacy,
		Override
	}
}