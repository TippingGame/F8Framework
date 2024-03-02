using System;
using UnityEngine;

namespace F8Framework.Core
{
    using RequestCallback = Action<CacheResult>;

    public class CacheRequestOperation : CustomYieldInstruction
    {
        private CacheInfo info;
        private RequestCallback callback;
        private bool waiting;

        public override bool keepWaiting
        {
            get
            {
                if (info == null)
                {
                    return false;
                }

                if (waiting == true)
                {
                    if (info.state == CacheInfo.State.REQUEST ||
                        info.IsRequest() == true)
                    {
                        return true;
                    }
                }

                return false;
            }
        }


        public CacheRequestOperation(CacheInfo info, bool waiting = false, RequestCallback callback = null)
        {
            this.info = info;
            this.callback = callback;
            this.waiting = waiting;

            if (info != null)
            {
                info.lastAccess = DateTime.UtcNow.Ticks;
                CacheStorageImplement.SetDirty();
            }
        }

        public void Cancel()
        {
            if (info != null &&
                callback != null)
            {
                info.ReleaseCallback(callback);

                info = null;
                callback = null;
                waiting = false;
            }
        }

        public static implicit operator CacheInfo(CacheRequestOperation data)
        {
            return data.info;
        }
    }
}