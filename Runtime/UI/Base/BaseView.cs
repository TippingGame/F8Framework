using UnityEngine;

namespace F8Framework.Core
{
    public class BaseView : ComponentBind
    {
        public enum WindowState
        {
            Awake,
            Animating,
            Ready,
            Closed
        }
        
        public object[] Args;
        public int UIid;
        public WindowState windowState = WindowState.Closed;

        private void Awake()
        {
            windowState = WindowState.Awake;
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
            windowState = WindowState.Animating;
            OnViewTweenInit();
            OnPlayViewTween();
        }

        protected virtual void OnAdded(object[] args, int uiId)
        {
        }

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
        }
        
        protected virtual void OnViewTweenInit()
        {
        }
        
        protected virtual void OnPlayViewTween()
        {
            windowState = WindowState.Ready;
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

        public void BeforeRemove()
        {
            OnBeforeRemove();
        }

        protected virtual void OnBeforeRemove()
        {
        }

        public void Removed()
        {
            OnRemoved();
        }

        protected virtual void OnRemoved()
        {
        }
    }
}
