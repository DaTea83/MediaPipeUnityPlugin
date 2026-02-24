// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;

namespace Mediapipe.Unity.Sample
{
	public static class AssetLoader
	{
		private static IResourceManager _resourceManager;

		public static void Provide(IResourceManager manager) => _resourceManager = manager;

		public static IEnumerator PrepareAssetAsync(string name, string uniqueKey, bool overwrite = false)
		{
			return _resourceManager is null ? 
				throw new System.InvalidOperationException("ResourceManager is not provided") 
				: _resourceManager.PrepareAssetAsync(name, uniqueKey, overwrite);
		}

		public static IEnumerator PrepareAssetAsync(string name, bool overwrite = false) =>
			PrepareAssetAsync(name, name, overwrite);
	}
}