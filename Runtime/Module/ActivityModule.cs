using System;
using System.Collections.Generic;
using System.Reflection;

namespace F8Framework.Core
{
    public readonly struct ActivityModuleState : IEquatable<ActivityModuleState>
    {
        public readonly bool IsUnlocked;
        public readonly bool IsVisible;
        public readonly bool IsOpen;

        public ActivityModuleState(bool isUnlocked, bool isVisible, bool isOpen)
        {
            IsUnlocked = isUnlocked;
            IsVisible = isVisible;
            IsOpen = isOpen;
        }

        public bool Equals(ActivityModuleState other)
        {
            return IsUnlocked == other.IsUnlocked && IsVisible == other.IsVisible && IsOpen == other.IsOpen;
        }

        public override bool Equals(object obj)
        {
            return obj is ActivityModuleState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + IsUnlocked.GetHashCode();
                hash = hash * 23 + IsVisible.GetHashCode();
                hash = hash * 23 + IsOpen.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(ActivityModuleState left, ActivityModuleState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ActivityModuleState left, ActivityModuleState right)
        {
            return !left.Equals(right);
        }
    }

    public abstract class ActivityModule
    {
        private static readonly Dictionary<Type, ActivityModule> _activityModules = new Dictionary<Type, ActivityModule>();
        private static readonly List<Type> _activityModuleTypes = new List<Type>();
        private static readonly List<ActivityModule> _tempActivityModules = new List<ActivityModule>();

        private readonly List<Func<bool>> _unlockConditions = new List<Func<bool>>();
        private readonly List<Func<bool>> _visibleConditions = new List<Func<bool>>();
        private readonly List<Func<bool>> _openConditions = new List<Func<bool>>();

        public bool IsInitialized { get; private set; }
        public bool IsUnlocked { get; private set; }
        public bool IsVisible { get; private set; }
        public bool IsOpen { get; private set; }
        public string ModuleName => GetType().Name;
        public ActivityModuleState State => new ActivityModuleState(IsUnlocked, IsVisible, IsOpen);

        public event Action<ActivityModule, ActivityModuleState, ActivityModuleState> StateChanged;

        static ActivityModule()
        {
            DiscoverActivityModules();
        }

        public static IReadOnlyList<Type> GetActivityModuleTypes()
        {
            return _activityModuleTypes;
        }

        public static IReadOnlyDictionary<Type, ActivityModule> GetActivityModules()
        {
            return _activityModules;
        }

        public static T GetActivityModule<T>() where T : ActivityModule, new()
        {
            return (T)GetActivityModule(typeof(T));
        }

        public static ActivityModule GetActivityModule(Type type)
        {
            if (type == null || !type.IsSubclassOf(typeof(ActivityModule)) || type.IsAbstract)
            {
                return null;
            }

            if (_activityModules.TryGetValue(type, out var module))
            {
                return module;
            }

            module = (ActivityModule)Activator.CreateInstance(type);
            _activityModules[type] = module;
            module.Initialize();
            return module;
        }

        public static void EnterGameAllModules()
        {
            _tempActivityModules.Clear();
            _tempActivityModules.AddRange(_activityModules.Values);
            foreach (var module in _tempActivityModules)
            {
                module.OnEnterGame();
            }
            _tempActivityModules.Clear();
        }
        
        public static void QuitGameAllModules()
        {
            _tempActivityModules.Clear();
            _tempActivityModules.AddRange(_activityModules.Values);
            foreach (var module in _tempActivityModules)
            {
                module.OnEnterGame();
            }
            _tempActivityModules.Clear();
        }
        
        public static void RefreshAllModules()
        {
            _tempActivityModules.Clear();
            _tempActivityModules.AddRange(_activityModules.Values);
            foreach (var module in _tempActivityModules)
            {
                module.RefreshState();
            }
            _tempActivityModules.Clear();
        }

        public static void ReleaseAllModules()
        {
            var modules = new List<ActivityModule>(_activityModules.Values);
            _activityModules.Clear();

            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].OnDispose();
            }
        }

        public static bool ReleaseActivityModule<T>() where T : ActivityModule
        {
            return ReleaseActivityModule(typeof(T));
        }

