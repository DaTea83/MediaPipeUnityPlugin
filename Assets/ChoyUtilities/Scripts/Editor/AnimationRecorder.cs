#if UNITY_EDITOR
using EugeneC.Utilities;
using UnityEditor.Animations;
using UnityEngine;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace EugeneC.Editor.ChoyUtilities.Scripts.Editor
{
	[AddComponentMenu("Eugene/Animation Recorder")]
	[RequireComponent(typeof(Animator))]
	public class AnimationRecorder : MonoBehaviour
	{
		[SerializeField] AnimationClip animationClip;
		[SerializeField] float duration = 1.0f;

		[Header("Fire Event")] 
		[SerializeField] private string className;

		[SerializeField] private string methodName;

		private float _timer;
		private bool _canRecord;
		private GameObjectRecorder _recorder;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		private void Start()
		{
			_recorder = new GameObjectRecorder(gameObject);
			_recorder.BindComponentsOfType<Transform>(gameObject, true);

			_timer = duration;
		}

		private void LateUpdate()
		{
			if (_canRecord)
				RecordAnimation();
		}

		private void OnGUI()
		{
			if (!GUI.Button(new Rect(0, 0, 200, 40), "Start Record")) return;
			if (_canRecord) return;
			UtilityMethods.CallGenericInstanceMethod(className, methodName);
			_canRecord = true;
		}

		private void RecordAnimation()
		{
			_timer -= Time.unscaledDeltaTime;
			if (_timer < 0)
			{
				if (_recorder.isRecording)
				{
					_recorder.SaveToClip(animationClip);
					print("End Recording");

					_canRecord = false;
					_timer = duration;
				}
			}
			else
			{
				_recorder.TakeSnapshot(Time.unscaledDeltaTime);
			}
		}
	}
}
#endif