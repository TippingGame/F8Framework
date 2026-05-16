using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    public class BaseView : ComponentBind
    {
        public int UIid;
        public string Guid;
        public object[] Args;
        private Sequence _viewOpenSequence;
        private Sequence _viewCloseSequence;
        
        // 消息事件追踪，自动卸载
        private EventDispatcher _eventDispatcher;
        public EventDispatcher EventDispatcher => _eventDispatcher ??= new EventDispatcher();
        
        // 资源加载追踪，自动卸载
        private AssetLoadTracker _assetLoadTracker;
        public AssetLoadTracker AssetLoadTracker => _assetLoadTracker ??= new AssetLoadTracker();
        
        // 计时器追踪，自动卸载
        private TimerTracker _timerTracker;
        public TimerTracker TimerTracker => _timerTracker ??= new TimerTracker();

        private void Awake()
        {
            OnAwake();
            OnAddUIComponentListener();
        }

        protected virtual void OnAwake()
        {
        }
        
        protected virtual void OnAddUIComponentListener()
        {
        }

        public void Added(int uiId, string guid, object[] args = null)
        {
            CancelViewOpenSequence();
            CancelViewCloseSequence();
            this.Args = args;
            this.UIid = uiId;
            this.Guid = guid;
            OnAdded(uiId, args);
            OnViewTweenInit();
            OnPlayViewTween();
            BindOpenSequenceComplete();
        }

        protected virtual void OnAdded(int uiId, object[] args = null)
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
            OnViewOpen();
        }

        protected virtual void OnViewOpen()
        {
        }

        protected virtual void OnPlayViewCloseTween()
        {
        }

        protected Sequence ViewOpenSequence => _viewOpenSequence ??= Tween.Instance?.GetSequence();

        protected Sequence ViewCloseSequence => _viewCloseSequence ??= Tween.Instance?.GetSequence();

        protected virtual void ButtonClick(UIBehaviour ui)
        {
        }
        
        protected virtual void ValueChange<T>(UIBehaviour ui, T value)
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
            UIManager.Instance.Close(this.UIid, isDestroy, this.Guid);
        }

        public void BeforeRemove()
        {
            CancelViewOpenSequence();
            if (_eventDispatcher != null) {
                _eventDispatcher.Clear();
                _eventDispatcher = null;
            }
            if (_timerTracker != null)
            {
                _timerTracker.Clear();
                _timerTracker = null;
            }
            OnBeforeRemove();
        }

        protected virtual void OnBeforeRemove()
        {
        }

        public void Removed(bool isDestroy)
        {
            CancelViewOpenSequence();
            CancelViewCloseSequence();
            OnRemoved();
            ClearAssetLoadTracker(isDestroy);
        }

        internal bool TryPlayCloseTween(Action onComplete)
        {
            CancelViewOpenSequence();
            CancelViewCloseSequence();
            OnPlayViewCloseTween();

            if (_viewCloseSequence == null)
            {
                return false;
            }

            if (!_viewCloseSequence.HasTweens)
            {
                CancelViewCloseSequence();
                return false;
            }

            _viewCloseSequence.SetOnComplete(() =>
            {
                _viewCloseSequence = null;
                onComplete?.Invoke();
            });
            return true;
        }

        private void BindOpenSequenceComplete()
        {
            if (_viewOpenSequence == null)
            {
                return;
            }

            if (!_viewOpenSequence.HasTweens)
            {
                CancelViewOpenSequence();
                return;
            }

            _viewOpenSequence.SetOnComplete(() =>
            {
                _viewOpenSequence = null;
                OnViewOpen();
            });
        }

        private void CancelViewOpenSequence()
        {
            if (_viewOpenSequence == null)
            {
                return;
            }

            Tween.Instance.KillSequence(_viewOpenSequence);
            _viewOpenSequence = null;
        }

        private void CancelViewCloseSequence()
        {
            if (_viewCloseSequence == null)
            {
                return;
            }

            Tween.Instance.KillSequence(_viewCloseSequence);
            _viewCloseSequence = null;
        }

        protected virtual void OnRemoved()
        {
        }
        
        public void AddEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }
            
        public void AddEventListener<T, T1>(T eventName, Action<T1> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }
            
        public void RemoveEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }
            
        public void RemoveEventListener<T, T1>(T eventName, Action<T1> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }
            
        public void DispatchEvent<T>(T eventName) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName);
        }

        public Coroutine DispatchEventAsync<T>(T eventName) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName);
        }
            
        public void DispatchEvent<T, T1>(T eventName, T1 arg1) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1);
        }

        public Coroutine DispatchEventAsync<T, T1>(T eventName, T1 arg1) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1);
        }

        public void DispatchEvent<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2);
        }

        public Coroutine DispatchEventAsync<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2);
        }

        public void DispatchEvent<T, T1, T2, T3>(T eventName, T1 arg1, T2 arg2, T3 arg3) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3>(T eventName, T1 arg1, T2 arg2, T3 arg3) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3);
        }

        public void DispatchEvent<T, T1, T2, T3, T4>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3, T4>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3, arg4);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3, T4, T5>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3, arg4, arg5);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3, T4, T5, T6>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        
        public void AddEventListener(int eventId, Action listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }
            
        public void AddEventListener<T1>(int eventId, Action<T1> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2>(int eventId, Action<T1, T2> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }
            
        public void RemoveEventListener(int eventId, Action listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }
            
        public void RemoveEventListener<T1>(int eventId, Action<T1> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2>(int eventId, Action<T1, T2> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }
            
        public void DispatchEvent(int eventId)
        {
            EventDispatcher.DispatchEvent(eventId);
        }

        public Coroutine DispatchEventAsync(int eventId)
        {
            return EventDispatcher.DispatchEventAsync(eventId);
        }
            
        public void DispatchEvent<T1>(int eventId, T1 arg1)
        {
            EventDispatcher.DispatchEvent(eventId, arg1);
        }

        public Coroutine DispatchEventAsync<T1>(int eventId, T1 arg1)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1);
        }

        public void DispatchEvent<T1, T2>(int eventId, T1 arg1, T2 arg2)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2);
        }

        public Coroutine DispatchEventAsync<T1, T2>(int eventId, T1 arg1, T2 arg2)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2);
        }

        public void DispatchEvent<T1, T2, T3>(int eventId, T1 arg1, T2 arg2, T3 arg3)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3>(int eventId, T1 arg1, T2 arg2, T3 arg3)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3);
        }

        public void DispatchEvent<T1, T2, T3, T4>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3, T4>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3, arg4);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3, T4, T5>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3, arg4, arg5);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3, T4, T5, T6>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6, T7>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3, T4, T5, T6, T7>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        
        /// <summary>
        /// 删除此事件所有监听（慎用）
        /// </summary>
        public void RemoveEventListener<T>(T eventName)
        {
            int tempName = (int)(object)eventName;
            EventDispatcher.RemoveEventListener(tempName);
        }
        
        /// <summary>
        /// 删除此事件所有监听（慎用）
        /// </summary>
        public void RemoveEventListener(int eventId)
        {
            EventDispatcher.RemoveEventListener(eventId);
        }
        
        public T LoadAsset<T>(string assetName, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where T : Object
        {
            return AssetLoadTracker.Load<T>(assetName, subAssetName, mode);
        }
        
        public Object LoadAsset(string assetName, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.Load(assetName, subAssetName, mode);
        }
        
        public Object LoadAsset(string assetName, Type assetType, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.Load(assetName, assetType, subAssetName, mode);
        }
        
        public BaseLoader LoadAssetAsync<T>(string assetName, OnAssetObject<T> callback = null,
            string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where T : Object
        {
            return AssetLoadTracker.LoadAsync(assetName, callback, subAssetName, mode);
        }
        
        public BaseLoader LoadAssetAsync(string assetName, OnAssetObject<Object> callback = null,
            string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.LoadAsync(assetName, callback, subAssetName, mode);
        }
        
        public BaseLoader LoadAssetAsync(string assetName, Type assetType, OnAssetObject<Object> callback = null,
            string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.LoadAsync(assetName, assetType, callback, subAssetName, mode);
        }
        
        public Dictionary<string, TObject> LoadAllAsset<TObject>(string assetName,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where TObject : Object
        {
            return AssetLoadTracker.LoadAll<TObject>(assetName, mode);
        }
        
        public Dictionary<string, Object> LoadAllAsset(string assetName,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.LoadAll(assetName, mode);
        }
        
        public BaseLoader LoadAllAssetAsync<TObject>(string assetName, OnAllAssetObject<TObject> callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where TObject : Object
        {
            return AssetLoadTracker.LoadAllAsync(assetName, callback, mode);
        }
        
        public BaseLoader LoadAllAssetAsync(string assetName, OnAllAssetObject<Object> callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.LoadAllAsync(assetName, callback, mode);
        }
        
        public Dictionary<string, TObject> LoadSubAsset<TObject>(string assetName, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where TObject : Object
        {
            return AssetLoadTracker.LoadSub<TObject>(assetName, subAssetName, mode);
        }
        
        public Dictionary<string, Object> LoadSubAsset(string assetName, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.LoadSub(assetName, subAssetName, mode);
        }
        
        public BaseLoader LoadSubAssetAsync<TObject>(string assetName, string subAssetName = null,
            OnAllAssetObject<TObject> callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where TObject : Object
        {
            return AssetLoadTracker.LoadSubAsync(assetName, subAssetName, callback, mode);
        }
        
        public BaseLoader LoadSubAssetAsync(string assetName, string subAssetName = null,
            OnAllAssetObject<Object> callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.LoadSubAsync(assetName, subAssetName, callback, mode);
        }
        
        public BaseDirLoader LoadDirAsset(string assetName,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.LoadDir(assetName, mode);
        }
        
        public BaseDirLoader LoadDirAssetAsync(string assetName, Action callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetLoadTracker.LoadDirAsync(assetName, callback, mode);
        }
        
        public void UnloadAsset(string assetName, bool unloadAllLoadedObjects = false)
        {
            _assetLoadTracker?.Release(assetName, unloadAllLoadedObjects);
        }
        
        public void ClearAssetLoadTracker(bool unloadAllLoadedObjects = false)
        {
            if (_assetLoadTracker == null)
                return;
            
            _assetLoadTracker.ReleaseAll(unloadAllLoadedObjects);
            _assetLoadTracker = null;
        }
        
        public int AddTimer(float duration, Action onComplete, bool ignoreTimeScale = false)
        {
            return TimerTracker.AddTimer(this, duration, onComplete, ignoreTimeScale);
        }
        
        public int AddTimer(float duration, bool isLoop, Action onComplete = null, bool ignoreTimeScale = false)
        {
            return TimerTracker.AddTimer(this, duration, isLoop, onComplete, ignoreTimeScale);
        }
        
        public int AddTimer(float step = 1f, int field = 0, Action onSecond = null,
            Action onComplete = null, bool ignoreTimeScale = false)
        {
            return TimerTracker.AddTimer(this, step, field, onSecond, onComplete, ignoreTimeScale);
        }
        
        public int AddTimer(float step, float delay, int field = 0, Action onSecond = null,
            Action onComplete = null, bool ignoreTimeScale = false)
        {
            return TimerTracker.AddTimer(this, step, delay, field, onSecond, onComplete, ignoreTimeScale);
        }
        
        public int AddTimerFrame(float duration, Action onComplete, bool ignoreTimeScale = false)
        {
            return TimerTracker.AddTimerFrame(this, duration, onComplete, ignoreTimeScale);
        }
        
        public int AddTimerFrame(float duration, bool isLoop, Action onComplete, bool ignoreTimeScale = false)
        {
            return TimerTracker.AddTimerFrame(this, duration, isLoop, onComplete, ignoreTimeScale);
        }
        
        public int AddTimerFrame(float step = 1f, int field = 0, Action onFrame = null,
            Action onComplete = null, bool ignoreTimeScale = false)
        {
            return TimerTracker.AddTimerFrame(this, step, field, onFrame, onComplete, ignoreTimeScale);
        }
        
        public int AddTimerFrame(float stepFrame, float delayFrame, int field = 0, Action onFrame = null,
            Action onComplete = null, bool ignoreTimeScale = false)
        {
            return TimerTracker.AddTimerFrame(this, stepFrame, delayFrame, field, onFrame, onComplete, ignoreTimeScale);
        }
        
        public void RemoveTimer(int id)
        {
            _timerTracker?.RemoveTimer(id);
        }
        
        public void ClearTimerTracker()
        {
            if (_timerTracker == null)
                return;
            
            _timerTracker.Clear();
            _timerTracker = null;
        }
    }
}
