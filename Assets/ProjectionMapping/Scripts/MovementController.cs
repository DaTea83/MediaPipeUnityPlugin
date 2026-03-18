using System;
using EugeneC.Singleton;
using EugeneC.Utilities;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectionMapping {
	
	[DisallowMultipleComponent]
	public class MovementController : GenericSingleton<MovementController> {
		
		[SerializeField] private Transform cameraTransform;
		private HandSettingISingleton _handSetting;
		private EntityQuery _handSettingQuery;
		private bool _hasHandSettings;

		private async void Start() {
			try {
				await Token.AwaitableUntil(() => CameraController.Instance is not null && CameraController.Instance.IsCameraReady);

				var system = World.GetExistingSystemManaged<HandDataEventSystemBase>();
				system.OnScreenDeltaChanged += MoveCamera;

				_handSetting = await GetSingletonEntity<HandSettingISingleton>();
			}
			catch (Exception e) {
				Debug.LogError($"MovementController: {e}");
			}
		}

		private void MoveCamera(float3 arg1, float3 arg2) {
			if (!_hasHandSettings) return;
			var delta = math.length(arg1) > math.length(arg2) ? arg1 : arg2;

			var scale = _handSetting.NavigationAxisType switch {
				ENavigationAxisType.XY => new float2(delta.x, delta.y),
				ENavigationAxisType.XZ => new float2(delta.x, delta.z),
				ENavigationAxisType.YZ => new float2(delta.y, delta.z),
				_ => throw new ArgumentOutOfRangeException()
			};

			var rotation = cameraTransform.eulerAngles;
			var startPos = cameraTransform.transform.position;

			if (_handSetting.AlwaysForward) {
				var target = startPos + _handSetting.NavigationScale.y * cameraTransform.forward;
				var clamp = _handSetting.ClampSize;
				target = clamp is { x: <= 0, y: <= 0, z: <= 0 }
					? target
					: math.clamp(target, clamp, clamp);

				cameraTransform.position = math.lerp(
					startPos, target, Time.deltaTime.SmoothFactor());
			}
			
			var absX = math.abs(scale.x);
			var absY = math.abs(scale.y);

			var useX = (absX > absY) || _handSetting.AlwaysForward;
			var input = useX? scale.x : scale.y;
			var inputScale = useX ? _handSetting.NavigationScale.x : _handSetting.NavigationScale.y;
			var transformType = useX ? _handSetting.XTransformType : _handSetting.YTransformType;
			var rotateScale = useX ? _handSetting.NavigationScale.x : _handSetting.NavigationScale.y;
			
			if (math.abs(input) <= 0.2f) return;
			if ((int)absX == (int)absY) return;

			switch (transformType) {
				case ENavigationTransformType.Up:
					ApplyMove(cameraTransform.up);
					break;
				case ENavigationTransformType.Right:
					ApplyMove(cameraTransform.right);
					break;
				case ENavigationTransformType.Forward:
					ApplyMove(cameraTransform.forward);
					break;
				case ENavigationTransformType.RotateX:
					ApplyRotate(0);
					break;
				case ENavigationTransformType.RotateY:
					ApplyRotate(1);
					break;
				case ENavigationTransformType.RotateZ:
					ApplyRotate(2);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return;

			void ApplyRotate(int axisIndex) {
				var clamped = math.clamp(input, -12f, 12f);
				rotation[axisIndex] += clamped * rotateScale;
				cameraTransform.eulerAngles = math.lerp(
					cameraTransform.eulerAngles,
					rotation,
					Time.deltaTime.SmoothFactor());
			}

			void ApplyMove(float3 direction) {
				var move = input * inputScale;
				var target = (float3)startPos + move * direction;
				var clamp = _handSetting.ClampSize;
				target = clamp is { x: <= 0, y: <= 0, z: <= 0 }
					? target
					: math.clamp(target, clamp, clamp);

				cameraTransform.position = math.lerp(
					startPos, target, Time.deltaTime.SmoothFactor());
			}
		}
	}
}