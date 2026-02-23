using System;
using EugeneC.Singleton;
using ProjectionMapping;
using Unity.Entities;
using UnityEngine;

namespace ChessGame
{
	[DisallowMultipleComponent]
    public class GameController : GenericSingleton<GameController>
    {
        [SerializeField] private Transform cameraTransform;
        [field: SerializeField] public GameObject playerPrefab { get; private set;}
        
        private World _world;
	    
        private async void Start()
        {
	        try
	        {
		        await Awaitable.EndOfFrameAsync();
		        _world = World.DefaultGameObjectInjectionWorld;
		        var system = _world.GetExistingSystemManaged<HandDataEventSystemBase>();
		        
	        }
	        catch (Exception e){ Debug.Log(e);}
        }
    }
}
