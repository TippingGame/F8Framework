using System.Collections.Generic;
using UnityEngine;
using System;

namespace F8Framework.Core
{
    public static class TweenPool
    {
        static List<ValueTween> valueTweens = new List<ValueTween>();
        static List<Vector2Tween> vector2Tweens = new List<Vector2Tween>();
        static List<Vector3Tween> vector3Tweens = new List<Vector3Tween>();
        static List<ColorTween> colorTweens = new List<ColorTween>();
        static List<QuaternionTween> quaternionTweens = new List<QuaternionTween>();

        static int counter = 0;

        private static int GenerateId()
        {
            try
            {
                counter++;
            }
            catch (OverflowException)
            {
                counter = 0;
            }

            return counter;
        }

        public static void AddTweenToPool(BaseTween tween)
        {
            switch (tween)
            {
                case ValueTween valueTween:
                    valueTweens.Add(valueTween);
                    break;
                case Vector2Tween vector2Tween:
                    vector2Tweens.Add(vector2Tween);
                    break;
                case Vector3Tween vector3Tween:
                    vector3Tweens.Add(vector3Tween);
                    break;
                case ColorTween colorTween:
                    colorTweens.Add(colorTween);
                    break;
                case QuaternionTween quaternionTween:
                    quaternionTweens.Add(quaternionTween);
                    break;
                default:
                    break;
            }
        }

        private static bool TryGetTween<T>(List<T> list, out T tween) where T : BaseTween
        {
            if (list.Count > 0)
            {
                int last = list.Count - 1;
                tween = list[last];
                list.RemoveAt(last);
                return true;
            }
            else
            {
                tween = null;
                return false;
            }
        }

        public static ValueTween GetValueTween(float start, float end, float t)
        {
            ValueTween tween;
            if (TryGetTween(valueTweens, out tween))
            {
                tween.Reset();
                tween.Init(start, end, t);
                tween.ID = GenerateId();
            }
            else
            {
                tween = new ValueTween(start, end, t, GenerateId());
            }
            Tween.Instance.tweens.Add(tween);
            return tween;
        }

        internal static Vector3Tween GetVector3Tween(Vector3 from, Vector3 to, float time)
        {
            Vector3Tween tween;
            if (TryGetTween(vector3Tweens, out tween))
            {
                tween.Reset();
                tween.Init(from, to, time);
                tween.ID = GenerateId();
            }
            else
            {
                tween = new Vector3Tween(from, to, time, GenerateId());
            }
            Tween.Instance.tweens.Add(tween);
            return tween;
        }

        internal static Vector2Tween GetVector2Tween(Vector2 from, Vector2 to, float t)
        {
            Vector2Tween tween;
            if (TryGetTween(vector2Tweens, out tween))
            {
                tween.Reset();
                tween.Init(from, to, t);
                tween.ID = GenerateId();
            }
            else
            {
                tween = new Vector2Tween(from, to, t, GenerateId());
            }
            Tween.Instance.tweens.Add(tween);
            return tween;
        }

        internal static ColorTween GetColorTween(Color from, Color to, float t)
        {
            ColorTween tween;
            if (TryGetTween(colorTweens, out tween))
            {
                tween.Reset();
                tween.Init(from, to, t);
                tween.ID = GenerateId();
            }
            else
            {
                tween = new ColorTween(from, to, t, GenerateId());
            }
            Tween.Instance.tweens.Add(tween);
            return tween;
        }

        internal static QuaternionTween GetQuaternionTween(Quaternion from, Quaternion to, float t)
        {
            QuaternionTween tween;
            if (TryGetTween(quaternionTweens , out tween))
            {
                tween.Reset();
                tween.Init(from, to, t);
                tween.ID = GenerateId();
            }
            else
            {
                tween = new QuaternionTween(from, to, t, GenerateId());
            }
            Tween.Instance.tweens.Add(tween);
            return tween;
        }

        public static void RemoveTween(BaseTween tween)
        {
            Tween.Instance.tweens.Remove(tween);
        }
    }
}
