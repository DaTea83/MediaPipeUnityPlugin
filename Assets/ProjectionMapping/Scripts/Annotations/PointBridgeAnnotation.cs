using System;
using EugeneC.ECS;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Unity;
using Mediapipe.Unity.CoordinateSystem;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ProjectionMapping
{
	[DisallowMultipleComponent]
    public sealed class PointBridgeAnnotation : HierarchicalAnnotation
    {
	    public EntityManager EManager;
	    public EntityArchetype EntityArchetype;
	    
	    public EHand eHand;
	    public byte id;
	    public bool isTracked;
	    public Entity Entity;

	    public void Draw(NormalizedLandmark target)
	    {
		    if (!ActivateFor(target)) return;
		    var position = GetScreenRect().GetPoint(target, rotationAngle, isMirrored);
		    transform.localPosition = position;
		    
		    if(Entity == Entity.Null)
			    Entity = EManager.CreateEntity(EntityArchetype);
		    
		    EManager.SetComponentData(Entity, new LocalTransform
		    {
			    Position = transform.localToWorldMatrix.GetPosition()
		    });
		    EManager.SetComponentData(Entity, new HandPointIData
		    {
			    ID = id,
			    EHand = eHand,
			    IsTracked = isTracked,
			    ScreenPosition = position
		    });
	    }
    }
}
