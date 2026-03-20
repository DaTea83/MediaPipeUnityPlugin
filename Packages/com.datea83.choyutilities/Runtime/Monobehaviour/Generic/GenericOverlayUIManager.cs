using System;
using EugeneC.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace EugeneC.Singleton {
	
	[RequireComponent(typeof(UIDocument))]
	public abstract class GenericOverlayUIManager<TMono, TObj> : GenericSingleton<TMono> 
		where TMono : MonoBehaviour
		where TObj : ScriptableObject{

		[Serializable]
		public struct BindingSerializable {
			public TObj binder;
			public EVisualElements bindType;
			public string bindName;
		}
		
		[SerializeField] protected UIDocument overlayUI;
		[SerializeField] protected BindingSerializable[] bindings;
		
		protected VisualElement root => overlayUI?.rootVisualElement;

		protected virtual void OnValidate() {
			overlayUI = GetComponent<UIDocument>();
		}

		private void OnEnable() {
			TryBindAll();
		}
		
		protected void TryBindAll()
		{
			if (overlayUI is null)
			{
				Debug.LogWarning($"{GetType().Name}: overlayUI is not assigned.", this);
				return;
			}
			
			if (root is null)
			{
				Debug.LogWarning($"{GetType().Name}: rootVisualElement is null.", this);
				return;
			}

			if (bindings is null || bindings.Length == 0)
			{
				Debug.LogWarning($"{GetType().Name}: No bindings assigned.", this);
				return;
			}

			for (var i = 0; i < bindings.Length; i++)
			{
				var binding = bindings[i];

				if (binding.binder is null)
				{
					Debug.LogWarning($"{GetType().Name}: Binding at index {i} has no {typeof(TObj).Name} assigned.", this);
					continue;
				}

				if (string.IsNullOrWhiteSpace(binding.bindName))
				{
					Debug.LogWarning($"{GetType().Name}: Binding at index {i} has an empty element name.", this);
					continue;
				}
				
				var element = root.Q<VisualElement>(binding.bindName);

				if (element is null)
				{
					Debug.LogWarning(
						$"{GetType().Name}: Could not find VisualElement named '{binding.bindName}' for data '{binding.binder.name}'.",
						this);
					OnBindFailed(binding);
					continue;
				}

				OnBindSuccess(binding, element);
			}
		}

		protected virtual void OnBindFailed(BindingSerializable binding) { }

		protected abstract void OnBindSuccess(BindingSerializable binding, VisualElement element);
	}
	
}