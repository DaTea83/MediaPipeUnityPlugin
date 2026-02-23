using Mediapipe.Unity;
using Unity.Entities;
using Unity.Mathematics;

namespace ProjectionMapping
{
    public struct HandPointIData : IComponentData
    {
	    public byte ID;
	    public EHand EHand;
	    public bool IsTracked;
	    public float3 ScreenPosition;
    }

    public struct HandSettingISingleton : IComponentData
    {
	    public bool UseGrabAny;
	    public bool UseGesture;
	    public EHandPose PickGesture;
	    public EHandPose NavigateGesture;
	    public float3 CamDirection;
    }

    public struct HandPoseISingleton : IComponentData
    {
	    public EHandPose LeftCurrentHandPose;
	    public EHandPose LeftPreviousHandPose;
	    public float3 LeftOrigin;
	    public EHandPose RightCurrentHandPose;
	    public EHandPose RightPreviousHandPose;
	    public float3 RightOrigin;
    }
    
    public struct PointSpawnIData : IComponentData
    {
	    public float CurrentTime;
    }

    public struct GrabbableData
    {
	    public bool Valid;
	    public Entity Target;
	    public float3 PointOnBody;
	    public float3 Origin;
    }

    public struct HandGrabbableIData : IComponentData
    {
	    public bool IsGrabbed;
    }
}
