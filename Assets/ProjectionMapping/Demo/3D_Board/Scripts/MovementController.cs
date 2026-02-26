using System;
using EugeneC.Singleton;
using EugeneC.Utilities;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectionMapping
{
	[DisallowMultipleComponent]
    public class MovementController : GenericSingleton<MovementController>
    {
        [SerializeField] private Transform cameraTransform;
        
        private World _world;
        private EntityQuery _handSettingQuery;
        private bool _hasHandSettings;
        private HandSettingISingleton _handSetting;
	    
        private async void Start()
        {
	        try
	        {
		        await Awaitable.EndOfFrameAsync();
		        _world = World.DefaultGameObjectInjectionWorld;
		        
		        var system = _world.GetExistingSystemManaged<HandDataEventSystemBase>();
		        system.OnScreenDeltaChanged += MoveCamera;

		        _handSettingQuery = _world.EntityManager.CreateEntityQuery(
			        ComponentType.ReadOnly<HandSettingISingleton>());
		        _hasHandSettings = _handSettingQuery.TryGetSingleton(out _handSetting);

	        }
	        catch (Exception e){ Debug.Log(e);}
        }
        
        private void MoveCamera(float3 arg1, float3 arg2)
        {
	        if (!_hasHandSettings) return;
	        var delta = math.length(arg1) > math.length(arg2) ? arg1 : arg2;

	        var scale = _handSetting.NavigationAxisType switch
	        {
		        ENavigationAxisType.XY => new float2(delta.x, delta.y),
		        ENavigationAxisType.XZ => new float2(delta.x, delta.z),
		        ENavigationAxisType.YZ => new float2(delta.y, delta.z),
		        _ => throw new ArgumentOutOfRangeException()
	        };

	        scale = math.abs(scale.x) > 0.75f ? scale : float2.zero;

	        // Can only choose one between turn and move
	        if (math.abs(scale.x) > math.abs(scale.y))
	        {
		        var rotation = cameraTransform.eulerAngles;
		        scale.x = math.clamp(scale.x, -12f, 12f);
		        rotation.y += scale.x * _handSetting.NavigationScale.x;
		        cameraTransform.eulerAngles = math.lerp(cameraTransform.eulerAngles, rotation, Time.deltaTime.SmoothFactor());
	        }
	        else if(math.abs(scale.x) < math.abs(scale.y))
	        {
		        scale = math.abs(scale.y) > 0.2f ? scale : float2.zero;
		        var startPos = cameraTransform.transform.position; 
		        scale.y *= _handSetting.NavigationScale.y * 0.1f;
		        cameraTransform.transform.position = math.lerp(startPos, 
			        startPos + scale.y * cameraTransform.transform.forward, Time.deltaTime.SmoothFactor());
	        }
        }
    }
}
