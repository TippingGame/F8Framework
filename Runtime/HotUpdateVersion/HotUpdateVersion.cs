using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace F8Framework.Core
{
    public class HotUpdateVersion : ModuleSingleton<HotUpdateVersion>, IModule
    {
        // 热更新内容根目录
        public static readonly string HotUpdateDir = Application.persistentDataPath + "/HotUpdate/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
        // 分包根目录
        public static readonly string SubPatchDir = Application.persistentDataPath + "/SubPackage/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
        
        
        public void OnInit(object createParam)
        {
            

        }

        // 初始化本地版本
        public void InitLocalVersion()
        {
            GameVersion gameVersion = Util.LitJson.ToObject<GameVersion>(Resources.Load<TextAsset>("GameVersion").ToString());
            GameConfig.LocalGameVersion = gameVersion;
        }
        
        // 初始化远程版本
        public IEnumerator InitRemoteVersion()
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate)
            {
                yield break;
            }
            UnityWebRequest webRequest = UnityWebRequest.Get(GameConfig.LocalGameVersion.EnableHotUpdate + "/GameVersion.json");
            yield return webRequest.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
            if (webRequest.result != UnityWebRequest.Result.Success)
#else
            if (webRequest.isNetworkError || webRequest.isHttpError)
#endif
            {
                LogF8.LogError($"获取远程版本失败：{GameConfig.LocalGameVersion.EnableHotUpdate} ，错误：{webRequest.error}");
            }
            else
            {
                string text = webRequest.downloadHandler.text;
                GameVersion gameVersion = Util.LitJson.ToObject<GameVersion>(text);
                GameConfig.RemoteGameVersion = gameVersion;
            }
            webRequest.Dispose();
            webRequest = null;
        }
        
        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            base.Destroy();
        }
    }
}
