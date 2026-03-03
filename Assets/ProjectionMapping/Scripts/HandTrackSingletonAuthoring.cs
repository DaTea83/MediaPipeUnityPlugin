using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectionMapping
{
	public enum ENavigationAxisType : byte
	{
		XY,
		XZ,
		YZ
	}

	public enum ENavigationTransformType : byte
	{
		Up,
		Right,
		Forward,
		RotateX,
		RotateY,
		RotateZ,
	}
	
	[DisallowMultipleComponent]
    public sealed class HandTrackSingletonAuthoring : MonoBehaviour
    {
	    [Header("Threshold")] 
	    [SerializeField, Range(0.2f, 10f)] private float pinky2Thumb = 1.5f;
	    [SerializeField, Range(0.2f, 10f)] private float wrist2Index = 1.8f, 
		    wrist2Middle = 1.8f, 
		    wrist2Ring = 1.8f, 
		    wrist2Pinky = 1.8f, 
		    thumb2Index = 1.1f;
	    [Space]
	    [SerializeField] private bool useGrabAny;
	    [SerializeField] private bool useGesture;
	    [Space]
	    [SerializeField] private EHandPose pickGesture = EHandPose.OkSign;
	    [Space]
	    [SerializeField] private EHandPose navigateHandPose = EHandPose.GunSign;
	    [SerializeField] private ENavigationAxisType navigationAxisType = ENavigationAxisType.XY;
	    [SerializeField] private ENavigationTransformType xTransformType = ENavigationTransformType.Up;
	    [SerializeField] private ENavigationTransformType yTransformType = ENavigationTransformType.Up;
	    [SerializeField] private float2 navigationScale = new float2(1f, 1f);
	    [SerializeField] private float3 clampArea;
	    
	    private void OnValidate()
	    {
		    if(useGrabAny) useGesture = false;
		    else if(useGesture) useGrabAny = false;
	    }

	    public class Baker : Baker<HandTrackSingletonAuthoring>
	    {
		    public override void Bake(HandTrackSingletonAuthoring authoring)
		    {
			    var e = GetEntity(TransformUsageFlags.None);
			    AddComponent<HandTrackingISingleton>(e);
			    AddComponent<HandPoseISingleton>(e);
			    AddComponent<HandScreenISingleton>(e);
			    AddComponent(e, new HandSettingISingleton
			    {
				    UseGrabAny = authoring.useGrabAny,
				    UseGesture = authoring.useGesture,
				    PickGesture = authoring.pickGesture,
				    NavigateGesture = authoring.navigateHandPose,
				    NavigationAxisType = authoring.navigationAxisType,
				    NavigationScale = authoring.navigationScale,
				    Pinky2ThumbThreshold = authoring.pinky2Thumb,
				    Wrist2IndexThreshold = authoring.wrist2Index,
				    Wrist2MiddleThreshold = authoring.wrist2Middle,
				    Wrist2RingThreshold = authoring.wrist2Ring,
				    Wrist2PinkyThreshold = authoring.wrist2Pinky,
				    Thumb2IndexThreshold = authoring.thumb2Index,
				    XTransformType = authoring.xTransformType,
				    YTransformType = authoring.yTransformType,
				    ClampSize = authoring.clampArea
			    });
		    }
	    }
    }
}
