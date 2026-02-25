using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace EugeneC.Utilities
{
	public class ActivateWebcam : MonoBehaviour
	{
		[SerializeField] private RawImage screen;
		[SerializeField] private int width = 1280;
		[SerializeField] private int height = 720;
		[SerializeField] private int fps = 30;

		private WebCamTexture _webCamTexture;
		private readonly CancellationTokenSource _tokenSource = new();
		
		private async void Start()
		{
			try
			{
				if (WebCamTexture.devices.Length == 0)
				{
					throw new Exception("Web Camera devices are not found");
				}
				
				var webCamDevice = WebCamTexture.devices[0];
				_webCamTexture = new WebCamTexture(webCamDevice.name, width, height, fps);
				_webCamTexture.Play();
				
				await _tokenSource.Token.AwaitableUntil(() => _webCamTexture.width > 16);
				
				screen.rectTransform.sizeDelta = new Vector2(width, height);
				screen.texture = _webCamTexture;
			}
			catch (Exception e) { print(e); }
		}

		private void OnDestroy()
		{
			_webCamTexture?.Stop();
			_tokenSource.Cancel();
		}
	}
}