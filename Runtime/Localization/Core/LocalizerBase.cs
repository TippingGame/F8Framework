using UnityEngine;

namespace F8Framework.Core
{
	public abstract class LocalizerBase : MonoBehaviour
	{
		protected IInjector injector;

		protected virtual void Awake()
		{
			Localization.Instance.AddLocalizer(this);
			Prepare();
		}

		/// <summary>
		/// 准备对目标组件的引用。
		/// </summary>
		protected abstract void Prepare();

		protected virtual void Start()
		{
			Localize();
		}

		/// <summary>
		/// 本地化目标组件。
		/// </summary>
		internal abstract void Localize();

		protected virtual void OnDestroy()
		{
			Localization.Instance.RemoveLocalizer(this);
		}
	}
}