        public static bool ReleaseActivityModule(Type type)
        {
            if (type == null || !type.IsSubclassOf(typeof(ActivityModule)) || type.IsAbstract)
            {
                return false;
            }

            if (!_activityModules.TryRemove(type, out var module))
            {
                return false;
            }

            module.OnDispose();
            return true;
        }

        protected static T GetInstance<T>() where T : ActivityModule, new()
        {
            return GetActivityModule<T>();
        }

        private static void DiscoverActivityModules()
        {
            _activityModuleTypes.Clear();
            Type baseType = typeof(ActivityModule);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException exception)
                {
                    types = exception.Types;
                }

                if (types == null)
                {
                    continue;
                }

                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    if (type == null || type.IsAbstract || !type.IsSubclassOf(baseType))
                    {
                        continue;
                    }

                    if (!_activityModuleTypes.Contains(type))
                    {
                        _activityModuleTypes.Add(type);
                        ActivityModule module = (ActivityModule)Activator.CreateInstance(type);
                        _activityModules[type] = module;
                        module.Initialize();
                    }
                }
            }
        }

        private void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            Init();
            IsInitialized = true;
            RefreshState();
        }

        protected abstract void Init();

        protected virtual void OnDispose()
        {
        }

        public virtual void OnEnterGame()
        {
            RefreshState();
        }

        public virtual void OnQuitGame()
        {
        }

        public void RefreshState()
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            var previousState = State;
            bool isUnlocked = EvaluateUnlocked();
            bool isVisible = EvaluateVisible(isUnlocked);
            bool isOpen = EvaluateOpen(isUnlocked);

            IsUnlocked = isUnlocked;
            IsVisible = isVisible;
            IsOpen = isOpen;

            var currentState = State;
            if (previousState != currentState)
            {
                if (previousState.IsUnlocked != currentState.IsUnlocked)
                {
                    OnUnlockStateChanged(previousState.IsUnlocked, currentState.IsUnlocked);
                }

                if (previousState.IsVisible != currentState.IsVisible)
                {
                    OnVisibleStateChanged(previousState.IsVisible, currentState.IsVisible);
                }

                if (previousState.IsOpen != currentState.IsOpen)
                {
                    OnOpenStateChanged(previousState.IsOpen, currentState.IsOpen);
                }

                OnStateChanged(previousState, currentState);
                StateChanged?.Invoke(this, previousState, currentState);
            }
        }

        protected virtual void OnStateChanged(ActivityModuleState previousState, ActivityModuleState currentState)
        {
        }

        protected virtual void OnUnlockStateChanged(bool previousValue, bool currentValue)
        {
        }

        protected virtual void OnVisibleStateChanged(bool previousValue, bool currentValue)
        {
        }

        protected virtual void OnOpenStateChanged(bool previousValue, bool currentValue)
        {
        }

        protected void AddUnlockCondition(Func<bool> predicate)
        {
            if (predicate != null)
            {
                _unlockConditions.Add(predicate);
            }
        }

        protected void AddVisibleCondition(Func<bool> predicate)
        {
            if (predicate != null)
            {
                _visibleConditions.Add(predicate);
            }
        }

        protected void AddOpenCondition(Func<bool> predicate)
        {
            if (predicate != null)
            {
                _openConditions.Add(predicate);
            }
        }

        protected virtual bool EvaluateUnlockedCore()
        {
            return true;
        }

        protected virtual bool EvaluateVisibleCore(bool isUnlocked)
        {
            return isUnlocked;
        }

        protected virtual bool EvaluateOpenCore(bool isUnlocked)
        {
            return isUnlocked;
        }

        private bool EvaluateUnlocked()
        {
            return EvaluateUnlockedCore() && EvaluateConditions(_unlockConditions);
        }

        private bool EvaluateVisible(bool isUnlocked)
        {
            return EvaluateVisibleCore(isUnlocked) && EvaluateConditions(_visibleConditions);
        }

        private bool EvaluateOpen(bool isUnlocked)
        {
            return EvaluateOpenCore(isUnlocked) && EvaluateConditions(_openConditions);
        }

        private bool EvaluateConditions(List<Func<bool>> conditions)
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                try
                {
                    if (!conditions[i].Invoke())
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    LogF8.LogError("ActivityModule 条件执行异常：{0}\n{1}", ModuleName, exception);
                    return false;
                }
            }

            return true;
        }
    }
}
