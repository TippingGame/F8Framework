using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace F8Framework.Tests
{
    public class DemoAssetManager : MonoBehaviour
    {
        IEnumerator Start()
        {
            /*----------所有加载均会自动判断是Resources资产还是AssetBundle资产----------*/
            
            // 编辑器模式，无需每次修改资源都按F8
            FF8.Asset.IsEditorMode = true;
            
            
            /*-------------------------------------同步加载-------------------------------------*/
            // 加载单个资产
            GameObject go = FF8.Asset.Load<GameObject>("Cube");

            // assetName：资产名
            // subAssetName：子资产名，使用Multiple模式的Sprite图片则可使用
            // 指定加载模式REMOTE_ASSET_BUNDLE，加载远程AssetBundle资产，需要配置AssetRemoteAddress = "http://127.0.0.1:6789/remote"
            Sprite sprite = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12", AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
            
            // 加载此资源的全部资产
            FF8.Asset.LoadAll("Cube");
            // 加载此资源的全部子资产
            FF8.Asset.LoadSub("Cube");
            
            
            /*-------------------------------------异步加载-------------------------------------*/
            FF8.Asset.LoadAsync<GameObject>("Cube", (go) =>
            {
                GameObject goo = Instantiate(go);
            });

            // async/await方式（无多线程，WebGL也可使用）
            // await FF8.Asset.LoadAsync<GameObject>("Cube");
            // 或者
            // BaseLoader load = FF8.Asset.LoadAsync<GameObject>("Cube");
            // await load;
            
            // 协程方式
            yield return FF8.Asset.LoadAsync<GameObject>("Cube");
            // 或者
            BaseLoader load2 = FF8.Asset.LoadAsync<GameObject>("Cube");
            yield return load2;
            GameObject go2 = load2.GetAssetObject<GameObject>();
            
            // 加载此资源的全部资产
            BaseLoader loaderAll = FF8.Asset.LoadAllAsync("Cube");
            yield return loaderAll;
            Dictionary<string, Object> allAsset = loaderAll.GetAllAssetObject();
            
            // 加载此资源的全部子资产
            BaseLoader loaderSub = FF8.Asset.LoadSubAsync("Atlas");
            yield return loaderSub;
            Dictionary<string, Sprite> allAsset2 = loaderSub.GetAllAssetObject<Sprite>();
            
            
            /*-------------------------------------加载文件夹内首层资产-------------------------------------*/
            // 加载文件夹内首层资产（不遍历所有文件夹）
            FF8.Asset.LoadDir("NewFolder");
            
            // async/await方式（无多线程，WebGL也可使用）
            // BaseDirLoader loadDir = FF8.Asset.LoadDirAsync("NewFolder", () => { });
            // await loadDir;
            
            // 加载文件夹内资产
            BaseDirLoader loadDir2 = FF8.Asset.LoadDirAsync("NewFolder", () => { });
            yield return loadDir2;
            
            // 你可以查看所有资产的BaseLoader
            List<BaseLoader> loaders = loadDir2.Loaders;
            
            // 也可以这样设置查看加载进度
            foreach (var item in FF8.Asset.LoadDirAsyncCoroutine("NewFolder"))
            {
                yield return item;
            }

            // 也可以这样
            var loadDir3 = FF8.Asset.LoadDirAsyncCoroutine("NewFolder").GetEnumerator();
            while (loadDir3.MoveNext())
            {
                yield return loadDir3.Current;
            }
            
            
            /*-------------------------------------其他功能-------------------------------------*/
            // 获取此资源的全部资产
            Dictionary<string, Object> allAsset3 = FF8.Asset.GetAllAssetObject("Cube");
            
            // 只获取指定类型
            Dictionary<string, Sprite> allAsset4 = FF8.Asset.GetAllAssetObject<Sprite>("Atlas");
            
            // 获取单个资产
            GameObject go3 = FF8.Asset.GetAssetObject<GameObject>("Cube");
            
            // 获取加载进度
            float loadProgress = FF8.Asset.GetLoadProgress("Cube");

            // 获取所有加载器的进度
            float loadProgress2 = FF8.Asset.GetLoadProgress();

            // 同步卸载资产
            FF8.Asset.Unload("Cube", false); //根据AbPath卸载资产，如果设置为 true，完全卸载。

            // 异步卸载资产
            FF8.Asset.UnloadAsync("Cube", false, () =>
            {
                // 卸载资产完成
            });
            
            
            /*-------------------------------------其他类型加载示例-------------------------------------*/
            // 加载场景，别忘了加载天空盒材质，不然会变紫色，并且这种方式不能加载Resources目录中的场景（不过可以手动放入Build Setting处）
            FF8.Asset.Load("Scene");
            SceneManager.LoadScene("Scene");
            
            // 使用图集首先需要，加载图集
            FF8.Asset.Load("SpriteAtlas");
            
            // 假如将图集与图片改为同一AB名，则无需预先加载图集
            FF8.Asset.LoadAsync<Sprite>("PackForest_2", sprite =>
            {
                LogF8.Log(sprite);
            });
            
            // 图片加载需要小心区分Texture2D和Sprite，当资源被当成Texture2D加载后，则加载不出Sprite类型
            FF8.Asset.Load<Texture2D>("PackForest_2");
        }
    }
}
