using UnityEngine;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class PathTween : BaseTween
    {
        private Transform targetTransform;
        private Vector3[] path;
        private PathType pathType = PathType.CatmullRom;
        private PathMode pathMode = PathMode.Ignore;
        private int resolution = 10;
        private bool closePath = false;
        
        private PathCalculator pathCalculator;
        private Vector3 tempValue = Vector3.zero;
        private Quaternion tempRotation = Quaternion.identity;
        
        public PathTween(Transform target, IList<Vector3> path, float duration, PathType pathType, PathMode pathMode, int resolution, bool closePath, int id)
        {
            Init(target, path, duration, pathType, pathMode, resolution, closePath);
            this.id = id;
        }

        internal void Init(Transform target, IList<Vector3> path, float duration, PathType pathType, PathMode pathMode, int resolution, bool closePath)
        {
            this.targetTransform = target;
            if (path is Vector3[] array)
            {
                this.path = array;
            }
            else
            {
                this.path = new Vector3[path.Count];
                for (int i = 0; i < path.Count; i++)
                {
                    this.path[i] = path[i];
                }
            }
            this.duration = duration;
            this.pathType = pathType;
            this.pathMode = pathMode;
            this.resolution = Mathf.Max(2, resolution);
            this.closePath = closePath;
            
            // 初始化路径计算器
            InitializePathCalculator();
            
            this.PauseReset = () => this.Init(target, path, duration, pathType, pathMode, resolution, closePath);
        }

        /// <summary>
        /// 初始化路径计算器
        /// </summary>
        private void InitializePathCalculator()
        {
            if (path == null || path.Length < 2)
                return;
                
            var options = new PathOptions
            {
                pathType = pathType,
                pathMode = pathMode,
                resolution = resolution,
                closePath = closePath
            };
            
            pathCalculator = new PathCalculator(path, options);
        }

        /// <summary>
        /// 每帧执行的更新逻辑
        /// </summary>
        public override void Update(float deltaTime)
        {
            if (isPause || IsComplete || IsRecycle || targetTransform == null || pathCalculator == null)
                return;

            // 处理启动延迟
            if (delay > 0.0f)
            {
                delay -= deltaTime;
                return;
            }
            
            base.Update(deltaTime);

            currentTime += deltaTime;
            
            // 检查是否完成当前周期
            if (currentTime >= duration)
            {
                // 确保到达终点
                UpdateTransform(1f);
                
                bool shouldComplete = !HandleLoop();
                if (shouldComplete)
                    onComplete();
            }
            else
            {
                float normalizedProgress = currentTime / duration;
                // 通过曲线函数计算缓动进度
                float curveProgress = GetCurveProgress(normalizedProgress);
                
                // 更新变换
                UpdateTransform(curveProgress);
            }
        }

        /// <summary>
        /// 更新位置和旋转
        /// </summary>
        private void UpdateTransform(float progress)
        {
            if (pathCalculator == null || targetTransform == null)
                return;
                
            // 获取路径上的位置
            Vector3 newPosition = pathCalculator.GetPoint(progress);
            tempValue = newPosition;
            targetTransform.position = newPosition;
            
            // 处理朝向
            UpdateRotation(progress, newPosition);
            
            // 触发值更新回调
            if (onUpdateVector3 != null)
                onUpdateVector3(tempValue);

            if (onUpdateVector2 != null)
                onUpdateVector2(tempValue);
        }

        /// <summary>
        /// 更新旋转
        /// </summary>
        private void UpdateRotation(float progress, Vector3 currentPosition)
        {
            if (pathMode == PathMode.Ignore)
                return;
                
            Vector3 direction = pathCalculator.GetDirection(progress);
            if (direction == Vector3.zero)
                return;
                
            switch (pathMode)
            {
                case PathMode.Full3D:
                    // 3D朝向，Z轴指向移动方向
                    tempRotation = Quaternion.LookRotation(direction);
                    targetTransform.rotation = tempRotation;
                    break;
                    
                case PathMode.TopDown2D:
                    // 俯视2D，在XY平面内旋转
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    tempRotation = Quaternion.Euler(0, 0, angle - 90);
                    targetTransform.rotation = tempRotation;
                    break;
                    
                case PathMode.Sidescroller2D:
                    // 横版2D，根据X方向翻转
                    UpdateSidescrollerRotation(direction);
                    break;
            }
        }

        /// <summary>
        /// 更新横版2D旋转
        /// </summary>
        private void UpdateSidescrollerRotation(Vector3 direction)
        {
            if (Mathf.Abs(direction.x) > 0.1f)
            {
                Vector3 scale = targetTransform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
                targetTransform.localScale = scale;
            }
        }

        /// <summary>
        /// 设置是否闭合路径
        /// </summary>
        public PathTween SetClosePath(bool close)
        {
            this.closePath = close;
            InitializePathCalculator(); // 重新初始化计算器
            return this;
        }

        public override void Reset()
        {
            base.Reset();
            targetTransform = null;
            path = null;
            pathCalculator = null;
            pathType = PathType.CatmullRom;
            pathMode = PathMode.Ignore;
            resolution = 10;
            closePath = false;
        }

        public override void ReplayReset()
        {
            base.ReplayReset();
            if (targetTransform != null && path != null)
            {
                Init(targetTransform, path, duration, pathType, pathMode, resolution, closePath);
            }
        }
        
        private float GetCurveProgress(float normalizedProgress)
        {
            switch (loopType)
            {
                case LoopType.Yoyo:
                    // 使用平滑的往返曲线 (0→1→0)
                    return Mathf.PingPong(normalizedProgress * 2, 1);
                default:
                    return normalizedProgress;
            }
        }
        
        private bool HandleLoop()
        {
            if (this.loopType == LoopType.None || this.tempLoopCount == 0)
            {
                return false;
            }
            else
            {
                if (this.tempLoopCount > 0)
                {
                    this.tempLoopCount -= 1;
                }
                
                // 对于路径动画，大部分循环类型不需要特殊处理
                // 因为路径计算器会处理进度
                switch (this.loopType)
                {
                    case LoopType.Restart:
                        break;
                    case LoopType.Flip:
                        // 对于路径，翻转可能没有意义，保持原样
                        break;
                    case LoopType.Incremental:
                        // 路径动画通常不支持增量
                        break;
                    case LoopType.Yoyo:
                        break;
                }
                
                this.ReplayReset();
                return this.tempLoopCount > 0 || this.tempLoopCount == -1;
            }
        }
    }
}