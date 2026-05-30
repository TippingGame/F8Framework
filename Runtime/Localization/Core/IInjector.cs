using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
	public interface IInjector
	{
		void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase;
	}

	public interface IUnloadableInjector : IInjector
	{
		void Unload();
	}

	public abstract class AssetInjectorBase : IUnloadableInjector
	{
		private AssetLoadTracker assetLoadTracker;
		private int loadVersion;

		public abstract void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase;

		public virtual void Unload()
		{
			loadVersion++;
			ClearTarget();
			assetLoadTracker?.ReleaseAll();
			assetLoadTracker = null;
		}

		protected void UseDirectAsset()
		{
			Unload();
		}

		protected void LoadLocalizedAsset<T>(string assetName, Action<T> onLoaded) where T : Object
		{
			Unload();
			if (string.IsNullOrEmpty(assetName))
			{
				return;
			}

			var tracker = new AssetLoadTracker();
			assetLoadTracker = tracker;
			int currentVersion = ++loadVersion;
			tracker.LoadAsync<T>(assetName, asset =>
			{
				if (currentVersion != loadVersion || assetLoadTracker != tracker)
				{
					return;
				}

				onLoaded?.Invoke(asset);
			});
		}

		protected void LoadLocalizedAsset(string assetName, Action<Object> onLoaded)
		{
			Unload();
			if (string.IsNullOrEmpty(assetName))
			{
				return;
			}

			var tracker = new AssetLoadTracker();
			assetLoadTracker = tracker;
			int currentVersion = ++loadVersion;
			tracker.LoadAsync(assetName, asset =>
			{
				if (currentVersion != loadVersion || assetLoadTracker != tracker)
				{
					return;
				}

				onLoaded?.Invoke(asset);
			});
		}

		protected abstract void ClearTarget();
	}
}
