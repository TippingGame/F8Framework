using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public static partial class ReferencePool
    {
        private sealed class ReferenceCollection
        {
            private readonly Queue<IReference> _mReferences;
            private readonly Type _mReferenceType;
            private int _mUsingReferenceCount;
            private int _mAcquireReferenceCount;
            private int _mReleaseReferenceCount;
            private int _mAddReferenceCount;
            private int _mRemoveReferenceCount;

            public ReferenceCollection(Type referenceType)
            {
                _mReferences = new Queue<IReference>();
                _mReferenceType = referenceType;
                _mUsingReferenceCount = 0;
                _mAcquireReferenceCount = 0;
                _mReleaseReferenceCount = 0;
                _mAddReferenceCount = 0;
                _mRemoveReferenceCount = 0;
            }

            public Type ReferenceType
            {
                get
                {
                    return _mReferenceType;
                }
            }

            public int UnusedReferenceCount
            {
                get
                {
                    return _mReferences.Count;
                }
            }

            public int UsingReferenceCount
            {
                get
                {
                    return _mUsingReferenceCount;
                }
            }

            public int AcquireReferenceCount
            {
                get
                {
                    return _mAcquireReferenceCount;
                }
            }

            public int ReleaseReferenceCount
            {
                get
                {
                    return _mReleaseReferenceCount;
                }
            }

            public int AddReferenceCount
            {
                get
                {
                    return _mAddReferenceCount;
                }
            }

            public int RemoveReferenceCount
            {
                get
                {
                    return _mRemoveReferenceCount;
                }
            }

            public T Acquire<T>() where T : class, IReference, new()
            {
                if (typeof(T) != _mReferenceType)
                {
                    LogF8.LogError("Type is invalid.");
                    return null;
                }

                _mUsingReferenceCount++;
                _mAcquireReferenceCount++;
                lock (_mReferences)
                {
                    if (_mReferences.Count > 0)
                    {
                        return (T)_mReferences.Dequeue();
                    }
                }

                _mAddReferenceCount++;
                return new T();
            }

            public IReference Acquire()
            {
                _mUsingReferenceCount++;
                _mAcquireReferenceCount++;
                lock (_mReferences)
                {
                    if (_mReferences.Count > 0)
                    {
                        return _mReferences.Dequeue();
                    }
                }

                _mAddReferenceCount++;
                return (IReference)Activator.CreateInstance(_mReferenceType);
            }

            public void Release(IReference reference)
            {
                reference.Clear();
                lock (_mReferences)
                {
                    if (_mEnableStrictCheck && _mReferences.Contains(reference))
                    {
                        LogF8.LogError("The reference has been released.");
                        return;
                    }

                    _mReferences.Enqueue(reference);
                }

                _mReleaseReferenceCount++;
                _mUsingReferenceCount--;
            }

            public void Add<T>(int count) where T : class, IReference, new()
            {
                if (typeof(T) != _mReferenceType)
                {
                    LogF8.LogError("Type is invalid.");
                    return;
                }

                lock (_mReferences)
                {
                    _mAddReferenceCount += count;
                    while (count-- > 0)
                    {
                        _mReferences.Enqueue(new T());
                    }
                }
            }

            public void Add(int count)
            {
                lock (_mReferences)
                {
                    _mAddReferenceCount += count;
                    while (count-- > 0)
                    {
                        _mReferences.Enqueue((IReference)Activator.CreateInstance(_mReferenceType));
                    }
                }
            }

            public void Remove(int count)
            {
                lock (_mReferences)
                {
                    if (count > _mReferences.Count)
                    {
                        count = _mReferences.Count;
                    }

                    _mRemoveReferenceCount += count;
                    while (count-- > 0)
                    {
                        _mReferences.Dequeue();
                    }
                }
            }

            public void RemoveAll()
            {
                lock (_mReferences)
                {
                    _mRemoveReferenceCount += _mReferences.Count;
                    _mReferences.Clear();
                }
            }
        }
    }
}
