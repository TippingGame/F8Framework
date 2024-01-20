using System;

namespace F8Framework.Core
{
    /// <summary>
    /// 引用池信息。
    /// </summary>
    public struct ReferencePoolInfo
    {
        private readonly Type _mType;
        private readonly int _mUnusedReferenceCount;
        private readonly int _mUsingReferenceCount;
        private readonly int _mAcquireReferenceCount;
        private readonly int _mReleaseReferenceCount;
        private readonly int _mAddReferenceCount;
        private readonly int _mRemoveReferenceCount;

        /// <summary>
        /// 初始化引用池信息的新实例。
        /// </summary>
        /// <param name="type">引用池类型。</param>
        /// <param name="unusedReferenceCount">未使用引用数量。</param>
        /// <param name="usingReferenceCount">正在使用引用数量。</param>
        /// <param name="acquireReferenceCount">获取引用数量。</param>
        /// <param name="releaseReferenceCount">归还引用数量。</param>
        /// <param name="addReferenceCount">增加引用数量。</param>
        /// <param name="removeReferenceCount">移除引用数量。</param>
        public ReferencePoolInfo(Type type, int unusedReferenceCount, int usingReferenceCount, int acquireReferenceCount, int releaseReferenceCount, int addReferenceCount, int removeReferenceCount)
        {
            _mType = type;
            _mUnusedReferenceCount = unusedReferenceCount;
            _mUsingReferenceCount = usingReferenceCount;
            _mAcquireReferenceCount = acquireReferenceCount;
            _mReleaseReferenceCount = releaseReferenceCount;
            _mAddReferenceCount = addReferenceCount;
            _mRemoveReferenceCount = removeReferenceCount;
        }

        /// <summary>
        /// 获取引用池类型。
        /// </summary>
        public Type Type
        {
            get
            {
                return _mType;
            }
        }

        /// <summary>
        /// 获取未使用引用数量。
        /// </summary>
        public int UnusedReferenceCount
        {
            get
            {
                return _mUnusedReferenceCount;
            }
        }

        /// <summary>
        /// 获取正在使用引用数量。
        /// </summary>
        public int UsingReferenceCount
        {
            get
            {
                return _mUsingReferenceCount;
            }
        }

        /// <summary>
        /// 获取获取引用数量。
        /// </summary>
        public int AcquireReferenceCount
        {
            get
            {
                return _mAcquireReferenceCount;
            }
        }

        /// <summary>
        /// 获取归还引用数量。
        /// </summary>
        public int ReleaseReferenceCount
        {
            get
            {
                return _mReleaseReferenceCount;
            }
        }

        /// <summary>
        /// 获取增加引用数量。
        /// </summary>
        public int AddReferenceCount
        {
            get
            {
                return _mAddReferenceCount;
            }
        }

        /// <summary>
        /// 获取移除引用数量。
        /// </summary>
        public int RemoveReferenceCount
        {
            get
            {
                return _mRemoveReferenceCount;
            }
        }
    }
}
