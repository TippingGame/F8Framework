# F8 CacheStorage

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 CacheStorage组件，高速本地缓存，下载并缓存byte、json、text、Texture等可序列化资产，设置缓存数量、大小、存活时间，清理缓存。

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 代码使用方法
```C#
        void Start()
        {
            // 缓存最大数量，超出后删除优先级低的资源
            int maxCount = 10000;
            // 5 MB，缓存最大容量，超出后删除优先级低的资源
            int maxSize = 5 * 1024 * 1024;
            // 30天，缓存失效时间
            double reRequestTime = 30 * 24 * 60 * 60;
            // 默认失效时间，ALWAYS 根据服务器缓存，FIRSTPLAY 重启游戏后缓存失效，ONCE 只使用一次，LOCAL 使用本地缓存
            CacheRequestType defaultRequestType = CacheRequestType.FIRSTPLAY;
            // 1年，清除在设定的时间（秒）内未使用的缓存。
            double unusedPeriodTime = 365 * 24 * 60 * 60;
            // 1秒，移除缓存的间隔时间
            double removeCycle = 1;
            
            // 初始化，参数为 0 则功能不启用
            CacheStorage.Initialize(maxCount, maxSize, reRequestTime, defaultRequestType, unusedPeriodTime, removeCycle);
            
            // 请求Request
            string url = "";
            // 自定义类型，默认使用初始化的类型
            CacheRequestType requestType = CacheRequestType.ALWAYS;
            CacheRequestOperation cro = CacheStorage.Request(url, requestType, (CacheResult result) =>
            {
                // success
                if (result.IsSuccess() == true)
                {
                    // date
                    byte[] data = result.Data;

                    // text - Encoding.UTF8
                    string text = result.Text;

                    // text - Encoding.UTF8
                    text = result.GetTextData();

                    // text - Encoding.Default
                    text = result.GetTextData(Encoding.Default);    

                    // json - Encoding.UTF8
                    // JsonClass json = result.GetJsonData<JsonClass>();

                    // json - Encoding.Default
                    // json = result.GetJsonData<JsonClass>(Encoding.Default);
                }
            });
            
            // 停止
            cro.Cancel();
            
            // http请求
            CacheStorage.RequestHttpCache(url, (CacheResult result) =>
            {
                if (result.IsSuccess() == true)
                {
                    byte[] data = result.Data;
                }
            });
            
            // 请求Texture
            CacheStorage.GetCachedTexture(url, (CachedTexture cachedTexture) =>
            {
                if (cachedTexture != null)
                {
                    Texture texture = cachedTexture.texture;
                    
                    // 如果为true，则这是来自web的新请求。
                    bool requested = cachedTexture.requested;

                    // 如果为true，则这是最新更新的纹理。
                    bool updateData = cachedTexture.updateData;

                    // 销毁Texture
                    cachedTexture.DestroyTexture();

                    // 移除缓存
                    cachedTexture.ReleaseCache();
                }
            });
            
            // 移除缓存
            CacheStorage.RemoveCache(url);
            
            // 清空缓存
            CacheStorage.ClearCache();
        }
        
        // 协程形式
        public IEnumerator Something()
        {
            string url = "";
            CacheRequestOperation cro = CacheStorage.Request(url, (CacheResult result) =>
            {
                if (result.IsSuccess() == true)
                {
                    byte[] data = result.Data;
                }
            });

            while(cro.keepWaiting == true)
            {
                yield return null;
            }
        }
```
## 拓展功能
1. 高速本地缓存 https://github.com/nhn/gpm.unity.git  

