using Mediapipe.Unity;
using Unity.Mathematics;

namespace ProjectionMapping
{
	//DO NOT TOUCH
	public enum ETrackingTarget : byte
	{
		LWrist2Thumb,
		LWrist2Index,
		LWrist2Middle,
		LWrist2Ring,
		LWrist2Pinky,
		LThumb2Index,
		LIndex2Middle,
		LMiddle2Ring,
		LRing2Pinky,
		LPinky2Thumb,
		RWrist2Thumb,
		RWrist2Index,
		RWrist2Middle,
		RWrist2Ring,
		RWrist2Pinky,
		RThumb2Index,
		RIndex2Middle,
		RMiddle2Ring,
		RRing2Pinky,
		RPinky2Thumb,
	}
	
	public enum EHandPose : ushort
	{
		None = 0,
		ClenchedFist = 1 << 0,
		ThumbsUp = 1 << 1,
		MiddleFinger = 1 << 2,
		PhoneSign = 1 << 3,
		PeaceSign = 1 << 4,
		GunSign = 1 << 5,
		RockNRoll = 1 << 6,
		OkSign = 1 << 7,
		HighFive = 1 << 8,
	}
	
    public static class HandCollection
    {
	    public static (float, float3) GetValue(this HandTrackingISingleton singleton, ETrackingTarget target)
	    {
		    return target switch
		    {
			    ETrackingTarget.LWrist2Thumb => (singleton.LeftHand.Wrist2Thumb.CurrentInput,
				    singleton.LeftHand.Wrist2Thumb.LocalPosition),
			    ETrackingTarget.LWrist2Index => (singleton.LeftHand.Wrist2Index.CurrentInput,
				    singleton.LeftHand.Wrist2Index.LocalPosition),
			    ETrackingTarget.LWrist2Middle => (singleton.LeftHand.Wrist2Middle.CurrentInput,
				    singleton.LeftHand.Wrist2Middle.LocalPosition),
			    ETrackingTarget.LWrist2Ring => (singleton.LeftHand.Wrist2Ring.CurrentInput,
				    singleton.LeftHand.Wrist2Ring.LocalPosition),
			    ETrackingTarget.LWrist2Pinky => (singleton.LeftHand.Wrist2Pinky.CurrentInput,
				    singleton.LeftHand.Wrist2Pinky.LocalPosition),
			    ETrackingTarget.LThumb2Index => (singleton.LeftHand.Thumb2Index.CurrentInput,
				    singleton.LeftHand.Thumb2Index.LocalPosition),
			    ETrackingTarget.LIndex2Middle => (singleton.LeftHand.Index2Middle.CurrentInput,
				    singleton.LeftHand.Index2Middle.LocalPosition),
			    ETrackingTarget.LMiddle2Ring => (singleton.LeftHand.Middle2Ring.CurrentInput,
				    singleton.LeftHand.Middle2Ring.LocalPosition),
			    ETrackingTarget.LRing2Pinky => (singleton.LeftHand.Ring2Pinky.CurrentInput,
				    singleton.LeftHand.Ring2Pinky.LocalPosition),
			    ETrackingTarget.LPinky2Thumb => (singleton.LeftHand.Pinky2Thumb.CurrentInput,
				    singleton.LeftHand.Pinky2Thumb.LocalPosition),
			    ETrackingTarget.RWrist2Thumb => (singleton.RightHand.Wrist2Thumb.CurrentInput,
				    singleton.RightHand.Wrist2Thumb.LocalPosition),
			    ETrackingTarget.RWrist2Index => (singleton.RightHand.Wrist2Index.CurrentInput,
				    singleton.RightHand.Wrist2Index.LocalPosition),
			    ETrackingTarget.RWrist2Middle => (singleton.RightHand.Wrist2Middle.CurrentInput,
				    singleton.RightHand.Wrist2Middle.LocalPosition),
			    ETrackingTarget.RWrist2Ring => (singleton.RightHand.Wrist2Ring.CurrentInput,
				    singleton.RightHand.Wrist2Ring.LocalPosition),
			    ETrackingTarget.RWrist2Pinky => (singleton.RightHand.Wrist2Pinky.CurrentInput,
				    singleton.RightHand.Wrist2Pinky.LocalPosition),
			    ETrackingTarget.RThumb2Index => (singleton.RightHand.Thumb2Index.CurrentInput,
				    singleton.RightHand.Thumb2Index.LocalPosition),
			    ETrackingTarget.RIndex2Middle => (singleton.RightHand.Index2Middle.CurrentInput,
				    singleton.RightHand.Index2Middle.LocalPosition),
			    ETrackingTarget.RMiddle2Ring => (singleton.RightHand.Middle2Ring.CurrentInput,
				    singleton.RightHand.Middle2Ring.LocalPosition),
			    ETrackingTarget.RRing2Pinky => (singleton.RightHand.Ring2Pinky.CurrentInput,
				    singleton.RightHand.Ring2Pinky.LocalPosition),
			    ETrackingTarget.RPinky2Thumb => (singleton.RightHand.Pinky2Thumb.CurrentInput,
				    singleton.RightHand.Pinky2Thumb.LocalPosition),
			    _ => (-1f, float3.zero)
		    };
	    }

