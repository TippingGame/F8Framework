using UnityEngine;

namespace F8Framework.Core
{
    public sealed class F8PoolDespawnTimer : MonoBehaviour, ISpawnable
    {
        [SerializeField] private UpdateType _updateType = UpdateType.Update;
        [SerializeField, Min(0f)] private float _timeToDespawn = 3f;

        private bool _hasDespawnPerformed;
        private float _elapsedTime;

#if DEBUG
        private void Start()
        {
            if (GameObjectPool.Instance.IsClone(gameObject) == false)
            {
                LogF8.LogError("您已将一个取消生成计时器添加到不是克隆的游戏对象！", this);
            }
            
            if (GameObjectPool.Instance.TryGetPoolByClone(gameObject, out F8GameObjectPool pool))
            {
                if (pool.BehaviourOnCapacityReached == BehaviourOnCapacityReached.Recycle)
                {
                    if (pool.CallbacksType == CallbacksType.None)
                    {
                        LogF8.LogEntity("此回收池中已禁用回调！" +
                                        "在回收池中，当生成时，定时器可能不会重置！" +
                                        "在池中启用回调或选择其他 '达到容量时的行为' 选项。", pool);
                    }
                }
            }
        }
#endif
        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void OnSpawn()
        {
            ResetTimer();
        }

        private void OnDisable()
        {
            ResetTimer();
        }

        private void Update()
        {
            if (_updateType == UpdateType.Update)
            {
                HandleDespawn(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (_updateType == UpdateType.LateUpdate)
            {
                HandleDespawn(Time.deltaTime);
            }
        }

        private void HandleDespawn(float deltaTime)
        {
            if (IsDespawnMoment(deltaTime))
            {
                GameObjectPool.Instance.Despawn(gameObject);
            }
        }

        private bool IsDespawnMoment(float deltaTime)
        {
            if (_hasDespawnPerformed)
                return false;
            
            _elapsedTime += deltaTime;

            if (_elapsedTime >= _timeToDespawn)
                return true;

            return false;
        }

        private void ResetTimer()
        {
            _hasDespawnPerformed = false;
            _elapsedTime = 0f;
        }
    }
}