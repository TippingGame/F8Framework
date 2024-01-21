using UnityEngine;

namespace F8Framework.Core
{
    public class BaseView : MonoBehaviour
    {
        public enum WindowState
        {
            Opening,
            Animating,
            Opened,
            Closed
        }

        public object[] Args;
        public string Uuid;

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        public void Added(object[] args, string uuid)
        {
            this.Args = args;
            this.Uuid = uuid;
            OnAdded(args, uuid);
        }

        protected virtual void OnAdded(object[] args, string uuid)
        {
        }

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
        }

        private void ViewOpen()
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

        public void Close(bool isDestroy)
        {

        }

        public void BeforeRemove(object[] args, string uuid)
        {
            OnBeforeRemove(args, uuid);
        }

        protected virtual void OnBeforeRemove(object[] args, string uuid)
        {
        }

        public void Removed(object[] args, string uuid)
        {
            OnRemoved(args, uuid);
        }

        protected virtual void OnRemoved(object[] args, string uuid)
        {
        }
    }
}
