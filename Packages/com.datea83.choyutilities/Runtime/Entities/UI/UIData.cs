using Unity.Entities;

namespace EugeneC.ECS {
	
	public struct UIData : IComponentData {
		public byte ParentId;
		public byte OwnId;
	}

	[InternalBufferCapacity(1)]
	public struct UIBuffer : IBufferElementData {
		public byte Value;
	}
}