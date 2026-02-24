using System;
using EugeneC.Singleton;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectionMapping
{
	[DisallowMultipleComponent]
    public class GameController : GenericSingleton<GameController>
    {
        [SerializeField] private Transform cameraTransform;
        
        private World _world;
	    
        private async void Start()
        {
	        try
	        {
		        await Awaitable.EndOfFrameAsync();
		        _world = World.DefaultGameObjectInjectionWorld;
		        var system = _world.GetExistingSystemManaged<HandDataEventSystemBase>();
		        system.OnScreenDeltaChanged += MoveCamera;
	        }
	        catch (Exception e){ Debug.Log(e);}
        }
        
        private void MoveCamera(float3 arg1, float3 arg2)
        {
	        var delta = math.length(arg1) > math.length(arg2) ? arg1 : arg2;
	        delta = math.abs(delta.x) > 1f ? delta : float3.zero;

	        // Can only choose one between turn and move
	        if (math.abs(delta.x) > math.abs(delta.y))
	        {
		        var rotation = cameraTransform.eulerAngles;
		        delta.x = math.clamp(delta.x, -12f, 12f);
		        rotation.y += delta.x;
		        cameraTransform.eulerAngles = math.lerp(cameraTransform.eulerAngles, rotation, 10f * Time.deltaTime);
	        }
	        else if(math.abs(delta.x) < math.abs(delta.y))
	        {
		        delta = math.abs(delta.y) > 2f ? delta : float3.zero;
		        var startPos = cameraTransform.transform.position; 
		        delta.y = math.clamp(delta.y, -2f, 2f);
		        cameraTransform.transform.position = math.lerp(startPos, 
			        startPos + delta.y * cameraTransform.transform.forward, 5f * Time.deltaTime);
	        }
	        else
	        {
		        return;
	        }
        }
    }
}
