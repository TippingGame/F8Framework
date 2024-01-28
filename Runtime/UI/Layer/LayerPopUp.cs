
namespace F8Framework.Core
{
    public class LayerPopUp : LayerUI
    {
        public new string Add(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            return base.Add(uiId, config, parameters, callbacks);
        }

        public new void Close(string prefabPath, bool isDestroy)
        {
            base.Close(prefabPath, isDestroy);
            SetBlackDisable();
        }

        protected new void RemoveByUuid(string prefabPath, bool isDestroy)
        {
            base.RemoveByUuid(prefabPath, isDestroy);
            SetBlackDisable();
        }

        private void SetBlackDisable()
        {
           
        }
    }
}