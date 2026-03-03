using System;
using System.Threading.Tasks;
using EugeneC.Singleton;
using UnityEditor;
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

		private async void OnEnable()
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
			await Awaitable.EndOfFrameAsync(Token);
			await Token.FadeScreenAsync(blackScreenImg, fadeType, duration, Time.deltaTime);
			IsCameraReady = true;
		}
	}

#if UNITY_EDITOR

	[CustomEditor(typeof(CameraController))]
	public class CameraTrackerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var instance = (CameraController)target;
			EditorGUILayout.HelpBox(
				"Currently in ECS you can't just attach the camera to a subscene entity and called it a day",
				MessageType.Info);
			EditorGUILayout.HelpBox("That's where this singleton comes into play", MessageType.Info);
			EditorGUILayout.HelpBox("Attach this to the camera object", MessageType.Info);
			EditorGUILayout.HelpBox("Don't put the camera in the subscene, put it in normal hierarchy",
				MessageType.Warning);
			EditorGUILayout.HelpBox(
				"Keep Singleton true or not doesn't matter, if true it will just override the other scene's camera",
				MessageType.Info);

			base.OnInspectorGUI();
		}
	}

#endif
}