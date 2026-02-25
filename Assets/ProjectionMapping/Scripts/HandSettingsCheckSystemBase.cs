using EugeneC.ECS;
using Unity.Entities;

namespace ProjectionMapping
{
	// Check once, if false disable related systems.
	[UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
	public partial class HandSettingsCheckSystemBase : SystemBase
	{
		private HandGrabAnyInputSystemBase _inputAnySystemBase;
		private HandGrabAnyFollowSystemBase _grabAnySystemBase;
		private HandGestureSystemGroup _gestureSystemGroup;
		
		protected override void OnCreate()
		{
			_inputAnySystemBase = World.GetOrCreateSystemManaged<HandGrabAnyInputSystemBase>();
			_grabAnySystemBase = World.GetOrCreateSystemManaged<HandGrabAnyFollowSystemBase>();
			_gestureSystemGroup = World.GetOrCreateSystemManaged<HandGestureSystemGroup>();
			RequireForUpdate<HandSettingISingleton>();
		}

		protected override void OnUpdate()
		{
			var settings = SystemAPI.GetSingleton<HandSettingISingleton>();
			if (!settings.UseGrabAny)
			{
				_inputAnySystemBase.Enabled = false;
				_grabAnySystemBase.Enabled = false;
			}
			else if (!settings.UseGesture)
			{
				_gestureSystemGroup.Enabled = false;
			}
			
			Enabled = false;
		}
	}
}