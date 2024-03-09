using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public static partial class Util
    {
        /// <summary>
        /// 这个类封装了所有跟Unity相关的工具函数
        /// </summary>
        public static class Unity
        {
            static CoroutineHelper coroutineHelper;

            static CoroutineHelper CoroutineHelper
            {
                get
                {
                    if (coroutineHelper == null)
                    {
                        MonoBehaviour mono = ModuleCenter.GetBehaviour();
                        coroutineHelper = mono.gameObject.AddComponent<CoroutineHelper>();
                    }

                    return coroutineHelper;
                }
            }

            #region Application

            /// <summary>
            /// 退出。editor停止播放，runtime则退出游戏
            /// </summary>
            public static void Quit()
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }

            #endregion

            #region UnityComponent

            /// <summary>
            /// 查找目标场景中的目标对象
            /// </summary>
            /// <param name="sceneName">传入的场景名</param>
            /// <param name="condition">查找条件</param>
            /// <returns>查找到的对象</returns>
            public static GameObject FindSceneGameObject(string sceneName, Func<GameObject, bool> condition)
            {
                var scene = SceneManager.GetSceneByName(sceneName);
                return scene.GetRootGameObjects().FirstOrDefault(condition);
            }

            /// <summary>
            /// 对unity对象进行升序排序
            /// </summary>
            /// <typeparam name="T">组件类型</typeparam>
            /// <typeparam name="K">排序的值</typeparam>
            /// <param name="comps">传入的组件数组</param>
            /// <param name="handler">处理的方法</param>
            public static void SortCompsByAscending<T, K>(T[] comps, Func<T, K> handler)
                where K : IComparable<K>
                where T : Component
            {
                Algorithm.SortByAscend(comps, handler);
                var length = comps.Length;
                for (int i = 0; i < length; i++)
                {
                    comps[i].transform.SetSiblingIndex(i);
                }
            }

            /// <summary>
            /// 对unity对象进行降序排序
            /// </summary>
            /// <typeparam name="T">组件类型</typeparam>
            /// <typeparam name="K">排序的值</typeparam>
            /// <param name="comps">传入的组件数组</param>
            /// <param name="handler">处理的方法</param>
            public static void SortCompsByDescending<T, K>(T[] comps, Func<T, K> handler)
                where K : IComparable<K>
                where T : Component
            {
                Algorithm.SortByDescend(comps, handler);
                var length = comps.Length;
                for (int i = 0; i < length; i++)
                {
                    comps[i].transform.SetSiblingIndex(i);
                }
            }

            #endregion

            #region Graphics

            /// <summary>
            /// 通过相机截取屏幕并转换为Texture2D
            /// </summary>
            /// <param name="camera">目标相机</param>
            /// <returns>相机抓取的屏幕Texture2D</returns>
            public static Texture2D CameraScreenshotAsTextureRGB(Camera camera)
            {
                return CameraScreenshotAsTexture(camera, TextureFormat.RGB565);
            }

            public static Texture2D CameraScreenshotAsTextureRGBA(Camera camera)
            {
                return CameraScreenshotAsTexture(camera, TextureFormat.RGBA32);
            }

            public static Texture2D CameraScreenshotAsTexture(Camera camera, TextureFormat textureFormat)
            {
                var oldRenderTexture = camera.targetTexture;
                var width = camera.pixelWidth;
                var height = camera.pixelHeight;
                RenderTexture renderTexture;
                renderTexture = new RenderTexture(width, height, 24);
                camera.targetTexture = renderTexture;
                camera.Render();
                Texture2D texture2D = new Texture2D(width, height, textureFormat, false);
                RenderTexture.active = renderTexture;
                texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture2D.Apply();
                RenderTexture.active = null;
                camera.targetTexture = oldRenderTexture;
                return texture2D;
            }

            /// <summary>
            /// 通过相机截取屏幕并转换为Sprite
            /// </summary>
            /// <param name="camera">目标相机</param>
            /// <returns>相机抓取的屏幕Texture2D</returns>
            public static Sprite CameraScreenshotAsSpriteRGBA(Camera camera)
            {
                var texture2D = CameraScreenshotAsTextureRGBA(camera);
                var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                return sprite;
            }

            public static Sprite CameraScreenshotAsSpriteRGB(Camera camera)
            {
                var texture2D = CameraScreenshotAsTextureRGB(camera);
                var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                return sprite;
            }

            public static Sprite CameraScreenshotAsSprite(Camera camera, TextureFormat textureFormat)
            {
                var texture2D = CameraScreenshotAsTexture(camera, textureFormat);
                var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                return sprite;
            }

            public static Texture2D BytesToTexture2D(byte[] bytes, int width, int height)
            {
                Texture2D texture2D = new Texture2D(width, height);
                texture2D.LoadImage(bytes);
                return texture2D;
            }

            /// <summary>
            /// 双线性插值法缩放图片，等比缩放 
            /// </summary>
            public static Texture2D ScaleTextureBilinear(Texture2D originalTexture, float scaleFactor)
            {
                Texture2D newTexture = new Texture2D(Mathf.CeilToInt(originalTexture.width * scaleFactor),
                    Mathf.CeilToInt(originalTexture.height * scaleFactor));
                float scale = 1.0f / scaleFactor;
                int maxX = originalTexture.width - 1;
                int maxY = originalTexture.height - 1;
                for (int y = 0; y < newTexture.height; y++)
                {
                    for (int x = 0; x < newTexture.width; x++)
                    {
                        float targetX = x * scale;
                        float targetY = y * scale;
                        int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                        int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                        int x2 = Mathf.Min(maxX, x1 + 1);
                        int y2 = Mathf.Min(maxY, y1 + 1);

                        float u = targetX - x1;
                        float v = targetY - y1;
                        float w1 = (1 - u) * (1 - v);
                        float w2 = u * (1 - v);
                        float w3 = (1 - u) * v;
                        float w4 = u * v;
                        Color color1 = originalTexture.GetPixel(x1, y1);
                        Color color2 = originalTexture.GetPixel(x2, y1);
                        Color color3 = originalTexture.GetPixel(x1, y2);
                        Color color4 = originalTexture.GetPixel(x2, y2);
                        Color color = new Color(
                            Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                            Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                            Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                            Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                        );
                        newTexture.SetPixel(x, y, color);
                    }
                }

                newTexture.Apply();
                return newTexture;
            }

            /// <summary> 
            /// 双线性插值法缩放图片为指定尺寸 
            /// </summary>
            public static Texture2D SizeTextureBilinear(Texture2D originalTexture, Vector2 size)
            {
                Texture2D newTexture = new Texture2D(Mathf.CeilToInt(size.x), Mathf.CeilToInt(size.y));
                float scaleX = originalTexture.width / size.x;
                float scaleY = originalTexture.height / size.y;
                int maxX = originalTexture.width - 1;
                int maxY = originalTexture.height - 1;
                for (int y = 0; y < newTexture.height; y++)
                {
                    for (int x = 0; x < newTexture.width; x++)
                    {
                        float targetX = x * scaleX;
                        float targetY = y * scaleY;
                        int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                        int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                        int x2 = Mathf.Min(maxX, x1 + 1);
                        int y2 = Mathf.Min(maxY, y1 + 1);

                        float u = targetX - x1;
                        float v = targetY - y1;
                        float w1 = (1 - u) * (1 - v);
                        float w2 = u * (1 - v);
                        float w3 = (1 - u) * v;
                        float w4 = u * v;
                        Color color1 = originalTexture.GetPixel(x1, y1);
                        Color color2 = originalTexture.GetPixel(x2, y1);
                        Color color3 = originalTexture.GetPixel(x1, y2);
                        Color color4 = originalTexture.GetPixel(x2, y2);
                        Color color = new Color(
                            Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                            Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                            Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                            Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                        );
                        newTexture.SetPixel(x, y, color);
                    }
                }

                newTexture.Apply();
                return newTexture;
            }

            /// <summary> 
            /// Texture旋转
            /// </summary>
            public static Texture2D RotateTexture(Texture2D texture, float eulerAngles)
            {
                int x;
                int y;
                int i;
                int j;
                float phi = eulerAngles / (180 / Mathf.PI);
                float sn = Mathf.Sin(phi);
                float cs = Mathf.Cos(phi);
                Color32[] arr = texture.GetPixels32();
                Color32[] arr2 = new Color32[arr.Length];
                int W = texture.width;
                int H = texture.height;
                int xc = W / 2;
                int yc = H / 2;

                for (j = 0; j < H; j++)
                {
                    for (i = 0; i < W; i++)
                    {
                        arr2[j * W + i] = new Color32(0, 0, 0, 0);

                        x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                        y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);

                        if ((x > -1) && (x < W) && (y > -1) && (y < H))
                        {
                            arr2[j * W + i] = arr[y * W + x];
                        }
                    }
                }

                Texture2D newImg = new Texture2D(W, H);
                newImg.SetPixels32(arr2);
                newImg.Apply();

                return newImg;
            }

            /// <summary> 
            /// 在指定物体上添加指定图片 
            /// </summary>
            public static Image AddImage(GameObject target, Sprite sprite)
            {
                target.SetActive(false);
                Image image = target.GetComponent<Image>();
                if (!image)
                    image = target.AddComponent<Image>();
                image.sprite = sprite;
                image.SetNativeSize();
                target.SetActive(true);
                return image;
            }

            #endregion

            #region Coroutine

            /// <summary>
            /// 加入延迟任务
            /// </summary>
            /// <param name="delay">延迟时间，delay>=0</param>
            /// <param name="action">触发的事件</param>
            /// <returns>任务Id</returns>
            public static long AddDelayTask(float delay, Action action)
            {
                return CoroutineHelper.AddDelayTask(delay, action);
            }

            /// <summary>
            /// 添加条件任务。包含超时机制，超时自动抛弃任务，不触发。
            /// </summary>
            /// <param name="condition">触发条件，返回值为true时触发action</param>
            /// <param name="action">执行回调函数</param>
            /// <param name="timeout">超时时间，单位秒(second)，默认int.MaxValue，超时自动抛弃任务</param>
            /// <returns>任务Id</returns>
            public static long AddConditionTask(Func<bool> condition, Action action, float timeout = int.MaxValue)
            {
                return CoroutineHelper.AddConditionTask(condition, action, timeout);
            }

            public static void StopAllDelayTask()
            {
                CoroutineHelper.StopAllDelayTask();
            }

            public static void StopAllConditionTask()
            {
                CoroutineHelper.StopAllConditionTask();
            }

            /// <summary>
            /// 移除延迟任务，已触发的则自动移除
            /// </summary>
            /// <param name="taskId">任务Id</param>
            public static void RemoveDelayTask(long taskId)
            {
                CoroutineHelper.RemoveDelayTask(taskId);
            }

            public static void RemoveConditionTask(long taskId)
            {
                CoroutineHelper.RemoveConditionTask(taskId);
            }

            /// <summary>
            /// 添加协程事件任务，每个任务之间完成会相隔一帧
            /// </summary>
            /// <param name="action">事件</param>
            /// <returns>任务Id</returns>
            public static long AddCoroutineActionTask(Action action)
            {
                return CoroutineHelper.AddCoroutineActionTask(action);
            }

            /// <summary>
            /// 移除协程事件任务
            /// </summary>
            /// <param name="taskId">任务Id</param>
            public static void RemoveCoroutineActionTask(long taskId)
            {
                CoroutineHelper.RemoveCoroutineActionTask(taskId);
            }

            /// <summary>
            /// 移除所有协程事件
            /// </summary>
            public static void StopAllCoroutineActionTask()
            {
                CoroutineHelper.StopAllCoroutineActionTask();
            }

            public static Coroutine StartCoroutine(Coroutine routine, Action callBack)
            {
                return CoroutineHelper.StartCoroutine(routine, callBack);
            }

            public static Coroutine StartCoroutine(IEnumerator routine)
            {
                return CoroutineHelper.StartCoroutine(routine);
            }

            public static Coroutine StartCoroutine(Action handler)
            {
                return CoroutineHelper.StartCoroutine(handler);
            }

            public static Coroutine StartCoroutine(Action handler, Action callback)
            {
                return CoroutineHelper.StartCoroutine(handler, callback);
            }

            public static void AddCoroutine(IEnumerator routine, Action<Coroutine> callback)
            {
                CoroutineHelper.AddCoroutine(routine, callback);
            }

            /// <summary>
            /// 延时协程；
            /// </summary>
            /// <param name="delay">延时的时间</param>
            /// <param name="callBack">延时后的回调函数</param>
            /// <returns>协程对象</returns>
            public static Coroutine DelayCoroutine(float delay, Action callBack)
            {
                return CoroutineHelper.DelayCoroutine(delay, callBack);
            }

            /// <summary>
            /// 条件协程；
            /// </summary>
            /// <param name="handler">目标条件</param>
            /// <param name="callBack">条件达成后执行的回调</param>
            /// <returns>协程对象</returns>
            public static Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
            {
                return CoroutineHelper.PredicateCoroutine(handler, callBack);
            }

            /// <summary>
            /// 嵌套协程；
            /// </summary>
            /// <param name="predicateHandler">条件函数</param>
            /// <param name="nestHandler">条件成功后执行的嵌套协程</param>
            /// <returns>Coroutine></returns>
            public static Coroutine PredicateNestCoroutine(Func<bool> predicateHandler, Action nestHandler)
            {
                return CoroutineHelper.PredicateNestCoroutine(predicateHandler, nestHandler);
            }

            public static void StopAllCoroutines()
            {
                CoroutineHelper.StopAllCoroutines();
            }

            public static void StopCoroutine(IEnumerator routine)
            {
                CoroutineHelper.StopCoroutine(routine);
            }

            public static void StopCoroutine(Coroutine routine)
            {
                CoroutineHelper.StopCoroutine(routine);
            }

            #endregion

            #region UnityThread

            public static void PostToUnityThread(Action<object> postCallback)
            {
                UnityThreadSync.Instance.PostToUnityThread(postCallback);
            }

            public static void SendToUnityThread(Action<object> postCallback)
            {
                UnityThreadSync.Instance.SendToUnityThread(postCallback);
            }

            #endregion

            #region Gizmos

            public static void DrawCircleXY(Vector3 position, float radius, int segments, Color color)
            {
                //https://dev-tut.com/2022/unity-draw-a-circle-part2/
                if (radius <= 0.0f || segments <= 0)
                {
                    return;
                }

                float angleStep = 360.0f / segments;

                angleStep *= Mathf.Deg2Rad;

                Vector3 lineStart = Vector3.zero;
                Vector3 lineEnd = Vector3.zero;

                for (int i = 0; i < segments; i++)
                {
                    lineStart.x = Mathf.Cos(angleStep * i);
                    lineStart.y = Mathf.Sin(angleStep * i);

                    lineEnd.x = Mathf.Cos(angleStep * (i + 1));
                    lineEnd.y = Mathf.Sin(angleStep * (i + 1));

                    lineStart *= radius;
                    lineEnd *= radius;

                    lineStart += position;
                    lineEnd += position;

                    UnityEngine.Debug.DrawLine(lineStart, lineEnd, color);
                }
            }

            public static void DrawCircleXZ(Vector3 position, float radius, int segments, Color color)
            {
                //https://dev-tut.com/2022/unity-draw-a-circle-part2/
                if (radius <= 0.0f || segments <= 0)
                {
                    return;
                }

                float angleStep = 360.0f / segments;

                angleStep *= Mathf.Deg2Rad;

                Vector3 lineStart = Vector3.zero;
                Vector3 lineEnd = Vector3.zero;

                for (int i = 0; i < segments; i++)
                {
                    lineStart.x = Mathf.Cos(angleStep * i);
                    lineStart.z = Mathf.Sin(angleStep * i);

                    lineEnd.x = Mathf.Cos(angleStep * (i + 1));
                    lineEnd.z = Mathf.Sin(angleStep * (i + 1));

                    lineStart *= radius;
                    lineEnd *= radius;

                    lineStart += position;
                    lineEnd += position;

                    UnityEngine.Debug.DrawLine(lineStart, lineEnd, color);
                }
            }

            public static void DrawCircleYZ(Vector3 position, float radius, int segments, Color color)
            {
                //https://dev-tut.com/2022/unity-draw-a-circle-part2/
                if (radius <= 0.0f || segments <= 0)
                {
                    return;
                }

                float angleStep = 360.0f / segments;

                angleStep *= Mathf.Deg2Rad;

                Vector3 lineStart = Vector3.zero;
                Vector3 lineEnd = Vector3.zero;

                for (int i = 0; i < segments; i++)
                {
                    lineStart.y = Mathf.Cos(angleStep * i);
                    lineStart.z = Mathf.Sin(angleStep * i);

                    lineEnd.y = Mathf.Cos(angleStep * (i + 1));
                    lineEnd.z = Mathf.Sin(angleStep * (i + 1));

                    lineStart *= radius;
                    lineEnd *= radius;

                    lineStart += position;
                    lineEnd += position;

                    UnityEngine.Debug.DrawLine(lineStart, lineEnd, color);
                }
            }

            public static void DrawEllipseXY(Vector3 center, float xScale, float yScale, int ellipseSegment)
            {
                if (ellipseSegment < 0)
                    ellipseSegment = 0;
                if (xScale < 0)
                    xScale = 0;
                if (yScale < 0)
                    yScale = 0;
                var ellipsePtrs = new Vector3[ellipseSegment];
                for (int i = 0; i < ellipseSegment; i++)
                {
                    var angle = ((float)i / (float)ellipseSegment) * 360 * Mathf.Deg2Rad;
                    var x = Mathf.Sin(angle) * xScale;
                    var y = Mathf.Cos(angle) * yScale;
                    var ptr = new Vector3(x, y, 0);
                    ellipsePtrs[i] = ptr + center;
                }

                for (int i = 0; i < ellipseSegment - 1; i++)
                {
                    Gizmos.DrawLine(ellipsePtrs[i], ellipsePtrs[i + 1]);
                }

                Gizmos.DrawLine(ellipsePtrs[ellipseSegment - 1], ellipsePtrs[0]);
            }

            public static void DrawEllipseXZ(Vector3 center, float xScale, float yScale, int ellipseSegment)
            {
                if (ellipseSegment < 0)
                    ellipseSegment = 0;
                if (xScale < 0)
                    xScale = 0;
                if (yScale < 0)
                    yScale = 0;
                var ellipsePtrs = new Vector3[ellipseSegment];
                for (int i = 0; i < ellipseSegment; i++)
                {
                    var angle = ((float)i / (float)ellipseSegment) * 360 * Mathf.Deg2Rad;
                    var x = Mathf.Sin(angle) * xScale;
                    var y = Mathf.Cos(angle) * yScale;
                    var ptr = new Vector3(x, 0, y);
                    ellipsePtrs[i] = ptr + center;
                }

                for (int i = 0; i < ellipseSegment - 1; i++)
                {
                    Gizmos.DrawLine(ellipsePtrs[i], ellipsePtrs[i + 1]);
                }

                Gizmos.DrawLine(ellipsePtrs[ellipseSegment - 1], ellipsePtrs[0]);
            }

            public static void DrawEllipseYZ(Vector3 center, float xScale, float yScale, int ellipseSegment)
            {
                if (ellipseSegment < 0)
                    ellipseSegment = 0;
                if (xScale < 0)
                    xScale = 0;
                if (yScale < 0)
                    yScale = 0;
                var ellipsePtrs = new Vector3[ellipseSegment];
                for (int i = 0; i < ellipseSegment; i++)
                {
                    var angle = ((float)i / (float)ellipseSegment) * 360 * Mathf.Deg2Rad;
                    var x = Mathf.Sin(angle) * xScale;
                    var y = Mathf.Cos(angle) * yScale;
                    var ptr = new Vector3(0, x, y);
                    ellipsePtrs[i] = ptr + center;
                }

                for (int i = 0; i < ellipseSegment - 1; i++)
                {
                    Gizmos.DrawLine(ellipsePtrs[i], ellipsePtrs[i + 1]);
                }

                Gizmos.DrawLine(ellipsePtrs[ellipseSegment - 1], ellipsePtrs[0]);
            }

            /// <summary>
            /// debug only !
            /// </summary>
            public static void DrawString(string text, Vector3 worldPosition, Color textColor, Vector2 anchor,
                float textSize = 15f)
            {
#if UNITY_EDITOR
                var view = UnityEditor.SceneView.currentDrawingSceneView;
                if (!view)
                    return;
                Vector3 screenPosition = view.camera.WorldToScreenPoint(worldPosition);
                if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || screenPosition.x < 0 ||
                    screenPosition.x > view.camera.pixelWidth || screenPosition.z < 0)
                    return;
                var pixelRatio = UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.right).x -
                                 UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.zero).x;
                UnityEditor.Handles.BeginGUI();
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = (int)textSize,
                    normal = new GUIStyleState() { textColor = textColor }
                };
                Vector2 size = style.CalcSize(new GUIContent(text)) * pixelRatio;
                var alignedPosition =
                    ((Vector2)screenPosition +
                     size * ((anchor + Vector2.left + Vector2.up) / 2f)) * (Vector2.right + Vector2.down) +
                    Vector2.up * view.camera.pixelHeight;
                GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);
                UnityEditor.Handles.EndGUI();
