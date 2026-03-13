using System;
using EugeneC.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace EugeneC.Utilities
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public sealed class CameraController : GenericSingleton<CameraController>
	{
		[SerializeField] private Image blackScreenImg;
		[SerializeField] private float initialFadeOutTime = 5f;

		public Camera Cam { get; private set; }

		public bool IsCameraReady { get; private set; }

		private async void Start()
		{
			try
			{
				Cam = GetComponent<Camera>();
				await Awaitable.WaitForSecondsAsync(.1f, Token);
				await RunFadeScreen(UtilityCollection.EFadeType.FadeOut, initialFadeOutTime);
			}
			catch (Exception e) { Debug.LogException(e); }
		}

		public async Awaitable RunFadeScreen(UtilityCollection.EFadeType fadeType, float duration)
		{
            IsCameraReady = false;
			await Awaitable.EndOfFrameAsync(Token);
			await Token.FadeScreenAsync(blackScreenImg, fadeType, duration, Time.deltaTime);
			IsCameraReady = true;
		}
	}
}