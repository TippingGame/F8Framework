using System.Collections;
using F8Framework.Core;
using F8Framework.F8ExcelDataClass;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
	public class LanguageCycler : MonoBehaviour
	{
		IEnumerator Start()
		{
			// 初始化模块中心
			ModuleCenter.Initialize(this);
			
			// 初始化版本
			FF8.HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();
        
			// 按顺序创建模块，可按需添加
			FF8.Message = ModuleCenter.CreateModule<MessageManager>();
			FF8.Input = ModuleCenter.CreateModule<InputManager>(new DefaultInputHelper());
			FF8.Storage = ModuleCenter.CreateModule<StorageManager>();
			FF8.Timer = ModuleCenter.CreateModule<TimerManager>();
			FF8.Procedure = ModuleCenter.CreateModule<ProcedureManager>();
			FF8.Network = ModuleCenter.CreateModule<NetworkManager>();
			FF8.FSM = ModuleCenter.CreateModule<FSMManager>();
			FF8.GameObjectPool = ModuleCenter.CreateModule<GameObjectPool>();
			FF8.Asset = ModuleCenter.CreateModule<AssetManager>();
#if UNITY_WEBGL
            yield return AssetBundleManager.Instance.LoadAssetBundleManifest(); // WebGL专用
#endif
			FF8.Config = ModuleCenter.CreateModule<F8DataManager>();
			FF8.Audio = ModuleCenter.CreateModule<AudioManager>();
			FF8.Tween = ModuleCenter.CreateModule<Tween>();
			FF8.UI = ModuleCenter.CreateModule<UIManager>();
#if UNITY_WEBGL
            yield return F8DataManager.Instance.LoadLocalizedStringsIEnumerator(); // WebGL专用
#endif
			FF8.Local = ModuleCenter.CreateModule<Localization>();
			FF8.SDK = ModuleCenter.CreateModule<SDKManager>();
			FF8.Download = ModuleCenter.CreateModule<DownloadManager>();
			FF8.LogWriter = ModuleCenter.CreateModule<F8LogWriter>();
			yield break;
		}

		void Update()
		{
			// 更新框架
			ModuleCenter.Update();
			if (Input.GetKeyDown(KeyCode.Return))
			{
				Cycle();
			}
		}

		void LateUpdate()
		{
			// 更新模块
			ModuleCenter.LateUpdate();
		}

		private void FixedUpdate()
		{
			ModuleCenter.FixedUpdate();
		}

		public void Cycle()
		{
			Localization.Instance.ActivateNextLanguage();
		}
	}
}

