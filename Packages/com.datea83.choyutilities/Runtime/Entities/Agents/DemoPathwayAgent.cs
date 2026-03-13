using Unity.Entities;

namespace EugeneC.ECS
{
	public class DemoPathwayAgent : DemoPathwayControllerAuthoring.AgentMovementBase
	{
		public override DemoPathwayControllerAuthoring.EPathway AgentEnum => DemoPathwayControllerAuthoring.EPathway.Type1;
		
		private class DemoPathwayAgentBaker : Baker<DemoPathwayAgent>
		{
			public override void Bake(DemoPathwayAgent authoring)
			{
				var e = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(e, new AgentMovementIData
				{
					PathwayId = (byte)authoring.AgentEnum,
					Speed = authoring.stats.speed,
				});
			}
		}
	}
}