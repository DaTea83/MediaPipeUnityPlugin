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
	public class StreamingAssetsResourceManager : IResourceManager
	{
		private const string Tag = nameof(StreamingAssetsResourceManager);

		private static string _assetPathRoot;
		private static string _cachePathRoot;

		public StreamingAssetsResourceManager(string path)
		{
			ResourceUtil.EnableCustomResolver();
			_assetPathRoot = Path.Combine(Application.streamingAssetsPath, path);
			_cachePathRoot = Path.Combine(Application.persistentDataPath, path);
		}

		public StreamingAssetsResourceManager() : this("")
		{
		}

		IEnumerator IResourceManager.PrepareAssetAsync(string name, string uniqueKey, bool overwriteDestination)
		{
			var destFilePath = GetCachePathFor(uniqueKey);
			ResourceUtil.SetAssetPath(name, destFilePath);

			if (File.Exists(destFilePath) && !overwriteDestination)
			{
				Logger.LogInfo(Tag, $"{name} will not be copied to {destFilePath} because it already exists");
				yield break;
			}

			var sourceFilePath = GetCachePathFor(name);
			if (!File.Exists(sourceFilePath)) yield return CreateCacheFile(name);

			if (sourceFilePath == destFilePath) yield break;

			Logger.LogVerbose(Tag, $"Copying {sourceFilePath} to {destFilePath}...");
			File.Copy(sourceFilePath, destFilePath, overwriteDestination);
			Logger.LogVerbose(Tag, $"{sourceFilePath} is copied to {destFilePath}");
		}

		private IEnumerator CreateCacheFile(string assetName)
		{
			var cacheFilePath = GetCachePathFor(assetName);

			if (File.Exists(cacheFilePath)) yield break;

#if !UNITY_ANDROID && !UNITY_WEBGL
			throw new FileNotFoundException($"{cacheFilePath} is not found");
#else
      var assetPath = GetAssetPathFor(assetName);
      using (var webRequest = UnityWebRequest.Get(assetPath))
      {
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
          if (!Directory.Exists(_CachePathRoot))
          {
            var _ = Directory.CreateDirectory(_CachePathRoot);
          }
          Logger.LogVerbose(_TAG, $"Writing {assetName} data to {cacheFilePath}...");
          var bytes = webRequest.downloadHandler.data;
          File.WriteAllBytes(cacheFilePath, bytes);
          Logger.LogVerbose(_TAG, $"{assetName} is saved to {cacheFilePath} (length={bytes.Length})");
        }
        else
        {
          throw new InternalException($"Failed to load {assetName}: {webRequest.error}");
        }
      }
#endif
		}

		private static string GetAssetPathFor(string assetName)
		{
			return Path.Combine(_assetPathRoot, assetName);
		}

		private static string GetCachePathFor(string assetName)
		{
			var assetPath = GetAssetPathFor(assetName);
			return File.Exists(assetPath) ? assetPath : Path.Combine(_cachePathRoot, assetName);
		}
	}
}