using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    public class EditorLoader : BaseLoader
    {
        public bool isLoadSuccess = false;
        public override bool LoaderSuccess => isLoadSuccess == true;
        public override float Progress => isLoadSuccess == true ? 1f : 0f;
        public Object Asset = null;
        public Dictionary<string, Object> AllAsset = null;
        
        public EditorLoader(Object asset = null, Dictionary<string, Object> allAsset = null)
        {
            Asset = asset;
            AllAsset = allAsset;
        }
        
        public void AssetDatabaseLoadSuccess()
        {
            isLoadSuccess = true;
            base.OnComplete();
        }
        
        public override T GetAssetObject<T>(string subAssetName = null)
        {
            return Asset as T;
        }

        public override Object GetAssetObject(string subAssetName = null)
        {
            return Asset;
        }
        
        public override Dictionary<string, TObject> GetAllAssetObject<TObject>()
        {
            Dictionary<string, TObject> allAsset = new Dictionary<string, TObject>();
            foreach (var item in AllAsset)
            {
                if (item.Value is TObject value)
                {
                    allAsset[item.Key] = value;
                }
            }
            return allAsset;
        }
        
        public override Dictionary<string, Object> GetAllAssetObject()
        {
            return AllAsset;
        }
    }
}
