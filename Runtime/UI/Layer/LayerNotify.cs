using UnityEngine;

namespace F8Framework.Core
{
    public class LayerNotify : LayerUI
    {
        public void Show(int uiId, UIConfig config, string content)
        {
            var prefabPath = config.AssetName;
            ViewParams viewParams = new ViewParams
            {
                UIid = uiId,
                Uuid = prefabPath, // 暂时和prefabPath相同
                PrefabPath = prefabPath,
                Params = new object[] { content },
                Callbacks = new UICallbacks(),
                Valid = true
            };

            uiViews[viewParams.Uuid] = viewParams;
            Load(viewParams);
        }

        protected new void Load(ViewParams viewParams)
        {
            AssetManager.Instance.LoadAsync<GameObject>(viewParams.PrefabPath, (res) =>
            {
                AssetManager.Instance.Unload(viewParams.PrefabPath, false);
                
                GameObject childObject = Instantiate(res);
                viewParams.Go = childObject;
            
                DelegateComponent comp = childObject.AddComponent<DelegateComponent>();
                viewParams.DelegateComponent = comp;
                viewParams.BaseView = childObject.GetComponent<BaseView>();
                comp.ViewParams = viewParams;
            
                CreateNode(viewParams);
            });
        }

        protected new void CreateNode(ViewParams viewParams)
        {
            viewParams.Valid = true;

            var comp = viewParams.DelegateComponent;
            comp.Add();
            viewParams.Go.transform.SetParent(gameObject.transform, false);
            viewParams.Go.transform.localPosition = Vector3.zero;
        }
    }
}