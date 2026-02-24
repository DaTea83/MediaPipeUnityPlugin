// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using System.IO;
using UnityEngine;

namespace Mediapipe.Unity
{
	public class AssetBundleResourceManager : IResourceManager
	{
		private static readonly string _TAG = nameof(AssetBundleResourceManager);

		private static string _assetBundlePath;
		private static string _cachePathRoot;

		public AssetBundleResourceManager(string assetBundleName, string cachePath = "Cache")
		{
			ResourceUtil.EnableCustomResolver();
			_assetBundlePath = Path.Combine(Application.streamingAssetsPath, assetBundleName);
			_cachePathRoot = Path.Combine(Application.persistentDataPath, cachePath);
		}

		private AssetBundleCreateRequest _assetBundleReq;
		private AssetBundle assetBundle => _assetBundleReq?.assetBundle;

		public void ClearAllCacheFiles()
		{
			if (Directory.Exists(_cachePathRoot))
			{
				Directory.Delete(_cachePathRoot, true);
			}
		}

		public IEnumerator LoadAssetBundleAsync()
		{
			if (assetBundle is not null)
			{
				Logger.LogWarning(_TAG, "AssetBundle is already loaded");
				yield break;
			}

			// No need to lock because this code can be run in main thread only.
			_assetBundleReq = AssetBundle.LoadFromFileAsync(_assetBundlePath);
			yield return _assetBundleReq;

			if (_assetBundleReq.assetBundle is null)
			{
				throw new IOException($"Failed to load {_assetBundlePath}");
			}
		}

		IEnumerator IResourceManager.PrepareAssetAsync(string name, string uniqueKey, bool overwriteDestination)
		{
			var destFilePath = GetCachePathFor(uniqueKey);
			ResourceUtil.SetAssetPath(name, destFilePath);

			if (File.Exists(destFilePath) && !overwriteDestination)
			{
				Logger.LogInfo(_TAG, $"{name} will not be copied to {destFilePath} because it already exists");
				yield break;
			}

			if (assetBundle is null)
			{
				yield return LoadAssetBundleAsync();
			}
			else
			{
				var assetLoadReq = assetBundle.LoadAssetAsync<TextAsset>(name);
				yield return assetLoadReq;

				if (assetLoadReq.asset == null)
				{
					throw new IOException($"Failed to load {name} from {assetBundle.name}");
				}

				Logger.LogVerbose(_TAG, $"Writing {name} data to {destFilePath}...");
				if (!Directory.Exists(_cachePathRoot))
				{
					var _ = Directory.CreateDirectory(_cachePathRoot);
				}

				var bytes = (assetLoadReq.asset as TextAsset)?.bytes;
				if (bytes == null) yield break;
				File.WriteAllBytes(destFilePath, bytes);
				Logger.LogVerbose(_TAG, $"{name} is saved to {destFilePath} (length={bytes.Length})");
			}
		}

		private static string GetCachePathFor(string assetName)
		{
			return Path.Combine(_cachePathRoot, assetName);
		}
	}
}