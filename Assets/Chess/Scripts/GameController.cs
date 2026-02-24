using System;
using EugeneC.Singleton;
using ProjectionMapping;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ChessGame
{
	public enum ESpawnType : byte
	{
		Player,
		Guards,
		None = byte.MaxValue
	}
	
	[DisallowMultipleComponent]
    public class GameController : GenericSingleton<GameController>
    {
        [SerializeField] private Transform cameraTransform;
        [field: SerializeField] public GameObject playerPrefab { get; private set;}
        [field: SerializeField] public GameObject guardPrefab { get; private set;}

        private float3 _delta;
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
	        _delta = math.length(arg1) > math.length(arg2) ? arg1 : arg2;
	        _delta = math.abs(_delta.x) > 1f ? _delta : float3.zero;

	        // Can only choose one between turn and move
	        if (math.abs(_delta.x) > math.abs(_delta.y))
	        {
		        var rotation = cameraTransform.eulerAngles;
		        _delta.x = math.clamp(_delta.x, -12f, 12f);
		        rotation.y += _delta.x;
		        cameraTransform.eulerAngles = math.lerp(cameraTransform.eulerAngles, rotation, 10f * Time.deltaTime);
	        }
	        else if(math.abs(_delta.x) < math.abs(_delta.y))
	        {
		        _delta = math.abs(_delta.y) > 2f ? _delta : float3.zero;
		        var startPos = cameraTransform.transform.position; 
		        _delta.y = math.clamp(_delta.y, -2f, 2f);
		        cameraTransform.transform.position = math.lerp(startPos, 
			        startPos + _delta.y * cameraTransform.transform.forward, 5f * Time.deltaTime);
	        }
	        else
	        {
		        return;
	        }
        }
    }
}
