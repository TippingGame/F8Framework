using F8Framework.Core;
using UnityEngine;

public class LanguageCycler : MonoBehaviour
{
	private void Start()
	{
		// 初始化模块中心
		ModuleCenter.Initialize(this);

		// 按顺序创建模块
		FF8.Message.ToString();
		FF8.Storage.ToString();
		FF8.Timer.ToString();
		FF8.Procedure.ToString();
		FF8.FSM.ToString();
		FF8.GameObjectPool.ToString();
		FF8.PoolGlobal.ToString();
		FF8.Asset.ToString();
		// FF8.Config.ToString();
		FF8.Audio.ToString();
		FF8.Tween.ToString();
		FF8.UI.ToString();
		FF8.Local.ToString();
		FF8.SDK.ToString();
		FF8.LogHelper.ToString();
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

