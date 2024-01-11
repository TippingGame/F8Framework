using F8Framework.AssetMap;
using F8Framework.Core;
using UnityEngine;

public class DemoAssetManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //同步加载资产，自动判断是Resources资产还是AssetBundle资产
        GameObject go = AssetManager.Instance.Load<GameObject>("Cube");
        GameObject go2 = AssetManager.Instance.Load("Cube")as GameObject;
        GameObject go3 = AssetManager.Instance.Load("Cube", typeof(GameObject))as GameObject;
        GameObject go4 = Instantiate(go);
        
        //异步加载资产，自动判断是Resources资产还是AssetBundle资产
        AssetManager.Instance.LoadAsync<GameObject>("Cube", (go) =>
        {
            GameObject goo = Instantiate(go);
        });
        
        //同步加载远程AssetBundle资产，需要配置REMOTE_ADDRESS = "http://127.0.0.1:6789/remote"
        GameObject go5 = AssetManager.Instance.Load<GameObject>("Cube", AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
        GameObject goo5 = Instantiate(go5);
        
        //异步加载远程AssetBundle资产
        AssetManager.Instance.LoadAsync<GameObject>("Cube", (go) =>
        {
            GameObject goo = Instantiate(go);
        }, AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
        
        
        //根据AssetName获取Ab映射属性
        AssetBundleMap.Mappings.TryGetValue("Cube", out AssetBundleMap.AssetMapping assetMapping);
        //获取加载进度
        float loadProgress = AssetManager.Instance.GetLoadProgress("Cube");
        //获取所有加载器的进度
        float loadProgress2 = AssetManager.Instance.GetLoadProgress();
        
        
        //同步卸载Resources或者AssetBundle资产
        AssetManager.Instance.Unload("Cube", false);//根据AbPath卸载资产，如果设置为 true，将卸载目标依赖的所有资源，
        //异步卸载AssetBundle资产
        AssetManager.Instance.UnloadAsync("Cube", false, () =>
        {
            //卸载资产完成
        });
        
        //编辑器模式，无需打包AB
        AssetManager.Instance.IsEditorMode = true;
    }
}
