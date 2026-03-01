using System;
using System.Collections.Generic;
using EugeneC.Utilities;
using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS
{
	[DisallowMultipleComponent]
	public abstract class AgentPathwaysSingletonAuthoring<T, TEnum> : MonoBehaviour
		where T : AgentPathwaysSingletonAuthoring<T, TEnum>
		where TEnum : Enum
	{
		[Serializable]
		public struct AgentsSerializable
		{
			public TEnum type;
			public PointSerializable[] points;
		}
		
		[DisallowMultipleComponent]
		[RequireComponent(typeof(RandomAuthoring))]
		public abstract class AgentMovementAuthoring<TA> : MonoBehaviour
			where TA : AgentMovementAuthoring<TA>
		{
			public abstract TEnum AgentEnum { get; }
			[SerializeField] protected AgentScriptable stats;

			private readonly T _pathwaysSingleton;

			protected AgentMovementAuthoring(T pathwaysSingleton)
			{
				_pathwaysSingleton = pathwaysSingleton;
			}
		}
		
		[SerializeField] protected AgentsSerializable[] agents;
	}

	public struct AgentPathwaysISingleton : IComponentData
	{
		public BlobAssetReference<BlobArray<SplineVectorBlob>> Pathways;
	}

	public struct AgentMovementIData : IComponentData
	{
		public byte MovementType;
		public float Speed;
	}
}