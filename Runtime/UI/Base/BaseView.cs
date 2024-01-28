using UnityEngine;

namespace F8Framework.Core
{
    public class BaseView : MonoBehaviour
    {
        public enum WindowState
        {
            Awake,
            Animating,
            Start,
            Closed
        }

        public object[] Args;
        public int UIid;
        public WindowState windowState = WindowState.Closed;

        private void Awake()
        {
            windowState = WindowState.Awake;
            OnViewTweenInit();
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }
        
        public void Added(object[] args, int uiId)
        {
            this.Args = args;
            this.UIid = uiId;
            OnAdded(args, uiId);
        }

        protected virtual void OnAdded(object[] args, int uiId)
        {
        }

        private void Start()
        {
            windowState = WindowState.Animating;
            OnPlayViewTween();
            OnStart();
            windowState = WindowState.Start;
        }

        protected virtual void OnStart()
        {
        }
        
        protected virtual void OnViewTweenInit()
        {
        }
        
        protected virtual void OnPlayViewTween()
        {
            OnViewOpen();
        }

        protected virtual void OnViewOpen()
        {
        }

        public void AddEscBtn()
        {

        }

        public void OnEscBtn()
        {

        }

        public void Close(bool isDestroy = false)
        {
            windowState = WindowState.Closed;
            UIManager.Instance.Close(UIid, isDestroy);
        }

        public void BeforeRemove(object[] args, int uiId)
        {
            OnBeforeRemove(args, uiId);
        }

        protected virtual void OnBeforeRemove(object[] args, int uiId)
        {
        }

        public void Removed(object[] args, int uiId)
        {
            OnRemoved(args, uiId);
        }

        protected virtual void OnRemoved(object[] args, int uiId)
        {
        }
    }
}
