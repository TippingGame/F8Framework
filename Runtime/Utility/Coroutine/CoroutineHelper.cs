using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    [DisallowMultipleComponent]
    public class CoroutineHelper : MonoBehaviour
    {
        class DelayTask : IEquatable<DelayTask>, IReference
        {
            public long TaskId;
            public float Delay;
            public float CurrentTime;
            public Action Action;

            public bool Equals(DelayTask other)
            {
                return other.TaskId == this.TaskId;
            }

            public void Clear()
            {
                TaskId = -1;
                Delay = 0;
                CurrentTime = 0;
                Action = null;
            }
        }

        class ConditionTask : IEquatable<ConditionTask>, IReference
        {
            public long TaskId;
            public float Timeout;
            public float CurrentTime;
            public Func<bool> Condition;
            public Action Action;

            public bool Equals(ConditionTask other)
            {
                return other.TaskId == this.TaskId;
            }

            public void Clear()
            {
                TaskId = -1;
                Timeout = 0;
                CurrentTime = 0;
                Condition = null;
                Action = null;
            }
        }

        class CoroutineActionTask : IReference
        {
            public Action Action;
            public long TaskId;

            public IEnumerator Task()
            {
                Action?.Invoke();
                yield return null;
            }

            public void Clear()
            {
                TaskId = -1;
                Action = null;
            }
        }

        class CoroutineTask : IReference
        {
            public IEnumerator Enumerator;
            public Action<Coroutine> Callback;
            public long TaskId;

            public void Clear()
            {
                TaskId = -1;
                Enumerator = null;
                Callback = null;
            }
        }

        List<CoroutineTask> routineList = new List<CoroutineTask>();
        static long taskIndex = 0;
        readonly Dictionary<long, DelayTask> delayTaskDict = new Dictionary<long, DelayTask>();
        readonly List<DelayTask> delayTaskList = new List<DelayTask>();
        readonly List<DelayTask> removalDelayTaskList = new List<DelayTask>();

        readonly Dictionary<long, ConditionTask> conditionTaskDict = new Dictionary<long, ConditionTask>();
        readonly List<ConditionTask> conditionTaskList = new List<ConditionTask>();
        readonly List<ConditionTask> removalConditionTaskList = new List<ConditionTask>();

        readonly Dictionary<long, CoroutineActionTask> coroutineActionTaskDict =
            new Dictionary<long, CoroutineActionTask>();

        readonly List<CoroutineActionTask> coroutineActionTaskList = new List<CoroutineActionTask>();
        bool runningCoroutineTask;
        Coroutine coroutineActionPtr;

        /// <summary>
        /// 加入延迟任务
        /// </summary>
        /// <param name="delay">延迟时间，delay>=0</param>
        /// <param name="action">触发的事件</param>
        /// <returns>任务Id</returns>
        public long AddDelayTask(float delay, Action action)
        {
            if (delay < 0)
                delay = 0;
            var taskId = GenerateTaskId();
            var delayTask = ReferencePool.Acquire<DelayTask>();
            delayTask.TaskId = taskId;
            delayTask.CurrentTime = 0;
            delayTask.Delay = delay;
            delayTask.Action = action;

            delayTaskList.Add(delayTask);
            delayTaskDict.Add(delayTask.TaskId, delayTask);
            return delayTask.TaskId;
        }

        /// <summary>
        /// 添加条件任务。包含超时机制，超时自动抛弃任务，不触发。
        /// </summary>
        /// <param name="condition">触发条件，返回值为true时触发action</param>
        /// <param name="action">执行回调函数</param>
        /// <param name="timeout">超时时间，默认int.MaxValue，超时自动抛弃任务</param>
        /// <returns>任务Id</returns>
        public long AddConditionTask(Func<bool> condition, Action action, float timeout)
        {
            if (timeout <= 0)
                timeout = 0;
            var taskId = GenerateTaskId();
            var conditionTask = ReferencePool.Acquire<ConditionTask>();
            conditionTask.TaskId = taskId;
            conditionTask.Timeout = timeout;
            conditionTask.CurrentTime = 0;
            conditionTask.Condition = condition;
            conditionTask.Action = action;

            conditionTaskList.Add(conditionTask);
            conditionTaskDict.Add(conditionTask.TaskId, conditionTask);
            return conditionTask.TaskId;
        }

        /// <summary>
        /// 移除延迟任务，已触发的则自动移除
        /// </summary>
        /// <param name="taskId">任务Id</param>
        public void RemoveDelayTask(long taskId)
        {
            if (delayTaskDict.TryRemove(taskId, out var task))
            {
                delayTaskList.Remove(task);
                ReferencePool.Release(task);
            }
        }

        /// <summary>
        /// 移除条件任务
        /// </summary>
        /// <param name="taskId">任务Id</param>
        public void RemoveConditionTask(long taskId)
        {
            if (conditionTaskDict.TryRemove(taskId, out var task))
            {
                conditionTaskList.Remove(task);
                ReferencePool.Release(task);
            }
        }

        public void StopAllDelayTask()
        {
            var length = delayTaskList.Count;
            for (int i = 0; i < length; i++)
            {
                var task = delayTaskList[i];
                ReferencePool.Release(task);
            }

            delayTaskList.Clear();
            delayTaskDict.Clear();
        }

        public void StopAllConditionTask()
        {
            var length = conditionTaskList.Count;
            for (int i = 0; i < length; i++)
            {
                var task = conditionTaskList[i];
                ReferencePool.Release(task);
            }

            conditionTaskList.Clear();
            conditionTaskDict.Clear();
        }

        /// <summary>
        /// 添加协程任务，每个任务之间完成会相隔一帧
        /// </summary>
        /// <param name="action">事件</param>
        /// <returns>任务Id</returns>
        public long AddCoroutineActionTask(Action action)
        {
            var taskId = GenerateTaskId();
            var coroutineTask = ReferencePool.Acquire<CoroutineActionTask>();
            coroutineTask.Action = action;
            coroutineTask.TaskId = taskId;
            coroutineActionTaskDict.Add(taskId, coroutineTask);
            if (!runningCoroutineTask)
            {
                StartCoroutine(RunCoroutineActionTask());
                runningCoroutineTask = true;
            }

            return taskId;
        }

        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="taskId">任务Id</param>
        public void RemoveCoroutineActionTask(long taskId)
        {
            if (coroutineActionTaskDict.Remove(taskId, out var coroutineTask))
            {
                coroutineActionTaskList.Remove(coroutineTask);
                ReferencePool.Release(coroutineTask);
            }
        }

        /// <summary>
        /// 停止所有协程事件任务
        /// </summary>
        public void StopAllCoroutineActionTask()
        {
            if (coroutineActionPtr != null)
            {
                StopCoroutine(coroutineActionPtr);
                runningCoroutineTask = false;
            }

            var length = coroutineActionTaskList.Count;
            for (int i = 0; i < length; i++)
            {
                var coroutineTask = coroutineActionTaskList[i];
                ReferencePool.Release(coroutineTask);
            }

            coroutineActionTaskDict.Clear();
            coroutineActionTaskList.Clear();
        }

        /// <summary>
        /// 条件协程；
        /// </summary>
        /// <param name="handler">目标条件</param>
        /// <param name="callBack">条件达成后执行的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
        {
            return StartCoroutine(EnumPredicateCoroutine(handler, callBack));
        }

        /// <summary>
        /// 嵌套协程；
        /// </summary>
        /// <param name="predicateHandler">条件函数</param>
        /// <param name="nestHandler">条件成功后执行的嵌套协程</param>
        /// <returns>Coroutine></returns>
        public Coroutine PredicateNestCoroutine(Func<bool> predicateHandler, Action nestHandler)
        {
            return StartCoroutine(EnumPredicateNestCoroutine(predicateHandler, nestHandler));
        }

        /// <summary>
        /// 延时协程；
        /// </summary>
        /// <param name="delay">延时的时间</param>
        /// <param name="callBack">延时后的回调函数</param>
        /// <returns>协程对象</returns>
        public Coroutine DelayCoroutine(float delay, Action callBack)
        {
            return StartCoroutine(EnumDelay(delay, callBack));
        }

        public Coroutine StartCoroutine(Action handler)
        {
            return StartCoroutine(EnumCoroutine(handler));
        }

        public Coroutine StartCoroutine(Action handler, Action callback)
        {
            return StartCoroutine(EnumCoroutine(handler, callback));
        }

        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        public Coroutine StartCoroutine(Coroutine routine, Action callBack)
        {
            return StartCoroutine(EnumCoroutine(routine, callBack));
        }

        public void AddCoroutine(IEnumerator routine, Action<Coroutine> callback)
        {
            var task = ReferencePool.Acquire<CoroutineTask>();
            task.Enumerator = routine;
            task.Callback = callback;
            routineList.Add(task);
        }

        void Update()
        {
            RefreshRoutineTask();
            RefreshDelayTask();
            RefreshConditionTask();
        }

        IEnumerator EnumDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator EnumCoroutine(Coroutine routine, Action callBack)
        {
            yield return routine;
            callBack?.Invoke();
        }

        IEnumerator EnumCoroutine(Action handler)
        {
            handler?.Invoke();
            yield return null;
        }

        IEnumerator EnumCoroutine(Action handler, Action callack)
        {
            yield return StartCoroutine(handler);
            callack?.Invoke();
        }

        IEnumerator EnumPredicateCoroutine(Func<bool> handler, Action callBack)
        {
            yield return new WaitUntil(handler);
            callBack();
        }

        /// <summary>
        /// 嵌套协程执行体
        /// </summary>
        /// <param name="predicateHandler">条件函数</param>
        /// <param name="nestHandler">条件成功后执行的嵌套协程</param>
        IEnumerator EnumPredicateNestCoroutine(Func<bool> predicateHandler, Action nestHandler)
        {
            yield return new WaitUntil(predicateHandler);
            yield return StartCoroutine(EnumCoroutine(nestHandler));
        }

        void RefreshRoutineTask()
        {
            while (routineList.Count > 0)
            {
                var task = routineList[0];
                routineList.RemoveAt(0);
                var coroutine = StartCoroutine(task.Enumerator);
                task.Callback?.Invoke(coroutine);
                ReferencePool.Release(task);
            }
        }

        void RefreshDelayTask()
        {
            removalDelayTaskList.Clear();
            var taskArray = delayTaskList;
            var taskCount = taskArray.Count;
            for (int i = 0; i < taskCount; i++)
            {
                var task = taskArray[i];
                task.CurrentTime += Time.deltaTime;
                if (task.CurrentTime >= task.Delay)
                {
                    try
                    {
                        task.Action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        LogF8.LogError(e);
                    }

                    removalDelayTaskList.Add(task);
                }
            }

            var removeCount = removalDelayTaskList.Count;
            for (int i = 0; i < removeCount; i++)
            {
                var removeTask = removalDelayTaskList[i];
                RemoveDelayTask(removeTask.TaskId);
            }
        }

        void RefreshConditionTask()
        {
            removalConditionTaskList.Clear();
            var taskArray = conditionTaskList;
            var taskCount = taskArray.Count;
            for (int i = 0; i < taskCount; i++)
            {
                var task = taskArray[i];
                task.CurrentTime += Time.deltaTime;
                if (task.CurrentTime >= task.Timeout)
                {
                    removalConditionTaskList.Add(task);
                    continue;
                }

                bool triggered = false;
                if (task.Condition == null)
                    triggered = true;
                else
                {
                    try
                    {
                        triggered = task.Condition.Invoke();
                    }
                    catch (Exception e)
                    {
                        LogF8.LogError(e);
                        triggered = true;
                    }
                }

                if (triggered)
                {
                    try
                    {
                        task.Action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        LogF8.LogError(e);
                    }

                    removalConditionTaskList.Add(task);
                }
            }

            var removeCount = removalConditionTaskList.Count;
            for (int i = 0; i < removeCount; i++)
            {
                var removeTask = removalConditionTaskList[i];
                RemoveConditionTask(removeTask.TaskId);
            }
        }

        IEnumerator RunCoroutineActionTask()
        {
            runningCoroutineTask = true;
            while (coroutineActionTaskList.Count > 0)
            {
                var coroutineTask = coroutineActionTaskList[0];
                coroutineActionTaskList.RemoveAt(0);
                yield return coroutineTask.Task();
                ReferencePool.Release(coroutineTask);
            }

            runningCoroutineTask = false;
        }

        /// <summary>
        /// 生成任务Id
        /// </summary>
        /// <returns>任务Id</returns>
        long GenerateTaskId()
        {
            if (taskIndex == long.MaxValue)
                taskIndex = 0;
            else
                taskIndex++;
            return taskIndex;
        }
    }
}