using System.Collections;
using System.Collections.Generic;
using System.IO;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Core
{
    public class HotUpdate : ModuleSingleton<HotUpdate>, IModule
    {
        // 热更新内容根目录
        public static readonly string HotUpdateDir = Application.persistentDataPath + "/HotUpdate/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
        // 分包根目录
        public static readonly string SubPatchDir = Application.persistentDataPath + "/SubPackage/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
        
        public void OnInit(object createParam)
        {
            
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
