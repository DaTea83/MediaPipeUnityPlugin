using System;
using System.Threading;
using UnityEngine;

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

		protected void KeepSingleton(bool keep)
		{
			if (keep) DontDestroyOnLoad(this);
		}

		protected virtual void Awake()
		{
			InitSingleton();
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