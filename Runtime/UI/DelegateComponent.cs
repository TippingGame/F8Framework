using System;
using UnityEngine;

namespace F8Framework.Core
{
    public class DelegateComponent : MonoBehaviour
    {
        public ViewParams ViewParams;
        private bool _isRemoving;
        private int _removeVersion;

        // 窗口添加
        public void Add()
        {
            _removeVersion++;
            _isRemoving = false;
            ViewParams.State = ViewState.Active;
            // 触发窗口组件上添加到父节点后的事件
            ViewParams.BaseView?.Added(ViewParams.UIid, ViewParams.Guid, ViewParams.Params);
            
            if (ViewParams.Callbacks != null && ViewParams.Callbacks.OnAdded != null)
            {
                ViewParams.Callbacks.OnAdded(ViewParams.Params, ViewParams.UIid);
            }
        }

        // 删除节点，该方法只能调用一次，将会触发OnBeforeRemoved回调
        public bool Remove(bool isDestroy, Action<ViewParams> onComplete = null)
        {
            if (ViewParams == null || !ViewParams.Valid)
            {
                return false;
            }

            ViewParams.DestroyOnClose |= isDestroy;
            ViewParams.State = ViewState.Closing;

            if (_isRemoving)
            {
                return true;
            }

            _isRemoving = true;
            int removeVersion = ++_removeVersion;
            if (ViewParams.BaseView != null && ViewParams.BaseView.TryPlayCloseTween(() => ContinueRemove(removeVersion, onComplete)))
            {
                return true;
            }

            ContinueRemove(removeVersion, onComplete);
            return true;
        }

        private void ContinueRemove(int removeVersion, Action<ViewParams> onComplete)
        {
            if (ViewParams == null)
            {
                _isRemoving = false;
                return;
            }

            if (!_isRemoving || removeVersion != _removeVersion)
            {
                return;
            }

            var viewParams = ViewParams;
            // 触发窗口组件上移除之前的事件
            viewParams.BaseView?.BeforeRemove();

            // 通知外部对象窗口组件上移除之前的事件（关闭窗口前的关闭动画处理）
            if (viewParams.Callbacks != null && viewParams.Callbacks.OnBeforeRemove != null)
            {
                viewParams.Callbacks.OnBeforeRemove();
                Removed(viewParams, removeVersion, onComplete);
            }
            else
            {
                Removed(viewParams, removeVersion, onComplete);
            }
        }

        // 窗口组件中触发移除事件与释放窗口对象
        private void Removed(ViewParams viewParams, int removeVersion, Action<ViewParams> onComplete)
        {
            if (ViewParams != viewParams || !_isRemoving || removeVersion != _removeVersion)
            {
                return;
            }

            _isRemoving = false;
            viewParams.State = ViewState.None;
            
            if (viewParams.Callbacks != null && viewParams.Callbacks.OnRemoved != null)
            {
                viewParams.Callbacks.OnRemoved(viewParams.Params, viewParams.UIid);
            }

            if (ViewParams != viewParams || removeVersion != _removeVersion)
            {
                return;
            }
            
            bool isDestroy = viewParams.DestroyOnClose;
            ViewParams?.BaseView?.Removed(isDestroy);
            
            if (isDestroy)
            {
                UIManager.Instance.UnloadUIPrefab(viewParams.PrefabPath, true);
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
            onComplete?.Invoke(viewParams);
        }

        private void OnDestroy()
        {
            _isRemoving = false;
            ViewParams = null;
        }
    }
}
