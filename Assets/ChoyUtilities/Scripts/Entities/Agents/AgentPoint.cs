using System;
using UnityEngine;

namespace EugeneC.ECS
{
	[DisallowMultipleComponent]
	public sealed class AgentPoint : MonoBehaviour
	{
		public EBakingLineType bakingLineType;
		public EAgentSpeed agentSpeed;
		public float overrideSpeed;
		public float pointThreshold = 0.2f;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, pointThreshold);
		}
	}

	[Serializable]
	public struct PointSerializable
	{
		public AgentPoint[] points;
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