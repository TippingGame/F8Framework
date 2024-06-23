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
			public int Priority = 0;
			public IModule Module = null;
			public bool ShouldBeRemoved = false;
			
			public ModuleWrapper(IModule module, int priority)
			{
				Module = module;
				Priority = priority;
			}
		}

		private static MonoBehaviour _behaviour;
		private static List<ModuleWrapper> _coms = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsUpdate = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsLateUpdate = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsFixedUpdate = new List<ModuleWrapper>(100);
		
		private static bool _isDirty = false;
		private static bool _isDirtyLate = false;
		private static bool _isDirtyFixed = false;
		private static long _frame = 0;

		/// <summary>
		/// 初始化框架
		/// </summary>
		public static void Initialize(MonoBehaviour behaviour)
		{
			if (behaviour == null)
				LogF8.LogError("MonoBehaviour 为空。");
			if (_behaviour != null)
			{
				LogF8.LogError($"{nameof(ModuleCenter)} 已初始化。");
				return;
			}

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

			// 说明：初始化之后，如果忘记更新ModuleCenter
			if (_frame == 0)
			{
				LogF8.LogError($"暂未调用轮询方法：ModuleCenter.Update");
				LogF8.LogError($"暂未调用轮询方法：ModuleCenter.LateUpdate");
				LogF8.LogError($"暂未调用轮询方法：ModuleCenter.FixedUpdate");
			}
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

			// 遍历所有模块
			for (int i = 0; i < _comsUpdate.Count; i++)
			{
				if (_comsUpdate[i]?.Module == null || _comsUpdate[i].ShouldBeRemoved)
				{
					_comsUpdate.RemoveAt(i);
					i--;
					continue;
				}
				
				// 执行模块更新
				_comsUpdate[i].Module.OnUpdate();
			}
		}
		
		/// <summary>
		/// 更新框架
		/// </summary>
		public static void LateUpdate()
		{
			// 如果有新模块需要重新排序
			if (_isDirtyLate)
			{
				_isDirtyLate = false;
				
				_comsLateUpdate.Sort((left, right) =>
				{
					if (left.Priority < right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}
			
			// 遍历所有模块
			for (int i = 0; i < _comsLateUpdate.Count; i++)
			{
				if (_comsLateUpdate[i]?.Module == null || _comsLateUpdate[i].ShouldBeRemoved)
				{
					_comsLateUpdate.RemoveAt(i);
					i--;
					continue;
				}
				
				// 执行模块更新
				_comsLateUpdate[i].Module.OnLateUpdate();
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
			
			// 遍历所有模块
			for (int i = 0; i < _comsFixedUpdate.Count; i++)
			{
				if (_comsFixedUpdate[i]?.Module == null || _comsFixedUpdate[i].ShouldBeRemoved)
				{
					_comsFixedUpdate.RemoveAt(i);
					i--;
					continue;
				}
				
				// 执行模块更新
				_comsFixedUpdate[i].Module.OnFixedUpdate();
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
		/// <param name="priority">运行时的优先级，优先级越小越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateModule<T>(int priority = 0) where T : class, IModule
		{
			return CreateModule<T>(null, priority);
		}

		/// <summary>
		/// 创建游戏模块
		/// </summary>
		/// <typeparam name="T">模块类</typeparam>
		/// <param name="createParam">创建参数</param>
		/// <param name="priority">运行时的优先级，优先级越小越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateModule<T>(System.Object createParam, int priority = 0) where T : class, IModule
		{
			if (priority < 0)
			{
				LogF8.LogError("优先级不能为负");
				priority = 0;
			}
			
			if (Contains(typeof(T)))
			{
				LogF8.LogError($"游戏模块 {typeof(T)} 已存在");
				return GetModule<T>();
			}
			
			// 如果没有设置优先级
			if (priority == 0)
			{
				int minPriority = GetMaxPriority();
				priority = ++minPriority;
			}

			LogF8.LogModule($"创建游戏模块: {typeof(T).Name}");

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
			if (typeof(T).GetCustomAttributes(typeof(LateUpdateRefreshAttribute), false).Length > 0)
			{
				_comsLateUpdate.Add(wrapper);
				_isDirtyLate = true;
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
					_comsUpdate[i].ShouldBeRemoved = true;
				}
			}
			
			for (int i = 0; i < _comsLateUpdate.Count; i++)
			{
				if (_comsLateUpdate[i].Module.GetType() == moduleType)
				{
					_comsLateUpdate[i].ShouldBeRemoved = true;
				}
			}
			
			for (int i = 0; i < _comsFixedUpdate.Count; i++)
			{
				if (_comsFixedUpdate[i].Module.GetType() == moduleType)
				{
					_comsFixedUpdate[i].ShouldBeRemoved = true;
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
		
		/// <summary>
		/// 获取初始化MonoBehaviour
		/// </summary>
		public static MonoBehaviour GetBehaviour()
		{
			if (_behaviour == null)
				LogF8.LogError($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			return _behaviour;
		}
	}
}