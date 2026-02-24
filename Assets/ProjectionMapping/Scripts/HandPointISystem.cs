using EugeneC.ECS;
using Mediapipe.Unity;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ProjectionMapping
{
	public struct PointData
	{
		public float CurrentInput;
		public float PreviousInput;
		public float3 LocalPosition;
		public float3 ScreenPosition;
	}

	public struct HandData
	{
		public PointData Wrist2Thumb;
		public PointData Wrist2Index;
		public PointData Wrist2Middle;
		public PointData Wrist2Ring;
		public PointData Wrist2Pinky;
		public PointData Thumb2Index;
		public PointData Index2Middle;
		public PointData Middle2Ring;
		public PointData Ring2Pinky;
		public PointData Pinky2Thumb;
	}

	public struct HandTrackingISingleton : IComponentData
	{
		public HandData LeftHand;
		public HandData RightHand;
	}

	[BurstCompile]
	[UpdateInGroup(typeof(Eu_EffectSystemGroup), OrderFirst = true)]
	public partial struct HandPointISystem : ISystem
	{
		private const int Wrist = 0;
		private const int Thumb = 4;
		private const int Index = 8;
		private const int Middle = 12;
		private const int Ring = 16;
		private const int Pinky = 20;

		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<HandTrackingISingleton>();
			state.RequireForUpdate<HandSettingISingleton>();
			state.RequireForUpdate<HandPoseISingleton>();
			state.RequireForUpdate<HandScreenISingleton>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var tracking = SystemAPI.GetSingleton<HandTrackingISingleton>();
			var pose = SystemAPI.GetSingleton<HandPoseISingleton>();
			var settings = SystemAPI.GetSingleton<HandSettingISingleton>();
			var screen = SystemAPI.GetSingleton<HandScreenISingleton>();

			var leftPos = new NativeArray<float3>(21, Allocator.Temp);
			var rightPos = new NativeArray<float3>(21, Allocator.Temp);

			var leftScreenPos = new NativeArray<float3>(21, Allocator.Temp);
			var rightScreenPos = new NativeArray<float3>(21, Allocator.Temp);

			var leftId = new NativeArray<byte>(21, Allocator.Temp);
			var rightId = new NativeArray<byte>(21, Allocator.Temp);

			foreach (var (point, lt, _)
			         in SystemAPI.Query<RefRO<HandPointIData>, RefRO<LocalTransform>>().WithEntityAccess())
			{
				if (point.ValueRO.EHand == EHand.None) continue;
				if (!point.ValueRO.IsTracked) continue;

				var pos = lt.ValueRO.Position;
				var id = point.ValueRO.ID;
				if (id >= 21) continue;

				switch (point.ValueRO.EHand)
				{
					case EHand.Left:
						leftPos[id] = pos;
						leftScreenPos[id] = point.ValueRO.ScreenPosition;
						leftId[id] = point.ValueRO.ID;
						break;
					case EHand.Right:
						rightPos[id] = pos;
						rightScreenPos[id] = point.ValueRO.ScreenPosition;
						rightId[id] = point.ValueRO.ID;
						break;
					default:
						continue;
				}
			}

			// Maybe there's a better way to do this, but keep it as it is for now
			// If I decided to use Enum.GetValues(),
			// cuz that is a managed type, means I had to give up burst compile.
			// TODO: Need testing to find which is better

			tracking.LeftHand.Wrist2Thumb.PreviousInput = tracking.LeftHand.Wrist2Thumb.CurrentInput;
			tracking.LeftHand.Wrist2Index.PreviousInput = tracking.LeftHand.Wrist2Index.CurrentInput;
			tracking.LeftHand.Wrist2Middle.PreviousInput = tracking.LeftHand.Wrist2Middle.CurrentInput;
			tracking.LeftHand.Wrist2Ring.PreviousInput = tracking.LeftHand.Wrist2Ring.CurrentInput;
			tracking.LeftHand.Wrist2Pinky.PreviousInput = tracking.LeftHand.Wrist2Pinky.CurrentInput;

			tracking.LeftHand.Thumb2Index.PreviousInput = tracking.LeftHand.Thumb2Index.CurrentInput;
			tracking.LeftHand.Index2Middle.PreviousInput = tracking.LeftHand.Index2Middle.CurrentInput;
			tracking.LeftHand.Middle2Ring.PreviousInput = tracking.LeftHand.Middle2Ring.CurrentInput;
			tracking.LeftHand.Ring2Pinky.PreviousInput = tracking.LeftHand.Ring2Pinky.CurrentInput;
			tracking.LeftHand.Pinky2Thumb.PreviousInput = tracking.LeftHand.Pinky2Thumb.CurrentInput;

			tracking.RightHand.Wrist2Thumb.PreviousInput = tracking.RightHand.Wrist2Thumb.CurrentInput;
			tracking.RightHand.Wrist2Index.PreviousInput = tracking.RightHand.Wrist2Index.CurrentInput;
			tracking.RightHand.Wrist2Middle.PreviousInput = tracking.RightHand.Wrist2Middle.CurrentInput;
			tracking.RightHand.Wrist2Ring.PreviousInput = tracking.RightHand.Wrist2Ring.CurrentInput;
			tracking.RightHand.Wrist2Pinky.PreviousInput = tracking.RightHand.Wrist2Pinky.CurrentInput;

			tracking.RightHand.Thumb2Index.PreviousInput = tracking.RightHand.Thumb2Index.CurrentInput;
			tracking.RightHand.Index2Middle.PreviousInput = tracking.RightHand.Index2Middle.CurrentInput;
			tracking.RightHand.Middle2Ring.PreviousInput = tracking.RightHand.Middle2Ring.CurrentInput;
			tracking.RightHand.Ring2Pinky.PreviousInput = tracking.RightHand.Ring2Pinky.CurrentInput;
			tracking.RightHand.Pinky2Thumb.PreviousInput = tracking.RightHand.Pinky2Thumb.CurrentInput;

			(tracking.LeftHand.Wrist2Thumb.CurrentInput, tracking.LeftHand.Wrist2Thumb.LocalPosition,
					tracking.LeftHand.Wrist2Thumb.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Wrist, Thumb);
			(tracking.LeftHand.Wrist2Index.CurrentInput, tracking.LeftHand.Wrist2Index.LocalPosition,
					tracking.LeftHand.Wrist2Index.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Wrist, Index);
			(tracking.LeftHand.Wrist2Middle.CurrentInput, tracking.LeftHand.Wrist2Middle.LocalPosition,
					tracking.LeftHand.Wrist2Middle.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Wrist, Middle);
			(tracking.LeftHand.Wrist2Ring.CurrentInput, tracking.LeftHand.Wrist2Ring.LocalPosition,
					tracking.LeftHand.Wrist2Ring.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Wrist, Ring);
			(tracking.LeftHand.Wrist2Pinky.CurrentInput, tracking.LeftHand.Wrist2Pinky.LocalPosition,
					tracking.LeftHand.Wrist2Pinky.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Wrist, Pinky);

			(tracking.LeftHand.Thumb2Index.CurrentInput, tracking.LeftHand.Thumb2Index.LocalPosition,
					tracking.LeftHand.Thumb2Index.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Thumb, Index);
			(tracking.LeftHand.Index2Middle.CurrentInput, tracking.LeftHand.Index2Middle.LocalPosition,
					tracking.LeftHand.Index2Middle.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Index, Middle);
			(tracking.LeftHand.Middle2Ring.CurrentInput, tracking.LeftHand.Middle2Ring.LocalPosition,
					tracking.LeftHand.Middle2Ring.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Middle, Ring);
			(tracking.LeftHand.Ring2Pinky.CurrentInput, tracking.LeftHand.Ring2Pinky.LocalPosition,
					tracking.LeftHand.Ring2Pinky.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Ring, Pinky);
			(tracking.LeftHand.Pinky2Thumb.CurrentInput, tracking.LeftHand.Pinky2Thumb.LocalPosition,
					tracking.LeftHand.Pinky2Thumb.ScreenPosition)
				= DistanceBetween(leftPos, leftId, leftScreenPos, Pinky, Thumb);

			(tracking.RightHand.Wrist2Thumb.CurrentInput, tracking.RightHand.Wrist2Thumb.LocalPosition,
					tracking.RightHand.Wrist2Thumb.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Wrist, Thumb);
			(tracking.RightHand.Wrist2Index.CurrentInput, tracking.RightHand.Wrist2Index.LocalPosition,
					tracking.RightHand.Wrist2Index.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Wrist, Index);
			(tracking.RightHand.Wrist2Middle.CurrentInput, tracking.RightHand.Wrist2Middle.LocalPosition,
					tracking.RightHand.Wrist2Middle.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Wrist, Middle);
			(tracking.RightHand.Wrist2Ring.CurrentInput, tracking.RightHand.Wrist2Ring.LocalPosition,
					tracking.RightHand.Wrist2Ring.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Wrist, Ring);
			(tracking.RightHand.Wrist2Pinky.CurrentInput, tracking.RightHand.Wrist2Pinky.LocalPosition,
					tracking.RightHand.Wrist2Pinky.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Wrist, Pinky);

			(tracking.RightHand.Thumb2Index.CurrentInput, tracking.RightHand.Thumb2Index.LocalPosition,
					tracking.RightHand.Thumb2Index.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Thumb, Index);
			(tracking.RightHand.Index2Middle.CurrentInput, tracking.RightHand.Index2Middle.LocalPosition,
					tracking.RightHand.Index2Middle.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Index, Middle);
			(tracking.RightHand.Middle2Ring.CurrentInput, tracking.RightHand.Middle2Ring.LocalPosition,
					tracking.RightHand.Middle2Ring.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Middle, Ring);
			(tracking.RightHand.Ring2Pinky.CurrentInput, tracking.RightHand.Ring2Pinky.LocalPosition,
					tracking.RightHand.Ring2Pinky.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Ring, Pinky);
			(tracking.RightHand.Pinky2Thumb.CurrentInput, tracking.RightHand.Pinky2Thumb.LocalPosition,
					tracking.RightHand.Pinky2Thumb.ScreenPosition)
				= DistanceBetween(rightPos, rightId, rightScreenPos, Pinky, Thumb);

			SystemAPI.SetSingleton(tracking);

			leftPos.Dispose();
			rightPos.Dispose();
			leftScreenPos.Dispose();
			rightScreenPos.Dispose();
			leftId.Dispose();
			rightId.Dispose();

			pose.LeftPreviousHandPose = pose.LeftCurrentHandPose;
			pose.RightPreviousHandPose = pose.RightCurrentHandPose;

			var (lValid, left) = tracking.GetHand(EHand.Left);
			pose.LeftCurrentHandPose = lValid ? left.GetPose() : EHandPose.None;
			var (rValid, right) = tracking.GetHand(EHand.Right);
			pose.RightCurrentHandPose = rValid ? right.GetPose() : EHandPose.None;

			pose.LeftLocalPosition = HandCollection.GetLocalPosition(tracking.LeftHand, settings);
			pose.RightLocalPosition = HandCollection.GetLocalPosition(tracking.RightHand, settings);
			SystemAPI.SetSingleton(pose);

			screen.LeftPreviousScreenPosition = screen.LeftCurrentScreenPosition;
			screen.RightPreviousScreenPosition = screen.RightCurrentScreenPosition;
			
			screen.LeftCurrentScreenPosition = HandCollection.GetScreenPosition(tracking.LeftHand, settings);
			screen.RightCurrentScreenPosition = HandCollection.GetScreenPosition(tracking.RightHand, settings);
			
			SystemAPI.SetSingleton(screen);
		}

		private (float, float3, float3) DistanceBetween(NativeArray<float3> pos, NativeArray<byte> id,
			NativeArray<float3> screen, int id1, int id2)
		{
			if (id[id1] != id1 || id[id2] != id2) return (-1f, float3.zero, float3.zero);
			return (math.distance(pos[id1], pos[id2]), math.lerp(pos[id1], pos[id2], 0.5f),
				math.lerp(screen[id1], screen[id2], 0.5f));
		}
	}
}