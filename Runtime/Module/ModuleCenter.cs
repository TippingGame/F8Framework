using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
	public static class ModuleCenter
	{
		private class ModuleWrapper
		{
			public int Priority { private set; get; }
			public IModule Module { private set; get; }

			public ModuleWrapper(IModule module, int priority)
			{
				Module = module;
				Priority = priority;
			}
		}

		private static readonly List<ModuleWrapper> _coms = new List<ModuleWrapper>(100);
		private static readonly List<ModuleWrapper> _comsUpdate = new List<ModuleWrapper>(100);
		private static readonly List<ModuleWrapper> _comsFixedUpdate = new List<ModuleWrapper>(100);
		private static MonoBehaviour _behaviour;
		private static bool _isDirty = false;
		private static bool _isDirtyFixed = false;
		private static long _frame = 0;

		/// <summary>
		/// 初始化框架
		/// </summary>
		public static void Initialize(MonoBehaviour behaviour)
		{
			if (behaviour == null)
				throw new Exception("MonoBehaviour 为空。");
			if (_behaviour != null)
				throw new Exception($"{nameof(ModuleCenter)} 已初始化。");

			UnityEngine.Object.DontDestroyOnLoad(behaviour.gameObject);
			_behaviour = behaviour;

			behaviour.StartCoroutine(CheckFrame());
		}

		/// <summary>
		/// 检测ModuleCenter更新方法
		/// </summary>
		private static IEnumerator CheckFrame()
		{
			var wait = new WaitForSeconds(1f);
			yield return wait;

			// 说明：初始化之后，如果忘记更新ModuleCenter，这里会抛出异常
			if (_frame == 0)
				throw new Exception($"请调用更新方法：ModuleCenter.Update");
		}

		/// <summary>
		/// 更新框架
		/// </summary>
		public static void Update()
		{
			_frame++;

			// 如果有新模块需要重新排序
			if (_isDirty)
			{
				_isDirty = false;

				_comsUpdate.Sort((left, right) =>
				{
					if (left.Priority < right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}

			// 轮询所有模块
			for (int i = 0; i < _comsUpdate.Count; i++)
			{
				_comsUpdate[i]?.Module.OnUpdate();
			}
			
			// 轮询所有模块
			for (int i = 0; i < _comsUpdate.Count; i++)
			{
				_comsUpdate[i]?.Module.OnLateUpdate();
			}
		}
		
		/// <summary>
		/// 更新框架
		/// </summary>
		public static void FixedUpdate()
		{
			// 如果有新模块需要重新排序
			if (_isDirtyFixed)
			{
				_isDirtyFixed = false;
				
				_comsFixedUpdate.Sort((left, right) =>
				{
					if (left.Priority < right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}

			// 轮询所有模块
			for (int i = 0; i < _comsFixedUpdate.Count; i++)
			{
				_comsFixedUpdate[i]?.Module.OnFixedUpdate();
			}
		}
		
		/// <summary>
		/// 查询游戏模块是否存在
		/// </summary>
		public static bool Contains<T>() where T : class, IModule
		{
			System.Type type = typeof(T);
			return Contains(type);
		}

		/// <summary>
		/// 查询游戏模块是否存在
		/// </summary>
		public static bool Contains(System.Type moduleType)
		{
			for (int i = 0; i < _coms.Count; i++)
			{
				if (_coms[i].Module.GetType() == moduleType)
					return true;
			}
			return false;
		}

		/// <summary>
		/// 创建游戏模块
		/// </summary>
		/// <typeparam name="T">模块类</typeparam>
		/// <param name="priority">运行时的优先级，优先级越大越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateModule<T>(int priority = 0) where T : class, IModule
		{
			return CreateModule<T>(null, priority);
		}

		/// <summary>
		/// 创建游戏模块
		/// </summary>
		/// <typeparam name="T">模块类</typeparam>
		/// <param name="createParam">创建参数</param>
		/// <param name="priority">运行时的优先级，优先级越大越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateModule<T>(System.Object createParam, int priority = 0) where T : class, IModule
		{
			if (priority < 0)
				throw new Exception("优先级不能为负");

			if (Contains(typeof(T)))
				throw new Exception($"游戏模块 {typeof(T)} 已存在");

			// 如果没有设置优先级
			if (priority == 0)
			{
				int minPriority = GetMaxPriority();
				priority = ++minPriority;
			}

			LogF8.Log($"创建游戏模块: {typeof(T)}");

			T module = null;

			// 检查类型是否是 MonoBehaviour 的子类
			if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T)))
			{
				GameObject obj = new GameObject(typeof(T).Name, typeof(T));
				module = obj.GetComponent<T>();
			}
			else
			{
				module = Activator.CreateInstance<T>();
			}

			ModuleWrapper wrapper = new ModuleWrapper(module, priority);
			wrapper.Module.OnInit(createParam);
			_coms.Add(wrapper);
			_coms.Sort((left, right) =>
			{
				if (left.Priority < right.Priority)
					return -1;
				else if (left.Priority == right.Priority)
					return 0;
				else
					return 1;
			});
			if (typeof(T).GetCustomAttributes(typeof(UpdateRefreshAttribute), false).Length > 0)
			{
				_comsUpdate.Add(wrapper);
				_isDirty = true;
			}
			if (typeof(T).GetCustomAttributes(typeof(FixedUpdateRefreshAttribute), false).Length > 0)
			{
				_comsFixedUpdate.Add(wrapper);
				_isDirtyFixed = true;
			}
			return module;
		}

		/// <summary>
		/// 销毁模块
		/// </summary>
		/// <typeparam name="T">模块类</typeparam>
		public static bool DestroyModule<T>()
		{
			var moduleType = typeof(T);
			for (int i = 0; i < _comsUpdate.Count; i++)
			{
				if (_comsUpdate[i].Module.GetType() == moduleType)
				{
					_comsUpdate[i].Module.OnTermination();
					_comsUpdate.RemoveAt(i);
				}
			}
			
			for (int i = 0; i < _comsFixedUpdate.Count; i++)
			{
				if (_comsFixedUpdate[i].Module.GetType() == moduleType)
				{
					_comsFixedUpdate[i].Module.OnTermination();
					_comsFixedUpdate.RemoveAt(i);
				}
			}
			
			for (int i = 0; i < _coms.Count; i++)
			{
				if (_coms[i].Module.GetType() == moduleType)
				{
					_coms[i].Module.OnTermination();
					_coms.RemoveAt(i);
					return true;
				}
			}
			
			return false;
		}

		/// <summary>
		/// 获取游戏模块
		/// </summary>
		/// <typeparam name="T">模块类</typeparam>
		public static T GetModule<T>() where T : class, IModule
		{
			System.Type type = typeof(T);
			for (int i = 0; i < _coms.Count; i++)
			{
				if (_coms[i].Module.GetType() == type)
					return _coms[i].Module as T;
			}

			LogF8.LogError($"未找到游戏模块 {type}");
			return null;
		}

		/// <summary>
		/// 获取当前模块里最大的优先级数值
		/// </summary>
		private static int GetMaxPriority()
		{
			int maxPriority = int.MinValue; // 初始化为 int 类型的最小值
			for (int i = 0; i < _coms.Count; i++)
			{
				if (_coms[i].Priority > maxPriority)
					maxPriority = _coms[i].Priority;
			}
			return maxPriority; // 大于等于零
		}

		#region 协程相关
		/// <summary>
		/// 开启一个协程
		/// </summary>
		public static Coroutine StartCoroutine(IEnumerator coroutine)
		{
			if (_behaviour == null)
				throw new Exception($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			return _behaviour.StartCoroutine(coroutine);
		}

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public static void StopCoroutine(Coroutine coroutine)
		{
			if (_behaviour == null)
				throw new Exception($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			_behaviour.StopCoroutine(coroutine);
		}


		/// <summary>
		/// 开启一个协程
		/// </summary>
		public static void StartCoroutine(string methodName)
		{
			if (_behaviour == null)
				throw new Exception($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			_behaviour.StartCoroutine(methodName);
		}

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public static void StopCoroutine(string methodName)
		{
			if (_behaviour == null)
				throw new Exception($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			_behaviour.StopCoroutine(methodName);
		}


		/// <summary>
		/// 停止所有协程
		/// </summary>
		public static void StopAllCoroutines()
		{
			if (_behaviour == null)
				throw new Exception($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			_behaviour.StopAllCoroutines();
		}
		#endregion
	}
}