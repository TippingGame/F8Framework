namespace F8Framework.Core
{
	public abstract class ModuleSingleton<T> where T : class, IModule, new()
	{
		private static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
#if UNITY_EDITOR
					_instance = new T();
					LogF8.Log($"模块 {typeof(T)} 不是通过模块中心创建并控制（无法轮询Update），仅在编辑器下可以临时使用");
#else
					LogF8.LogError($"模块 {typeof(T)} 未创建。");
#endif
				}
				return _instance;
			}
		}

		protected ModuleSingleton()
		{
			if (_instance != null)
				LogF8.LogError($"模块 {typeof(T)} 实例已创建。");
			_instance = this as T;
		}
		
		protected void Destroy()
		{
			_instance = null;
		}
	}
}