#endif
            }

            #endregion

            #region Math

            /// <summary>
            ///  检测点是否在XY平面的椭圆内
            /// </summary>
            /// <param name="point">检测点</param>
            /// <param name="center">椭圆中心</param>
            /// <param name="radiusX">x的半径</param>
            /// <param name="radiusY">y的半径</param>
            /// <returns>是否包含</returns>
            public static bool IsPointContainedInEllipseXY(Vector3 point, Vector3 center, float radiusX, float radiusY)
            {
                double v = Mathf.Pow(center.x - point.x, 2) / Mathf.Pow(radiusX, 2) +
                           Mathf.Pow(center.y - point.y, 2) / Mathf.Pow(radiusY, 2);
                return v < 1;
            }

            /// <summary>
            ///  检测点是否在XZ平面的椭圆内
            /// </summary>
            /// <param name="point">检测点</param>
            /// <param name="center">椭圆中心</param>
            /// <param name="radiusX">x的半径</param>
            /// <param name="radiusY">y的半径</param>
            /// <returns>是否包含</returns>
            public static bool IsPointContainedInEllipseXZ(Vector3 point, Vector3 center, float radiusX, float radiusY)
            {
                double v = Mathf.Pow(center.x - point.x, 2) / Mathf.Pow(radiusX, 2) +
                           Mathf.Pow(center.z - point.z, 2) / Mathf.Pow(radiusY, 2);
                return v < 1;
            }

            /// <summary>
            ///  检测点是否在YZ平面的椭圆内
            /// </summary>
            /// <param name="point">检测点</param>
            /// <param name="center">椭圆中心</param>
            /// <param name="radiusX">x的半径</param>
            /// <param name="radiusY">y的半径</param>
            /// <returns>是否包含</returns>
            public static bool IsPointContainedInEllipseYZ(Vector3 point, Vector3 center, float radiusX, float radiusY)
            {
                double v = Mathf.Pow(center.y - point.y, 2) / Mathf.Pow(radiusX, 2) +
                           Mathf.Pow(center.z - point.z, 2) / Mathf.Pow(radiusY, 2);
                return v < 1;
            }

            /// <summary>
            /// 获取一个圆内随机点
            /// </summary>
            /// <param name="center">中心点</param>
            /// <param name="radius">半径</param>
            /// <returns>圆内随机点</returns>
            public static Vector2 GetRandomPointInCircle(Vector2 center, float radius)
            {
                if (radius < 0)
                    radius = 0;
                var rndPtr = UnityEngine.Random.insideUnitCircle * radius;
                var rndPos = rndPtr + center;
                return rndPos;
            }

            /// <summary>
            /// 获取一个圆内随机点
            /// </summary>
            /// <param name="center">中心点</param>
            /// <param name="miniRadius">最小半径</param>
            /// <param name="maxRadius">最大半径</param>
            /// <returns>圆内随机点</returns>
            public static Vector2 GetRandomPointInCircle(Vector2 center, float miniRadius, float maxRadius)
            {
                if (miniRadius < 0)
                    miniRadius = 0;
                if (maxRadius < miniRadius)
                    maxRadius = miniRadius;
                var randomRadius = UnityEngine.Random.Range(miniRadius, maxRadius);
                var rndPtr = UnityEngine.Random.insideUnitCircle * randomRadius;
                var rndPos = rndPtr + center;
                return rndPos;
            }

            /// <summary>
            /// 获取一个球内随机点
            /// </summary>
            /// <param name="center">中心点</param>
            /// <param name="radius">半径</param>
            /// <returns>球内随机点</returns>
            public static Vector3 GetRandomPointInSphere(Vector3 center, float radius)
            {
                if (radius < 0)
                    radius = 0;
                var rndPtr = UnityEngine.Random.insideUnitSphere * radius;
                var rndPos = rndPtr + center;
                return rndPos;
            }

            /// <summary>
            /// 获取一个球内随机点
            /// </summary>
            /// <param name="center">中心点</param>
            /// <param name="miniRadius">最小半径</param>
            /// <param name="maxRadius">最大半径</param>
            /// <returns>球内随机点</returns>
            public static Vector3 GetRandomPointInSphere(Vector3 center, float miniRadius, float maxRadius)
            {
                if (miniRadius < 0)
                    miniRadius = 0;
                if (maxRadius < miniRadius)
                    maxRadius = miniRadius;
                var randomRadius = UnityEngine.Random.Range(miniRadius, maxRadius);
                var rndPtr = UnityEngine.Random.insideUnitSphere * randomRadius;
                var rndPos = rndPtr + center;
                return rndPos;
            }

            /// <summary>
            /// 角度转向量 
            /// </summary>
            public static Vector2 AngleToVector2D(float angle)
            {
                float radian = Mathf.Deg2Rad * angle;
                return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
            }

            /// <summary>
            /// 返回两个向量的夹角
            /// </summary>
            public static float VectorAngle(Vector2 lhs, Vector2 rhs)
            {
                float angle;
                Vector3 cross = Vector3.Cross(lhs, rhs);
                angle = Vector2.Angle(lhs, rhs);
                return cross.z > 0 ? -angle : angle;
            }

            /// <summary>
            /// 是否约等于另一个浮点数
            /// </summary>
            public static bool Approximately(float sourceValue, float targetValue)
            {
                return Mathf.Approximately(sourceValue, targetValue);
            }

            /// <summary>
            /// 限制一个向量在最大值与最小值之间
            /// </summary>
            public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
            {
                value.x = Mathf.Clamp(value.x, min.x, max.x);
                value.y = Mathf.Clamp(value.y, min.y, max.y);
                value.z = Mathf.Clamp(value.z, min.z, max.z);
                return value;
            }

            public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
            {
                value.x = Mathf.Clamp(value.x, min.x, max.x);
                value.y = Mathf.Clamp(value.y, min.y, max.y);
                return value;
            }

            /// <summary>
            /// 获得固定位数小数的向量
            /// </summary>
            public static Vector3 Round(Vector3 value, int decimals)
            {
                value.x = (float)Math.Round(value.x, decimals);
                value.y = (float)Math.Round(value.y, decimals);
                value.z = (float)Math.Round(value.z, decimals);
                return value;
            }

            /// <summary>
            /// 限制一个向量在最大值与最小值之间
            /// </summary>
            public static Vector3 Clamp(Vector3 value, float minX, float minY, float minZ, float maxX, float maxY,
                float maxZ)
            {
                value.x = Mathf.Clamp(value.x, minX, maxX);
                value.y = Mathf.Clamp(value.y, minY, maxY);
                value.z = Mathf.Clamp(value.z, minZ, maxZ);
                return value;
            }

            public static Vector2 Clamp(Vector2 value, float minX, float minY, float maxX, float maxY)
            {
                value.x = Mathf.Clamp(value.x, minX, maxX);
                value.y = Mathf.Clamp(value.y, minY, maxY);
                return value;
            }

            /// <summary>
            /// 获得固定位数小数的向量
            /// </summary>
            public static Vector2 Round(Vector2 value, int decimals)
            {
                value.x = (float)Math.Round(value.x, decimals);
                value.y = (float)Math.Round(value.y, decimals);
                return value;
            }

            #endregion

            #region GameObject

            /// <summary>
            /// 清除单个实例，默认延迟为0，仅在场景中删除对应对象
            /// </summary>
            public static void DestroyObject(Object obj, float delay = 0)
            {
                GameObject.Destroy(obj, delay);
            }

            /// <summary>
            /// 立刻清理实例对象，会在内存中清理实例，Editor适用
            /// </summary>
            public static void DestroyObjectImmediate(Object obj)
            {
                GameObject.DestroyImmediate(obj);
            }

            /// <summary>
            /// 清除一组实例
            /// </summary>
            /// <typeparam name="T">实例类型</typeparam>
            /// <param name="objs">对象实例集合</param>
            public static void DestroyObjects<T>(IEnumerable<T> objs) where T : Object
            {
                foreach (var obj in objs)
                {
                    GameObject.Destroy(obj);
                }
            }

            #endregion

            #region Other

            /// <summary>
            /// 场景是否被加载；
            /// </summary>
            /// <param name="sceneName">场景名</param>
            /// <returns>是否被加载</returns>
            public static bool IsSceneLoaded(string sceneName)
            {
                var scene = SceneManager.GetSceneByName(sceneName);
                if (scene != null)
                {
                    return scene.isLoaded;
                }

                return false;
            }

            /// <summary>
            /// 判断是否是路径；
            /// 需要注意根目录下的文件可能不带/或\符号！
            /// </summary>
            /// <param name="path">路径str</param>
            /// <returns>是否是路径</returns>
            public static bool IsPath(string path)
            {
                return path.Contains("\\") || path.Contains("/");
            }

            #endregion
        }
    }
}