	    public static float GetPrevious(this HandTrackingISingleton singleton, ETrackingTarget target)
	    {
		    return target switch
		    {
			    ETrackingTarget.LWrist2Thumb => singleton.LeftHand.Wrist2Thumb.PreviousInput,
			    ETrackingTarget.LWrist2Index => singleton.LeftHand.Wrist2Index.PreviousInput,
			    ETrackingTarget.LWrist2Middle => singleton.LeftHand.Wrist2Middle.PreviousInput,
			    ETrackingTarget.LWrist2Ring => singleton.LeftHand.Wrist2Ring.PreviousInput,
			    ETrackingTarget.LWrist2Pinky => singleton.LeftHand.Wrist2Pinky.PreviousInput,
			    ETrackingTarget.LThumb2Index => singleton.LeftHand.Thumb2Index.PreviousInput,
			    ETrackingTarget.LIndex2Middle => singleton.LeftHand.Index2Middle.PreviousInput,
			    ETrackingTarget.LMiddle2Ring => singleton.LeftHand.Middle2Ring.PreviousInput,
			    ETrackingTarget.LRing2Pinky => singleton.LeftHand.Ring2Pinky.PreviousInput,
			    ETrackingTarget.LPinky2Thumb => singleton.LeftHand.Pinky2Thumb.PreviousInput,
			    ETrackingTarget.RWrist2Thumb => singleton.RightHand.Wrist2Thumb.PreviousInput,
			    ETrackingTarget.RWrist2Index => singleton.RightHand.Wrist2Index.PreviousInput,
			    ETrackingTarget.RWrist2Middle => singleton.RightHand.Wrist2Middle.PreviousInput,
			    ETrackingTarget.RWrist2Ring => singleton.RightHand.Wrist2Ring.PreviousInput,
			    ETrackingTarget.RWrist2Pinky => singleton.RightHand.Wrist2Pinky.PreviousInput,
			    ETrackingTarget.RThumb2Index => singleton.RightHand.Thumb2Index.PreviousInput,
			    ETrackingTarget.RIndex2Middle => singleton.RightHand.Index2Middle.PreviousInput,
			    ETrackingTarget.RMiddle2Ring => singleton.RightHand.Middle2Ring.PreviousInput,
			    ETrackingTarget.RRing2Pinky => singleton.RightHand.Ring2Pinky.PreviousInput,
			    ETrackingTarget.RPinky2Thumb => singleton.RightHand.Pinky2Thumb.PreviousInput,
			    _ => -1f
		    };
	    }
	    
	    public static float3 GetLocalPosition(HandData data, HandSettingISingleton setting)
	    {
		    return setting.PickGesture switch
		    {
			    EHandPose.None => float3.zero,
			    EHandPose.MiddleFinger => data.Wrist2Middle.LocalPosition,
			    EHandPose.PeaceSign => data.Index2Middle.LocalPosition,
			    EHandPose.RockNRoll or EHandPose.OkSign or EHandPose.ThumbsUp or EHandPose.GunSign => data.Thumb2Index.LocalPosition,
			    EHandPose.HighFive or EHandPose.PhoneSign or EHandPose.ClenchedFist => data.Pinky2Thumb.LocalPosition,
			    _ => float3.zero
		    };
	    }
	    
	    public static float3 GetScreenPosition(HandData data, HandSettingISingleton setting)
	    {
		    return setting.PickGesture switch
		    {
			    EHandPose.None => float3.zero,
			    EHandPose.MiddleFinger => data.Wrist2Middle.ScreenPosition,
			    EHandPose.PeaceSign => data.Index2Middle.ScreenPosition,
			    EHandPose.RockNRoll or EHandPose.OkSign or EHandPose.ThumbsUp or EHandPose.GunSign => data.Thumb2Index.ScreenPosition,
			    EHandPose.HighFive or EHandPose.PhoneSign or EHandPose.ClenchedFist => data.Pinky2Thumb.ScreenPosition,
			    _ => float3.zero
		    };
	    }

	    public static (bool, HandData) GetHand(this HandTrackingISingleton singleton, EHand hand)
	    {
		    return hand switch
		    {
			    EHand.Left => (true, singleton.LeftHand),
			    EHand.Right => (true, singleton.RightHand),
			    _ => (false, default)
		    };
	    }

	    public static EHandPose GetPose(this HandData data, HandSettingISingleton setting)
	    {
		    if (data.Pinky2Thumb.CurrentInput <= -1 || data.Wrist2Index.CurrentInput <= -1) return EHandPose.None;
		    
		    var thumb = IsFingerCurled(data.Pinky2Thumb, setting.Pinky2ThumbThreshold);
		    var index = IsFingerCurled(data.Wrist2Index, setting.Wrist2IndexThreshold);
		    var middle = IsFingerCurled(data.Wrist2Middle, setting.Wrist2MiddleThreshold);
		    var ring = IsFingerCurled(data.Wrist2Ring, setting.Wrist2RingThreshold);
		    var pinky = IsFingerCurled(data.Wrist2Pinky, setting.Wrist2PinkyThreshold);
		    var pinch = IsFingerCurled(data.Thumb2Index, setting.Thumb2IndexThreshold);

		    return thumb switch
		    {
			    false when !index && !middle && !ring && !pinky && !pinch => EHandPose.HighFive,
			    false when !index && !middle && !ring && !pinky && pinch => EHandPose.OkSign,
			    true when index && middle && ring && pinky && pinch => EHandPose.ClenchedFist,
			    false when index && middle && ring && pinky && !pinch => EHandPose.ThumbsUp,
			    false when !index && middle && ring && pinky && !pinch => EHandPose.GunSign,
			    false when index && middle && ring && !pinky && !pinch => EHandPose.PhoneSign,
			    false when !index && middle && ring && !pinky && !pinch => EHandPose.RockNRoll,
			    true when index && !middle && ring && pinky && pinch => EHandPose.MiddleFinger,
			    true when !index && !middle && ring && pinky && !pinch => EHandPose.PeaceSign,
			    _ => EHandPose.None
		    };

		    bool IsFingerCurled(PointData point, float threshold = 1.8f) => point.CurrentInput < threshold;
	    }
    }
}
