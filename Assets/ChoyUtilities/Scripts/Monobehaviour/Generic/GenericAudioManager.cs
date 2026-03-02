using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

namespace EugeneC.Singleton
{
#if UNITY_2023_2_OR_NEWER

	// For audio randomization use the Unity new Random Audio Container, hence why there's a version requirement
	public abstract class GenericAudioManager<TEnum, TMono> : GenericSingleton<TMono>
		where TEnum : Enum
		where TMono : MonoBehaviour
	{
		[Serializable]
		public struct AudioResourceSerialize
		{
			public TEnum id;
			public AudioResource audio;
		}

		public enum EAudioPriority : byte
		{
			Highest = 0,
			UltraHigh = 1 << 0,
			VeryHigh = 1 << 1,
			High = 1 << 2,
			AboveAverage = 1 << 3,
			Average = 1 << 4,
			BelowAverage = 1 << 5,
			Low = 1 << 6,
			VeryLow = 1 << 7,
			Lowest = byte.MaxValue,
		}

		[SerializeField] protected AudioResourceSerialize[] audioResource;
		[SerializeField] protected AudioSource audioSourcePrefab;
		[SerializeField] protected byte poolCount = 32;
		[SerializeField] protected bool loop;
		[SerializeField] protected AudioMixerGroup audioMixerGroup;
		[SerializeField] protected EAudioPriority priority;

		protected AudioSource[] AudioSources;
		protected int CurrentIndex;
		protected int PreviousIndex;
		protected List<int> PauseIndexes;

		protected virtual async void Start()
		{
			try
			{
				await Awaitable.NextFrameAsync(Cts.Token);
				if (audioSourcePrefab is null) return;

				AudioSources = new AudioSource[poolCount];
				for (var i = 0; i < poolCount; i++)
				{
					var spawnAudio = Instantiate(audioSourcePrefab, transform);
					spawnAudio.loop = loop;
					spawnAudio.outputAudioMixerGroup = audioMixerGroup;
					spawnAudio.priority = (int)priority;
					AudioSources[i] = spawnAudio;
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		
		public virtual float PlayClipAtPos(TEnum id, float3 pos, byte audioPriority = (byte)EAudioPriority.Average)
		{
			if (!Enum.IsDefined(typeof(TEnum), id)) return 0f;
			var resourceIndex = Array.FindIndex(audioResource,
				r => EqualityComparer<TEnum>.Default.Equals(r.id, id));

			var resource = audioResource[resourceIndex].audio;
			return PlayClipAtPos(resource, pos, audioPriority);
		}

		public virtual float PlayClipAtPos(AudioResource resource, float3 pos,
			byte audioPriority = (byte)EAudioPriority.Average)
		{
			var currentSource = AudioSources[CurrentIndex];

			currentSource.transform.localPosition = pos;
			currentSource.resource = resource;
			currentSource.priority = audioPriority;
			currentSource.Play();
			
			var lengthSeconds = currentSource.clip?.length ?? 0f;

			PreviousIndex = CurrentIndex;
			CurrentIndex++;
			CurrentIndex %= AudioSources.Length;
			
			return lengthSeconds;
		}

		public virtual float PlayClip(TEnum id, byte audioPriority = (byte)EAudioPriority.Average) =>
			PlayClipAtPos(id, float3.zero, audioPriority);

		public virtual float PlayClip(AudioResource resource, byte audioPriority = (byte)EAudioPriority.Average) =>
			PlayClipAtPos(resource, float3.zero, audioPriority);

		public virtual bool StopClip(int idx = -1)
		{
			idx = idx == -1 ? PreviousIndex : idx;
			var source = AudioSources[idx];
			if (!source.isPlaying) return false;
			source.Stop();
			return true;
		}

		public virtual bool PauseAllClips(bool isStop = false)
		{
			PauseIndexes = new();
			for (var i = 0; i < AudioSources.Length; i++)
			{
				var currentSource = AudioSources[i];
				if (!currentSource.isPlaying) continue;
				if (!isStop)
					currentSource.Pause();
				else
					currentSource.Stop();
				PauseIndexes.Add(i);
			}
			
			return PauseIndexes.Count == AudioSources.Length;
		}

		public virtual bool ResumeClips()
		{
			if (PauseIndexes is null) return false;

			foreach (var index in PauseIndexes)
				AudioSources[index].Play();

			PauseIndexes.Clear();
			
			return true;
		}
	}

#endif
}