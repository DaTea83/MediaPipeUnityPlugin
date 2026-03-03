using System;
using System.Threading;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace EugeneC.Singleton
{
	public abstract class GenericSingleton<T> : MonoBehaviour
		where T : MonoBehaviour
	{
		public static T Instance { get; private set; }

		private CancellationTokenSource _cts = new();
		protected CancellationToken Token => _cts.Token;
		protected event Action OnCancelTask;

		protected void CancelTask()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = new CancellationTokenSource();
			OnCancelTask?.Invoke();
		}
		
		protected Random RandomInstance { get; private set; }

		protected virtual void InitSingleton()
		{
			if (Instance is not null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = (T)(MonoBehaviour)this;
		}

		protected virtual void UnInitSingleton()
		{
			if (Instance == this)
				Instance = null;
		}

		protected virtual void InitRandom()
		{
			var systemMilliseconds = (uint)Environment.TickCount;
#if UNITY_6000_3_OR_NEWER			
			RandomInstance = Random.CreateFromIndex(systemMilliseconds + (uint)this.GetEntityId());
#else
			RandomInstance = Random.CreateFromIndex(systemMilliseconds + (uint)this.GetInstanceID());
#endif
		}

		protected void KeepSingleton(bool keep)
		{
			if (keep) DontDestroyOnLoad(this);
		}

		protected virtual void Awake()
		{
			InitSingleton();
			InitRandom();
		}

		protected virtual void OnDisable()
		{
			CancelTask();
		}

		protected virtual void OnDestroy()
		{
			UnInitSingleton();
		}
	}
}