using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ECS
{
	[DisallowMultipleComponent]
	public sealed class TextGridSpawnAuthoring : MonoBehaviour
	{
		[SerializeField] private GameObject[] prefabs;
		[SerializeField] private char[] identifiers;
		[SerializeField] private TextAsset textAsset;
		[SerializeField] private float2 spacing = new float2(1f, 1f);
		[SerializeField] private float scale = 1f;
		
		private class TextGridSpawnAuthoringBaker : Baker<TextGridSpawnAuthoring>
		{
			public override void Bake(TextGridSpawnAuthoring authoring)
			{
				var e = GetEntity(TransformUsageFlags.Dynamic);

				var builder = new BlobBuilder(Allocator.Temp);
				var pattern = MakeBlob(authoring, ref builder);
				builder.Dispose();
				AddBlobAsset(ref pattern, out _);
				
				AddComponent(e, new TextGridSpawnIData
				{
					Pattern = pattern,
					Scale = authoring.scale,
					Spacing = authoring.spacing
				});

				var buffer = AddBuffer<TextGridIBuffer>(e);
				for (ushort i = 0; i < authoring.prefabs.Length; i++)
				{
					var p = GetEntity(authoring.prefabs[i], TransformUsageFlags.Dynamic);
					buffer.Add(new TextGridIBuffer
					{
						Id = i,
						Prefab = p
					});
				}
			}

			private static BlobAssetReference<BlobArray<FixedList512Bytes<byte>>> MakeBlob(TextGridSpawnAuthoring authoring, ref BlobBuilder builder)
			{
				builder = new BlobBuilder(Allocator.Temp);
				ref var patternBlob = ref builder.ConstructRoot<BlobArray<FixedList512Bytes<byte>>>();
				
				var arrayBuilder = builder.Allocate(ref patternBlob, 1);
				var patternString = authoring.textAsset.text;
				var patternList = new FixedList512Bytes<byte>();
				var patternId = authoring.identifiers;

				foreach (var c in patternString)
				{
					if(char.IsControl(c)) continue;
					if(c == '\n') continue;

					for (var i = 0; i < patternId.Length; i++)
					{
						if (patternId[i] != c) continue;
						
						var patternByte = (byte)i;
						patternList.Add(patternByte);
					}
					
				}

				arrayBuilder[0] = patternList;
				return builder.CreateBlobAssetReference<BlobArray<FixedList512Bytes<byte>>>(Allocator.Persistent);
			}
		}
	}

	public struct TextGridSpawnIData : IComponentData
	{
		public BlobAssetReference<BlobArray<FixedList512Bytes<byte>>> Pattern;
		public float2 Spacing;
		public float Scale;
	}

	public struct TextGridIBuffer : IBufferElementData
	{
		public ushort Id;
		public Entity Prefab;
	}

	[UpdateInGroup(typeof(Eu_InitializationSystemGroup), OrderFirst = true)]
	[UpdateAfter(typeof(InitializeRandomISystem))]
	public partial struct TextGridSpawnISystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			
		}
	}
}