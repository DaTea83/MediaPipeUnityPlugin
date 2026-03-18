using System;
using EugeneC.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace ProjectionMapping {
	[UpdateInGroup(typeof(Eu_EffectSystemGroup), OrderLast = true)]
	public partial class HandDataEventSystemBase : SystemBase {
		public event Action<EHandPose, EHandPose> OnPoseChanged;
		public event Action<HandData, HandData> OnHandDataChanged;
		public event Action<float3, float3> OnScreenDeltaChanged;

		protected override void OnCreate() {
			RequireForUpdate<HandTrackingISingleton>();
			RequireForUpdate<HandPoseISingleton>();
			RequireForUpdate<HandScreenISingleton>();
			RequireForUpdate<HandSettingISingleton>();
		}

		protected override void OnUpdate() {
			var tracking = SystemAPI.GetSingleton<HandTrackingISingleton>();
			var pose = SystemAPI.GetSingleton<HandPoseISingleton>();
			var settings = SystemAPI.GetSingleton<HandSettingISingleton>();
			var screen = SystemAPI.GetSingleton<HandScreenISingleton>();

			screen.LeftDeltaPosition = screen.LeftCurrentScreenPosition - screen.LeftPreviousScreenPosition;
			screen.RightDeltaPosition = screen.RightCurrentScreenPosition - screen.RightPreviousScreenPosition;

			SystemAPI.SetSingleton(screen);

			OnPoseChanged?.Invoke(pose.LeftCurrentHandPose, pose.RightCurrentHandPose);
			OnHandDataChanged?.Invoke(tracking.LeftHand, tracking.RightHand);

			if (settings.NavigateGesture == EHandPose.None) return;
			if (settings.UsePhysics) return;
			if (pose.LeftCurrentHandPose == settings.NavigateGesture ||
			    pose.RightCurrentHandPose == settings.NavigateGesture)
				OnScreenDeltaChanged?.Invoke(screen.LeftDeltaPosition, screen.RightDeltaPosition);
		}
	}
}