using EugeneC.Singleton;
using UnityEngine;

namespace ChessGame
{
	[DisallowMultipleComponent]
    public class GameController : GenericSingleton<GameController>
    {
        [SerializeField] private Transform cameraTransform;
    }
}
