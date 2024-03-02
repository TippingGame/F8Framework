using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoReferencePool : MonoBehaviour
    {
        // 使用IReference接口
        public class AssetInfo : IReference
        {
            public void Clear()
            {

            }
        }

        void Start()
        {
            // 添加入池50个数据
            ReferencePool.Add<AssetInfo>(50);
            // 取出
            AssetInfo assetInfo = ReferencePool.Acquire<AssetInfo>();

            // 回收
            ReferencePool.Release(assetInfo);
            // 清空
            ReferencePool.RemoveAll(typeof(AssetInfo));
        }
    }
}
