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
	
	[DisallowMultipleComponent]
    public sealed class HandTrackSingletonAuthoring : MonoBehaviour
    {
	    [SerializeField] private bool useGrabAny;
	    [SerializeField] private bool useGesture;
	    [Space]
	    [SerializeField] private EHandPose pickGesture = EHandPose.OkSign;
	    [Space]
	    [SerializeField] private EHandPose navigateHandPose = EHandPose.GunSign;
	    [SerializeField] private ENavigationAxisType navigationAxisType = ENavigationAxisType.XY;
	    [SerializeField] private float2 navigationScale = new float2(1f, 1f);
	    
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
				    NavigationScale = authoring.navigationScale
			    });
		    }
	    }
    }
}
