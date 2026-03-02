using System.Threading;
using UnityEngine;

namespace EugeneC.Singleton
{
	public abstract class GenericSingleton<T> : MonoBehaviour
		where T : MonoBehaviour
	{
		public static T Instance { get; private set; }
		
		protected readonly CancellationTokenSource Cts = new();
		protected CancellationToken Token => Cts.Token;

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
			Cts.Cancel();
		}

		protected virtual void OnDestroy()
		{
			UnInitSingleton();
		}
	}
}