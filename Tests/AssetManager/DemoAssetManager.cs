using System.Collections;
using F8Framework.Core;
using UnityEngine;

public class DemoAssetManager : MonoBehaviour
{
    IEnumerator Start()
    {
        /*----------所有加载均会自动判断是Resources资产还是AssetBundle资产----------*/
        
        /*----------同步加载----------*/
        GameObject go = FF8.Asset.Load<GameObject>("Cube");
        GameObject go2 = FF8.Asset.Load("Cube")as GameObject;
        GameObject go3 = FF8.Asset.Load("Cube", typeof(GameObject))as GameObject;
        //指定加载模式REMOTE_ASSET_BUNDLE，加载远程AssetBundle资产，需要配置REMOTE_ADDRESS = "http://127.0.0.1:6789/remote"
        GameObject go5 = FF8.Asset.Load<GameObject>("Cube", AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
        //加载文件夹内资产
        FF8.Asset.LoadDir("NewFolder");
        
        /*----------异步加载----------*/
        FF8.Asset.LoadAsync<GameObject>("Cube", (go) =>
        {
            GameObject goo = Instantiate(go);
        });
        //协程
        var load = FF8.Asset.LoadAsyncCoroutine<GameObject>("Cube");
        yield return load;
        //加载文件夹内资产
        FF8.Asset.LoadDirAsync("NewFolder", () =>
        {
            
        });
        //协程
        var loadDir = FF8.Asset.LoadDirAsyncCoroutine("NewFolder");
        yield return loadDir;
        
        
        /*----------其他功能----------*/
        //获取加载进度
        float loadProgress = FF8.Asset.GetLoadProgress("Cube");
        //获取所有加载器的进度
        float loadProgress2 = FF8.Asset.GetLoadProgress();
        //同步卸载资产
        FF8.Asset.Unload("Cube", false);//根据AbPath卸载资产，如果设置为 true，将卸载目标依赖的所有资源，
        //异步卸载资产
        FF8.Asset.UnloadAsync("Cube", false, () =>
        {
            //卸载资产完成
        });
        
        //编辑器模式，无需打包AB
        FF8.Asset.IsEditorMode = true;
    }
}
