// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Mediapipe.Unity.Sample
{
	public static class ImageSourceProvider
	{
		private static WebCamSource _webCamSource;
		private static StaticImageSource _staticImageSource;
		private static VideoSource _videoSource;

		public static ImageSource ImageSource { get; private set; }

		public static ImageSourceType CurrentSourceType
		{
			get
			{
				return ImageSource switch
				{
					WebCamSource => ImageSourceType.WebCamera,
					StaticImageSource => ImageSourceType.Image,
					VideoSource => ImageSourceType.Video,
					_ => ImageSourceType.Unknown
				};
			}
		}

		internal static void Initialize(WebCamSource webCamSource, StaticImageSource staticImageSource,
			VideoSource videoSource)
		{
			_webCamSource = webCamSource;
			_staticImageSource = staticImageSource;
			_videoSource = videoSource;
		}

		public static void Switch(ImageSourceType imageSourceType)
		{
			switch (imageSourceType)
			{
				case ImageSourceType.WebCamera:
				{
					ImageSource = _webCamSource;
					break;
				}
				case ImageSourceType.Image:
				{
					ImageSource = _staticImageSource;
					break;
				}
				case ImageSourceType.Video:
				{
					ImageSource = _videoSource;
					break;
				}
				case ImageSourceType.Unknown:
				default:
				{
					throw new System.ArgumentException($"Unsupported source type: {imageSourceType}");
				}
			}
		}
	}
}