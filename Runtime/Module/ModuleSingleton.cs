namespace F8Framework.Core
{
	public abstract class ModuleSingleton<T> where T : class, IModule
	{
		private static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
					LogF8.LogError($"{typeof(T)} 未创建。");
				return _instance;
			}
		}

		protected ModuleSingleton()
		{
			if (_instance != null)
				throw new System.Exception($"{typeof(T)} 实例已创建。");
			_instance = this as T;
		}
		
		protected void Destroy()
		{
			_instance = null;
		}
	}
}