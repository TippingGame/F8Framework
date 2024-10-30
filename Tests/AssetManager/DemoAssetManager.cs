using System.Collections;
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

            /*-------------------------------------同步加载-------------------------------------*/
            GameObject go = FF8.Asset.Load<GameObject>("Cube");

            // 指定加载模式REMOTE_ASSET_BUNDLE，加载远程AssetBundle资产，需要配置AssetRemoteAddress = "http://127.0.0.1:6789/remote"
            GameObject go5 = FF8.Asset.Load<GameObject>("Cube", AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);

            // 加载文件夹内资产（不遍历所有文件夹）
            FF8.Asset.LoadDir("NewFolder");


            /*-------------------------------------异步加载-------------------------------------*/

            FF8.Asset.LoadAsync<GameObject>("Cube", (go) =>
            {
                GameObject goo = Instantiate(go);
            });

            // 协程
            var load = FF8.Asset.LoadAsyncCoroutine<GameObject>("Cube");
            yield return load;
            GameObject go2 = FF8.Asset.GetAssetObject<GameObject>("Cube");

            // 加载文件夹内资产
            FF8.Asset.LoadDirAsync("NewFolder", () => { });

            // 协程，迭代文件夹内资产（不遍历所有文件夹）
            foreach (var item in FF8.Asset.LoadDirAsyncCoroutine("NewFolder"))
            {
                yield return item;
            }

            // 也可以这样
            var loadDir = FF8.Asset.LoadDirAsyncCoroutine("NewFolder").GetEnumerator();
            while (loadDir.MoveNext())
            {
                yield return loadDir.Current;
            }


            /*-------------------------------------其他功能-------------------------------------*/

            // 获取加载进度
            float loadProgress = FF8.Asset.GetLoadProgress("Cube");

            // 获取所有加载器的进度
            float loadProgress2 = FF8.Asset.GetLoadProgress();

            // 同步卸载资产
            FF8.Asset.Unload("Cube", false); //根据AbPath卸载资产，如果设置为 true，完全卸载。

            // 步卸载资产
            FF8.Asset.UnloadAsync("Cube", false, () =>
            {
                // 卸载资产完成
            });

            // 编辑器模式，无需打包AB
            FF8.Asset.IsEditorMode = true;
            
            // 加载场景，别忘了加载天空盒材质，不然会变紫色
            FF8.Asset.Load("Scene");
            SceneManager.LoadScene("Scene");
            
            // 使用图集首先需要，加载图集
            FF8.Asset.Load("SpriteAtlas");
        }
    }
}
