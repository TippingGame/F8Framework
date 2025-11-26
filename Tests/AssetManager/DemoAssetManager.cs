using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace F8Framework.Tests
{
    public class DemoAssetManager : MonoBehaviour
    {
        IEnumerator Start()
        {
            /*========== 基础配置 ==========*/
            // 启用编辑器模式（无需每次修改资源都按F8），也可在菜单栏勾选
            FF8.Asset.IsEditorMode = true;
            

            /*========== 1. 同步加载 ==========*/

            // 基础加载 - 自动识别 Resources 或 AssetBundle
            GameObject cube = FF8.Asset.Load<GameObject>("Cube");

            // 完整路径加载（需在 F5 打包工具中启用对应功能）
            GameObject prefab1 = FF8.Asset.Load<GameObject>("AssetBundles/Prefabs/Cube");
            GameObject prefab2 = FF8.Asset.Load<GameObject>("Resources/Prefabs/Cube.prefab");

            // 加载子资源（如 使用 Multiple 模式的 Sprite 图片）
            Sprite sprite = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12");

            // 强制远程加载模式，需在 F5 打包工具配置远程资源地址
            Sprite remoteSprite = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12",
                AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);


            /*========== 2. 异步加载 ==========*/

            // 回调方式
            FF8.Asset.LoadAsync<GameObject>("Cube", (go) => { Instantiate(go); });

            // 协程方式
            yield return FF8.Asset.LoadAsync<GameObject>("Cube");

            // 或获取加载器控制
            BaseLoader loader = FF8.Asset.LoadAsync<GameObject>("Cube");
            yield return loader;
            GameObject result = loader.GetAssetObject<GameObject>();

            // async/await 方式（WebGL 兼容）
            // await FF8.Asset.LoadAsync<GameObject>("Cube");


            /*========== 3. 批量加载 ==========*/

            // 同步加载文件夹
            FF8.Asset.LoadDir("UI/Prefabs");

            // 异步加载文件夹 - 回调方式
            FF8.Asset.LoadDirAsync("UI/Prefabs", () => { LogF8.Log("所有UI资源加载完成"); });

            // 异步加载文件夹 - 协程方式
            BaseDirLoader dirLoader = FF8.Asset.LoadDirAsync("UI/Prefabs");
            yield return dirLoader;

            // 遍历加载进度
            foreach (var progress in FF8.Asset.LoadDirAsyncCoroutine("UI/Prefabs"))
            {
                LogF8.Log($"加载进度: {progress}");
                yield return progress;
            }

            // async/await 方式（WebGL 兼容）
            // await FF8.Asset.LoadDirAsync("UI/Prefabs");
            
            // 加载此资源的全部资产
            FF8.Asset.LoadAll("Cube");
            BaseLoader loaderAll = FF8.Asset.LoadAllAsync("Cube");
            
            // 加载此资源的全部子资产
            FF8.Asset.LoadSub("Atlas");
            BaseLoader loaderSub = FF8.Asset.LoadSubAsync("Atlas");

            
            /*========== 4. 场景加载 ==========*/

            // 同步加载场景
            FF8.Asset.LoadScene("MainScene");

            // 异步加载场景
            SceneLoader sceneLoader = FF8.Asset.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            yield return sceneLoader;

            // 手动控制场景激活
            SceneLoader sceneLoader2 = FF8.Asset.LoadSceneAsync("MainScene", new LoadSceneParameters(LoadSceneMode.Single),
                allowSceneActivation: false);
            yield return new WaitForSeconds(2);
            sceneLoader2.AllowSceneActivation();


            /*========== 5. 资源管理 ==========*/

            // 获取已加载资源
            GameObject cachedCube = FF8.Asset.GetAssetObject<GameObject>("Cube");

            // 获取所有子资源
            Dictionary<string, Object> allAssets = FF8.Asset.GetAllAssetObject("Atlas");
            Dictionary<string, Sprite> allSprites = FF8.Asset.GetAllAssetObject<Sprite>("Atlas");

            // 获取加载进度
            float assetProgress = FF8.Asset.GetLoadProgress("Cube"); // 单个资源
            float totalProgress = FF8.Asset.GetLoadProgress(); // 所有资源

            // 资源卸载
            FF8.Asset.Unload("Cube", false); // 保留依赖
            FF8.Asset.Unload("Cube", true); // 完全卸载

            // 异步卸载
            FF8.Asset.UnloadAsync("Cube", false, () => { LogF8.Log("资源卸载完成"); });
            
            
            /*========== 6. 注意：常见问题 ==========*/
            
            // 编辑器下加载不同平台的AB包（Android平台，iOS平台，WebGL平台），Shader会变紫色，Scene会加载失败，音频加载失败等（解决方案：启用编辑器模式）
            
            // 加载场景，别忘了加载天空盒材质，不然会变紫色，并且不能加载Resources目录中的场景（需要手动放入Build Setting处）
            
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
