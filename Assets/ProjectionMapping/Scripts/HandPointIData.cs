using Mediapipe.Unity;
using Unity.Entities;
using Unity.Mathematics;

namespace ProjectionMapping {
	public struct HandPointIData : IComponentData {
		public float3 ScreenPosition;
		public EHand EHand;
		public byte ID;
		public bool IsTracked;
	}

	public struct HandSettingISingleton : IComponentData {
		public float3 ClampSize;
		public float2 NavigationScale;
		public float Pinky2ThumbThreshold;
		public float Wrist2IndexThreshold;
		public float Wrist2MiddleThreshold;
		public float Wrist2RingThreshold;
		public float Wrist2PinkyThreshold;
		public float Thumb2IndexThreshold;
		public EHandPose NavigateGesture;
		public EHandPose PickGesture;
		public ENavigationTransformType XTransformType;
		public ENavigationTransformType YTransformType;
		public ENavigationAxisType NavigationAxisType;
		public bool UseGesture;
		public bool UseGrabAny;
		public bool AlwaysForward;
	}

	public struct HandPoseISingleton : IComponentData {
		public float3 LeftLocalPosition;
		public float3 RightLocalPosition;
		public EHandPose LeftCurrentHandPose;
		public EHandPose LeftPreviousHandPose;
		public EHandPose RightCurrentHandPose;
		public EHandPose RightPreviousHandPose;
	}

	public struct HandScreenISingleton : IComponentData {
		public float3 LeftCurrentScreenPosition;
		public float3 LeftPreviousScreenPosition;
		public float3 LeftDeltaPosition;
		public float3 RightCurrentScreenPosition;
		public float3 RightPreviousScreenPosition;
		public float3 RightDeltaPosition;
	}

	public struct PointSpawnIData : IComponentData {
		public float CurrentTime;
	}

	public struct GrabbableData {
		public Entity Target;
		public float3 PointOnBody;
		public float3 Origin;
		public bool Valid;
	}

	public struct HandGrabbableIData : IComponentData {
		public bool IsTrigger;
		public bool IsGrabbed;
	}

	// Make sure it only register one trigger event in the time frame
	[InternalBufferCapacity(1)]
	public struct GrabbableOverlapIBuffer : IBufferElementData {
		public byte Value;
	}
}