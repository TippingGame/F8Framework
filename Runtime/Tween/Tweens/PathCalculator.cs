using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 路径类型
    /// </summary>
    public enum PathType
    {
        Linear,      // 线性
        CatmullRom   // 卡特穆尔罗姆曲线
    }
    
    /// <summary>
    /// 路径模式
    /// </summary>
    public enum PathMode
    {
        Ignore,         // 忽略朝向
        Full3D,         // 3D全朝向
        TopDown2D,      // 俯视2D
        Sidescroller2D  // 横版2D
    }
    
    /// <summary>
    /// 路径选项
    /// </summary>
    public struct PathOptions
    {
        public PathType pathType;
        public PathMode pathMode;
        public int resolution;
        public bool closePath;
        
        public static PathOptions Default => new PathOptions
        {
            pathType = PathType.CatmullRom,
            pathMode = PathMode.Ignore,
            resolution = 10,
            closePath = false
        };
    }
    
    public class PathCalculator
    {
        private Vector3[] path;
        private PathType pathType;
        private int resolution;
        private bool closePath;
        
        // 缓存的曲线点
        private List<Vector3> cachedCurvePoints;
        private float[] segmentLengths;
        private float totalLength;
        
        public PathCalculator(Vector3[] path, PathOptions options)
        {
            this.path = path;
            this.pathType = options.pathType;
            this.resolution = Mathf.Max(2, options.resolution);
            this.closePath = options.closePath;
            
            CalculateCurve();
        }
        
        /// <summary>
        /// 计算曲线路径
        /// </summary>
        private void CalculateCurve()
        {
            if (path == null || path.Length < 2)
            {
                cachedCurvePoints = new List<Vector3>();
                return;
            }
            
            cachedCurvePoints = new List<Vector3>();
            
            switch (pathType)
            {
                case PathType.Linear:
                    CalculateLinearPath();
                    break;
                case PathType.CatmullRom:
                    CalculateCatmullRomPath();
                    break;
            }
            
            CalculatePathLengths();
        }
        
        /// <summary>
        /// 计算线性路径
        /// </summary>
        private void CalculateLinearPath()
        {
            for (int i = 0; i < path.Length; i++)
            {
                cachedCurvePoints.Add(path[i]);
                
                if (i < path.Length - 1)
                {
                    // 在线段间插入点以提高精度
                    for (int j = 1; j < resolution; j++)
                    {
                        float t = j / (float)resolution;
                        Vector3 point = Vector3.Lerp(path[i], path[i + 1], t);
                        cachedCurvePoints.Add(point);
                    }
                }
            }
            
            if (closePath && path.Length > 2)
            {
                // 闭合路径
                for (int j = 1; j < resolution; j++)
                {
                    float t = j / (float)resolution;
                    Vector3 point = Vector3.Lerp(path[path.Length - 1], path[0], t);
                    cachedCurvePoints.Add(point);
                }
            }
        }
        
        /// <summary>
        /// 计算Catmull-Rom曲线路径
        /// </summary>
        private void CalculateCatmullRomPath()
        {
            if (path.Length < 2)
                return;
        
            List<Vector3> points = new List<Vector3>(path);
    
            // 处理点数量不足的情况
            if (points.Count == 2)
            {
                // 只有2个点时，退化为线性插值
                CalculateLinearPath();
                return;
            }
    
            if (closePath && points.Count > 2)
            {
                // 闭合路径时需要添加额外的点
                points.Insert(0, points[points.Count - 1]);
                points.Add(points[1]); // 原本是points[1]，现在改为points[2]
                points.Add(points[2]); // 添加额外的点确保有足够的控制点
            }
            else
            {
                // 非闭合路径时，复制端点作为控制点
                if (points.Count == 3)
                {
                    // 3个点时，复制第一个和最后一个点
                    points.Insert(0, points[0]);
                    points.Add(points[points.Count - 1]);
                }
                else if (points.Count >= 4)
                {
                    // 4个或以上点时正常处理
                    // 可以保持原有逻辑，或者也进行端点复制以获得更好的端点行为
                    points.Insert(0, points[0]);
                    points.Add(points[points.Count - 1]);
                }
            }
    
            cachedCurvePoints.Clear();
    
            // 确保有足够的点进行计算
            if (points.Count < 4)
            {
                CalculateLinearPath();
                return;
            }
    
            for (int i = 0; i < points.Count - 3; i++)
            {
                Vector3 p0 = points[i];
                Vector3 p1 = points[i + 1];
                Vector3 p2 = points[i + 2];
                Vector3 p3 = points[i + 3];
        
                for (int j = 0; j <= resolution; j++)
                {
                    float t = j / (float)resolution;
                    Vector3 point = CalculateCatmullRomPoint(t, p0, p1, p2, p3);
            
                    // 避免重复添加点（第一个段的第一个点除外）
                    if (i > 0 && j == 0) 
                        continue;
                
                    cachedCurvePoints.Add(point);
                }
            }
    
            // 如果没有生成任何点，退回到线性路径
            if (cachedCurvePoints.Count == 0)
            {
                CalculateLinearPath();
            }
        }
        
        /// <summary>
        /// 计算Catmull-Rom曲线点
        /// </summary>
        private Vector3 CalculateCatmullRomPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            
            return 0.5f * (
                (2 * p1) +
                (-p0 + p2) * t +
                (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
                (-p0 + 3 * p1 - 3 * p2 + p3) * t3
            );
        }
        
        /// <summary>
        /// 计算路径长度
        /// </summary>
        private void CalculatePathLengths()
        {
            if (cachedCurvePoints.Count < 2)
            {
                segmentLengths = new float[0];
                totalLength = 0;
                return;
            }
            
            segmentLengths = new float[cachedCurvePoints.Count - 1];
            totalLength = 0;
            
            for (int i = 0; i < cachedCurvePoints.Count - 1; i++)
            {
                float segmentLength = Vector3.Distance(cachedCurvePoints[i], cachedCurvePoints[i + 1]);
                segmentLengths[i] = segmentLength;
                totalLength += segmentLength;
            }
        }
        
        /// <summary>
        /// 根据进度获取路径上的点
        /// </summary>
        public Vector3 GetPoint(float progress)
        {
            progress = Mathf.Clamp01(progress);
            
            if (cachedCurvePoints.Count == 0)
                return Vector3.zero;
                
            if (cachedCurvePoints.Count == 1)
                return cachedCurvePoints[0];
            
            float targetDistance = progress * totalLength;
            float currentDistance = 0f;
            
            for (int i = 0; i < segmentLengths.Length; i++)
            {
                float segmentLength = segmentLengths[i];
                
                if (currentDistance + segmentLength >= targetDistance)
                {
                    float segmentProgress = (targetDistance - currentDistance) / segmentLength;
                    return Vector3.Lerp(cachedCurvePoints[i], cachedCurvePoints[i + 1], segmentProgress);
                }
                
                currentDistance += segmentLength;
            }
            
            return cachedCurvePoints[cachedCurvePoints.Count - 1];
        }
        
        /// <summary>
        /// 获取路径方向（用于朝向计算）
        /// </summary>
        public Vector3 GetDirection(float progress, float delta = 0.01f)
        {
            float progress1 = Mathf.Clamp01(progress - delta);
            float progress2 = Mathf.Clamp01(progress + delta);
            
            Vector3 point1 = GetPoint(progress1);
            Vector3 point2 = GetPoint(progress2);
            
            return (point2 - point1).normalized;
        }
    }
}