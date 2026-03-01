using System;
using System.Threading;
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

		private Camera _camera;
		public Camera Cam => _camera;

		public event Action OnCameraReady;

		private readonly CancellationTokenSource _tokenSource = new();
		public void CameraCancellation() => _tokenSource.Cancel();

		private async void OnEnable()
		{
			try
			{
				_camera = GetComponent<Camera>();
				await Awaitable.WaitForSecondsAsync(.1f, _tokenSource.Token);
				await RunFadeScreen(UtilityCollection.EFadeType.FadeOut, initialFadeOutTime);
			}
			catch (Exception e) { Debug.LogException(e); }
		}

		private void OnDisable()
		{
			CameraCancellation();
		}

		public async Task RunFadeScreen(UtilityCollection.EFadeType fadeType, float duration)
		{
			await Awaitable.EndOfFrameAsync(_tokenSource.Token);
			await _tokenSource.Token.FadeScreenAsync(blackScreenImg, fadeType, duration, Time.deltaTime);
			OnCameraReady?.Invoke();
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