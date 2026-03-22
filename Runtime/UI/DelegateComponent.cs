using UnityEngine;

namespace F8Framework.Core
{
    public class DelegateComponent : MonoBehaviour
    {
        public ViewParams ViewParams;
        private bool _isRemoving;

        // 窗口添加
        public void Add()
        {
            _isRemoving = false;
            // 触发窗口组件上添加到父节点后的事件
            ViewParams.BaseView?.Added(ViewParams.UIid, ViewParams.Guid, ViewParams.Params);
            
            if (ViewParams.Callbacks != null && ViewParams.Callbacks.OnAdded != null)
            {
                ViewParams.Callbacks.OnAdded(ViewParams.Params, ViewParams.UIid);
            }
        }

        // 删除节点，该方法只能调用一次，将会触发OnBeforeRemoved回调
        public void Remove(bool isDestroy)
        {
            if (ViewParams != null && ViewParams.Valid && !_isRemoving)
            {
                _isRemoving = true;
                if (ViewParams.BaseView != null && ViewParams.BaseView.TryPlayCloseTween(() => ContinueRemove(isDestroy)))
                {
                    return;
                }

                ContinueRemove(isDestroy);
            }
        }

        private void ContinueRemove(bool isDestroy)
        {
            if (ViewParams == null)
            {
                return;
            }

            // 触发窗口组件上移除之前的事件
            ViewParams.BaseView?.BeforeRemove();

            // 通知外部对象窗口组件上移除之前的事件（关闭窗口前的关闭动画处理）
            if (ViewParams.Callbacks != null && ViewParams.Callbacks.OnBeforeRemove != null)
            {
                ViewParams.Callbacks.OnBeforeRemove();
                Removed(ViewParams, isDestroy);
            }
            else
            {
                Removed(ViewParams, isDestroy);
            }
        }

        // 窗口组件中触发移除事件与释放窗口对象
        private void Removed(ViewParams viewParams, bool isDestroy)
        {
            _isRemoving = false;
            viewParams.Valid = false;
            
            if (viewParams.Callbacks != null && viewParams.Callbacks.OnRemoved != null)
            {
                viewParams.Callbacks.OnRemoved(viewParams.Params, viewParams.UIid);
            }
            
            ViewParams?.BaseView?.Removed();
            
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
            
            UIManager.Instance.CurrentUIs.Remove(viewParams);
        }

        private void OnDestroy()
        {
            ViewParams = null;
        }
    }
}
