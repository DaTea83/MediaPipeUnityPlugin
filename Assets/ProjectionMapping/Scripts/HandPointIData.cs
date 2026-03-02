using Mediapipe.Unity;
using Unity.Entities;
using Unity.Mathematics;

namespace ProjectionMapping
{
    public struct HandPointIData : IComponentData
    {
	    public byte ID;
	    public bool IsTracked;
	    public EHand EHand;
	    public float3 ScreenPosition;
    }

    public struct HandSettingISingleton : IComponentData
    {
	    public bool UseGrabAny;
	    public bool UseGesture;
	    public EHandPose PickGesture;
	    public EHandPose NavigateGesture;
	    public ENavigationAxisType NavigationAxisType;
	    public ENavigationTransformType XTransformType;
	    public ENavigationTransformType YTransformType;
	    public float Pinky2ThumbThreshold;
	    public float Wrist2IndexThreshold;
	    public float Wrist2MiddleThreshold;
	    public float Wrist2RingThreshold;
	    public float Wrist2PinkyThreshold;
	    public float Thumb2IndexThreshold;
	    public float2 NavigationScale;
	    public float3 ClampSize;
    }

    public struct HandPoseISingleton : IComponentData
    {
	    public EHandPose LeftCurrentHandPose;
	    public EHandPose LeftPreviousHandPose;
	    public EHandPose RightCurrentHandPose;
	    public EHandPose RightPreviousHandPose;
	    public float3 LeftLocalPosition;
	    public float3 RightLocalPosition;
    }

    public struct HandScreenISingleton : IComponentData
    {
	    public float3 LeftCurrentScreenPosition;
	    public float3 LeftPreviousScreenPosition;
	    public float3 LeftDeltaPosition;
	    public float3 RightCurrentScreenPosition;
	    public float3 RightPreviousScreenPosition;
	    public float3 RightDeltaPosition;
    }
    
    public struct PointSpawnIData : IComponentData
    {
	    public float CurrentTime;
    }

    public struct GrabbableData
    {
	    public bool Valid;
	    public float3 PointOnBody;
	    public float3 Origin;
	    public Entity Target;
    }

    public struct HandGrabbableIData : IComponentData
    {
	    public bool IsGrabbed;
    }

    // Make sure it only register one trigger event in the time frame
    [InternalBufferCapacity( 1 )]
    public struct GrabbableOverlapIBuffer : IBufferElementData
    {
	    public byte Value;
    }
}
