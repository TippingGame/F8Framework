using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LitJson
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class LitJsonRegister
    {
        static bool _registerd;
        
        static LitJsonRegister()
        {
            Register();
        }
        
        public static void Register()
        {
            if (_registerd) return;
            _registerd = true;

            // 注册Type类型的Exporter
            JsonMapper.RegisterExporter<Type>((v, w) =>
            {
                w.Write(v.FullName);
            });

            JsonMapper.RegisterImporter<string, Type>((s) =>
            {
                return Type.GetType(s);
            });

            // 注册Vector2类型的Exporter
            Action<Vector2, JsonWriter> writeVector2 = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteObjectEnd();
            };

            JsonMapper.RegisterExporter<Vector2>((v, w) =>
            {
                writeVector2(v, w);
            });

            // 注册Vector3类型的Exporter
            Action<Vector3, JsonWriter> writeVector3 = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteObjectEnd();
            };
            
            // Quaternion
            Action<Quaternion, JsonWriter> writeQuaternion = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            };
            
            // Transform
            Action<Transform, JsonWriter> writeTransform = (v, w) =>
            {
                w.WriteObjectStart();
                var localPosition = v.localPosition;
                w.WriteProperty("xPos", localPosition.x);
                w.WriteProperty("yPos", localPosition.y);
                w.WriteProperty("zPos", localPosition.z);
                var localRotation = v.localRotation;
                w.WriteProperty("xQua", localRotation.x);
                w.WriteProperty("yQua", localRotation.y);
                w.WriteProperty("zQua", localRotation.z);
                w.WriteProperty("wQua", localRotation.x);
                var localScale = v.localScale;
                w.WriteProperty("xScale", localScale.y);
                w.WriteProperty("yScale", localScale.y);
                w.WriteProperty("zScale", localScale.z);
                w.WriteObjectEnd();
            };

            JsonMapper.RegisterExporter<Vector3>((v, w) =>
            {
                writeVector3(v, w);
            });

            // 注册Vector4类型的Exporter
            JsonMapper.RegisterExporter<Vector4>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });

            // 注册Quaternion类型的Exporter
            JsonMapper.RegisterExporter<Quaternion>((v, w) =>
            {
                writeQuaternion(v, w);
            });

            // 注册Color类型的Exporter
            JsonMapper.RegisterExporter<Color>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });

            // 注册Color32类型的Exporter
            JsonMapper.RegisterExporter<Color32>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });

            // 注册Bounds类型的Exporter
            JsonMapper.RegisterExporter<Bounds>((v, w) =>
            {
                w.WriteObjectStart();

                w.WritePropertyName("center");
                writeVector3(v.center, w);

                w.WritePropertyName("size");
                writeVector3(v.size, w);

                w.WriteObjectEnd();
            });

            // 注册Rect类型的Exporter
            JsonMapper.RegisterExporter<Rect>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("width", v.width);
                w.WriteProperty("height", v.height);
                w.WriteObjectEnd();
            });

            // 注册RectOffset类型的Exporter
            JsonMapper.RegisterExporter<RectOffset>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("top", v.top);
                w.WriteProperty("left", v.left);
                w.WriteProperty("bottom", v.bottom);
                w.WriteProperty("right", v.right);
                w.WriteObjectEnd();
            });

            // 注册GameObject类型的Exporter
            JsonMapper.RegisterExporter<GameObject>((v, w) =>
            {
                w.WriteObjectStart();
                if (v != null)
                {
                    w.WriteProperty("name", v.name);
                    w.WriteProperty("active", v.activeSelf);
                    w.WriteProperty("tag", v.tag);
                    w.WriteProperty("layer", v.layer);
                    w.WriteProperty("isStatic", v.isStatic);
                    if (v.transform)
                    {
                        var localPosition = v.transform.localPosition;
                        w.WriteProperty("xPos", localPosition.x);
                        w.WriteProperty("yPos", localPosition.y);
                        w.WriteProperty("zPos", localPosition.z);
                        var localRotation = v.transform.localRotation;
                        w.WriteProperty("xQua", localRotation.x);
                        w.WriteProperty("yQua", localRotation.y);
                        w.WriteProperty("zQua", localRotation.z);
                        w.WriteProperty("wQua", localRotation.x);
                        var localScale = v.transform.localScale;
                        w.WriteProperty("xScale", localScale.y);
                        w.WriteProperty("yScale", localScale.y);
                        w.WriteProperty("zScale", localScale.z);
                    }
                }
                w.WriteObjectEnd();
            });

            // Transform
            JsonMapper.RegisterExporter<Transform>((v, w) =>
            {
                writeTransform(v, w);
            });
            
            // LayerMask
            JsonMapper.RegisterExporter<LayerMask>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("value", v.value);
                w.WriteObjectEnd();
            });
            
#if UNITY_2017_2_OR_NEWER
            // 注册Vector3Int类型的Exporter
            Action<Vector3Int, JsonWriter> writeVector3Int = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteObjectEnd();
            };
            
            // Vector2Int
            JsonMapper.RegisterExporter<Vector2Int>((v, w) => {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteObjectEnd();
            });
            
            // 注册Vector3Int类型的Exporter
            JsonMapper.RegisterExporter<Vector3Int>((v, w) => {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteObjectEnd();
            });
            
            // RangeInt
            JsonMapper.RegisterExporter<RangeInt>((v, w) => {
                w.WriteObjectStart();
                w.WriteProperty("start", v.start);
                w.WriteProperty("length", v.length);
                w.WriteProperty("end", v.end);
                w.WriteObjectEnd();
            });
            
            // BoundsInt
            JsonMapper.RegisterExporter<BoundsInt>((v, w) =>
            {
                w.WriteObjectStart();

                w.WritePropertyName("center");
                writeVector3(v.center, w);

                w.WritePropertyName("size");
                writeVector3Int(v.size, w);

                w.WriteObjectEnd();
            });
            
            // 注册Tile类型的Exporter
            JsonMapper.RegisterExporter<Tile>((v, w) => {
                w.WriteObjectStart();
                // TODO
                w.WriteObjectEnd();
            });
#endif
        }
    }
}
