using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    public class EditorLoader : BaseLoader
    {
        private bool isLoadSuccess = false;
        public override bool LoaderSuccess => isLoadSuccess == true;

        public Object Asset = null;
        
        public EditorLoader(Object asset)
        {
            isLoadSuccess = true;
            Asset = asset;
        }

        public override T GetAssetObject<T>(string subAssetName = null)
        {
            return Asset as T;
        }

        public override Object GetAssetObject(string subAssetName = null)
        {
            return Asset;
        }
    }
}
