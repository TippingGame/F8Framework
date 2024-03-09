using UnityEngine;

namespace F8Framework.Core
{
    public class DelegateComponent : MonoBehaviour
    {
        public ViewParams ViewParams;
        // 窗口添加
        public void Add()
        {
            // 触发窗口组件上添加到父节点后的事件
            ApplyComponentsFunction("Added", ViewParams.Params, ViewParams.UIid);
            if (ViewParams.Callbacks != null && ViewParams.Callbacks.OnAdded != null)
            {
                ViewParams.Callbacks.OnAdded(ViewParams.Params, ViewParams.UIid);
            }
        }

        // 删除节点，该方法只能调用一次，将会触发OnBeforeRemoved回调
        public void Remove(bool isDestroy)
        {
            if (ViewParams.Valid)
            {
                // 触发窗口组件上移除之前的事件
                ApplyComponentsFunction("BeforeRemove");

                // 通知外部对象窗口组件上移除之前的事件（关闭窗口前的关闭动画处理）
                if (ViewParams.Callbacks != null && ViewParams.Callbacks.OnBeforeRemove != null)
                {
                    ViewParams.Callbacks.OnBeforeRemove(() => Removed(ViewParams, isDestroy));
                }
                else
                {
                    Removed(ViewParams, isDestroy);
                }
            }
        }

        // 窗口组件中触发移除事件与释放窗口对象
        private void Removed(ViewParams viewParams, bool isDestroy)
        {
            viewParams.Valid = false;
            
            if (viewParams.Callbacks != null && viewParams.Callbacks.OnRemoved != null)
            {
                viewParams.Callbacks.OnRemoved(viewParams.Params, viewParams.UIid);
            }

            if (isDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                }
                gameObject.transform.SetParent(null, false);
            }
        }

        private void OnDestroy()
        {
            // 触发窗口组件上窗口移除之后的事件
            ApplyComponentsFunction("Removed");

            ViewParams = null;
        }

        private void ApplyComponentsFunction(string functionName, object parameters = null, int uiId = 0)
        {
            foreach (var component in gameObject.GetComponents<BaseView>())
            {
                var methodInfo = component.GetType().GetMethod(functionName);
                if (methodInfo == null)
                {
                    continue;
                }

                if (functionName == "Added")
                {
                    if (parameters == null)
                    {
                        methodInfo.Invoke(component, new object[] { uiId, null });
                    }
                    else
                    {
                        methodInfo.Invoke(component, new object[] { parameters, uiId });
                    }
                }
                else
                {
                    methodInfo.Invoke(component, null);
                }
            }
        }
    }
}
