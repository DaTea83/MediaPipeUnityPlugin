using UnityEngine;

namespace EugeneC.Utilities
{
	[CreateAssetMenu(fileName = "AgentScriptable", menuName = "Choy Utilities/Agents", order = 0)]
	public class AgentScriptable : ScriptableObject
	{
		public float speed = 10f;
	}
}