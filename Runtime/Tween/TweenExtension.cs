using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public static class TweenExtension
    {
        #region SCALE_TWEENS
        
        public static BaseTween ScaleTween(this Transform t, Vector3 to, float time)
        {
            return Tween.Instance.ScaleTween(t, to, time);
        }

        public static BaseTween ScaleTween(this GameObject go, Vector3 to, float time)
        {
            return Tween.Instance.ScaleTween(go.transform, to, time);
        }

        public static BaseTween ScaleTween(this RectTransform rect, Vector3 to, float time)
        {
            return Tween.Instance.ScaleTween(rect, to, time);
        }
        
        #endregion
        
        #region SCALE_TWEENS_AT_SPEED
        
        public static BaseTween ScaleAtSpeed(this Transform t, Vector3 to, float speed)
        {
            return Tween.Instance.ScaleTweenAtSpeed(t, to, speed);
        }

        public static BaseTween ScaleAtSpeed(this GameObject go, Vector3 to, float speed)
        {
            return Tween.Instance.ScaleTweenAtSpeed(go, to, speed);
        }

        public static BaseTween ScaleAtSpeed(this RectTransform rect, Vector3 to, float speed)
        {
            return Tween.Instance.ScaleTweenAtSpeed(rect, to, speed);
        }
        
        #endregion
        
        #region SCALE_AXIS_TWEENS
        
        public static BaseTween ScaleX(this Transform obj, float value, float t)
        {
            return Tween.Instance.ScaleX(obj, value, t);
        }

        public static BaseTween ScaleX(this GameObject obj, float value, float t)
        {
            return Tween.Instance.ScaleX(obj, value, t);
        }

        public static BaseTween ScaleX(this RectTransform obj, float value, float t)
        {
            return Tween.Instance.ScaleX(obj, value, t);
        }

        public static BaseTween ScaleY(this Transform obj, float value, float t)
        {
            return Tween.Instance.ScaleY(obj, value, t);
        }

        public static BaseTween ScaleY(this GameObject obj, float value, float t)
        {
            return Tween.Instance.ScaleY(obj, value, t);
        }

        public static BaseTween ScaleY(this RectTransform obj, float value, float t)
        {
            return Tween.Instance.ScaleY(obj, value, t);
        }

        public static BaseTween ScaleZ(this Transform obj, float value, float t)
        {
            return Tween.Instance.ScaleZ(obj, value, t);
        }

        public static BaseTween ScaleZ(this GameObject obj, float value, float t)
        {
            return Tween.Instance.ScaleZ(obj, value, t);
        }

        public static BaseTween ScaleZ(this RectTransform obj, float value, float t)
        {
            return Tween.Instance.ScaleZ(obj, value, t);
        }
        
        #endregion
        
        #region SCALE_AXIS_TWEENS_AT_SPEED
        
        public static BaseTween ScaleXAtSpeed(this Transform obj, float value, float speed)
        {
            return Tween.Instance.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleXAtSpeed(this GameObject obj, float value, float speed)
        {
            return Tween.Instance.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleXAtSpeed(this RectTransform obj, float value, float speed)
        {
            return Tween.Instance.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleYAtSpeed(this Transform obj, float value, float speed)
        {
            return Tween.Instance.ScaleYAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleYAtSpeed(this GameObject obj, float value, float speed)
        {
            return Tween.Instance.ScaleYAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleYAtSpeed(this RectTransform obj, float value, float speed)
        {
            return Tween.Instance.ScaleYAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleZAtSpeed(this Transform obj, float value, float speed)
        {
            return Tween.Instance.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleZAtSpeed(this GameObject obj, float value, float speed)
        {
            return Tween.Instance.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleZAtSpeed(this RectTransform obj, float value, float speed)
        {
            return Tween.Instance.ScaleXAtSpeed(obj, value, speed);
        }
        
        #endregion
        
        #region ROTATE_TWEENS
        
        public static BaseTween RotateTween(this Transform t, Vector3 axis, float to, float time)
        {
            return Tween.Instance.RotateTween(t, axis, to, time);
        }

        public static BaseTween RotateTween(this Transform t, Vector3 to, float time)
        {
            return Tween.Instance.RotateTween(t, to, time);
        }

        public static BaseTween RotateTween(this Transform t, Quaternion to, float time)
        {
            return Tween.Instance.RotateTween(t, to, time);
        }

        public static BaseTween RotateTween(this GameObject go, Vector3 to, float time)
        {
            return Tween.Instance.RotateTween(go, to, time);
        }

        public static BaseTween RotateTween(this GameObject go, Vector3 axis, float to, float time)
        {
            return Tween.Instance.RotateTween(go, axis, to, time);
        }

        public static BaseTween RotateTween(this GameObject go, Quaternion to, float time)
        {
            return Tween.Instance.RotateTween(go, to, time);
        }

        public static BaseTween RotateTween(this RectTransform rect, Vector3 to, float time)
        {
            return Tween.Instance.RotateTween(rect, to, time);
        }

        public static BaseTween RotateTween(this RectTransform rect, Quaternion to, float time)
        {
            return Tween.Instance.RotateTween(rect, to, time);
        }

        public static BaseTween RotateTween(this RectTransform rect, Vector3 axis, float to, float time)
        {
            return Tween.Instance.RotateTween(rect, axis, to, time);
        }
        
        #endregion
        
        #region ROTATE_TWEEN_AT_SPEED
        
        public static BaseTween RotateTweenAtSpeed(this Transform t, Quaternion to, float speed)
        {
            return Tween.Instance.RotateTweenAtSpeed(t, to, speed);
        }

        public static BaseTween RotateTweenAtSpeed(this Transform t, Vector3 to, float speed)
        {
            return Tween.Instance.RotateTweenAtSpeed(t, to, speed);
        }

        public static BaseTween RotateTweenAtSpeed(this Transform t, Vector3 axis, float toAngle, float speed)
        {
            return Tween.Instance.RotateTweenAtSpeed(t, axis, toAngle, speed);
        }

        public static BaseTween RotateTweenAtSpeed(this GameObject go, Quaternion to, float speed)
        {
            return Tween.Instance.RotateTweenAtSpeed(go, to, speed);
        }

        public static BaseTween RotateTweenAtSpeed(this GameObject go, Vector3 to, float speed)
        {
            return Tween.Instance.RotateTweenAtSpeed(go, to, speed);
        }

        public static BaseTween RotateTweenAtSpeed(this GameObject go, Vector3 axis, float toAngle, float speed)
        {
            return Tween.Instance.RotateTweenAtSpeed(go, axis, toAngle, speed);
        }

        public static BaseTween RotateTweenAtSpeed(this RectTransform rect, Quaternion to, float speed)
        {
            return Tween.Instance.RotateTweenAtSpeed(rect, to, speed);
        }

        public static BaseTween RotateTweenAtSpeed(this RectTransform rect, Vector3 to, float speed)
        {
            return Tween.Instance.RotateTweenAtSpeed(rect, to, speed);
        }

        public static BaseTween RotateTweenAtSpeed(this RectTransform rect, Vector3 axis, float toAngle, float speed)
        {
            return Tween.Instance.RotateTweenAtSpeed(rect, axis, toAngle, speed);
        }
        
        #endregion
        
        #region LOCAL_ROTATE_TWEENS
        
        public static BaseTween LocalRotateTween(this Transform t, Vector3 axis, float to, float time)
        {
            return Tween.Instance.LocalRotateTween(t, axis, to, time);
        }

        public static BaseTween LocalRotateTween(this Transform t, Vector3 to, float time)
        {
            return Tween.Instance.LocalRotateTween(t, to, time);
        }

        public static BaseTween LocalRotateTween(this Transform t, Quaternion to, float time)
        {
            return Tween.Instance.LocalRotateTween(t, to, time);
        }

        public static BaseTween LocalRotateTween(this GameObject go, Vector3 to, float time)
        {
            return Tween.Instance.LocalRotateTween(go, to, time);
        }

        public static BaseTween LocalRotateTween(this GameObject go, Vector3 axis, float to, float time)
        {
            return Tween.Instance.LocalRotateTween(go, axis, to, time);
        }

        public static BaseTween LocalRotateTween(this GameObject go, Quaternion to, float time)
        {
            return Tween.Instance.LocalRotateTween(go, to, time);
        }

        public static BaseTween LocalRotateTween(this RectTransform rect, Vector3 to, float time)
        {
            return Tween.Instance.LocalRotateTween(rect, to, time);
        }

        public static BaseTween LocalRotateTween(this RectTransform rect, Quaternion to, float time)
        {
            return Tween.Instance.LocalRotateTween(rect, to, time);
        }

        public static BaseTween LocalRotateTween(this RectTransform rect, Vector3 axis, float to, float time)
        {
            return Tween.Instance.LocalRotateTween(rect, axis, to, time);
        }
        
        #endregion
        
        #region LOCAL_ROTATE_TWEEN_AT_SPEED
        
        public static BaseTween LocalRotateTweenAtSpeed(this Transform t, Quaternion to, float speed)
        {
            return Tween.Instance.LocalRotateTweenAtSpeed(t, to, speed);
        }

        public static BaseTween LocalRotateTweenAtSpeed(this Transform t, Vector3 to, float speed)
        {
            return Tween.Instance.LocalRotateTweenAtSpeed(t, to, speed);
        }

        public static BaseTween LocalRotateTweenAtSpeed(this Transform t, Vector3 axis, float toAngle, float speed)
        {
            return Tween.Instance.LocalRotateTweenAtSpeed(t, axis, toAngle, speed);
        }

        public static BaseTween LocalRotateTweenAtSpeed(this GameObject go, Quaternion to, float speed)
        {
            return Tween.Instance.LocalRotateTweenAtSpeed(go, to, speed);
        }

        public static BaseTween LocalRotateTweenAtSpeed(this GameObject go, Vector3 to, float speed)
        {
            return Tween.Instance.LocalRotateTweenAtSpeed(go, to, speed);
        }

        public static BaseTween LocalRotateTweenAtSpeed(this GameObject go, Vector3 axis, float toAngle, float speed)
        {
            return Tween.Instance.LocalRotateTweenAtSpeed(go, axis, toAngle, speed);
        }

        public static BaseTween LocalRotateTweenAtSpeed(this RectTransform rect, Quaternion to, float speed)
        {
            return Tween.Instance.LocalRotateTweenAtSpeed(rect, to, speed);
        }

        public static BaseTween LocalRotateTweenAtSpeed(this RectTransform rect, Vector3 to, float speed)
        {
            return Tween.Instance.LocalRotateTweenAtSpeed(rect, to, speed);
        }

        public static BaseTween LocalRotateTweenAtSpeed(this RectTransform rect, Vector3 axis, float toAngle,
            float speed)
        {
            return Tween.Instance.LocalRotateTweenAtSpeed(rect, axis, toAngle, speed);
        }
        
        #endregion
        
        #region EULERANGLES_TWEENS
        
        public static BaseTween EulerAnglesTween(this Transform t, Vector3 to, float time)
        {
            return Tween.Instance.EulerAnglesTween(t, to, time);
        }

        public static BaseTween EulerAnglesTween(this GameObject go, Vector3 to, float time)
        {
            return Tween.Instance.EulerAnglesTween(go, to, time);
        }

        public static BaseTween EulerAnglesTween(this RectTransform rect, Vector3 to, float time)
        {
            return Tween.Instance.EulerAnglesTween(rect, to, time);
        }
        
        #endregion
        
        #region EULERANGLES_TWEEN_AT_SPEED
        
        public static BaseTween EulerAnglesTweenAtSpeed(this Transform t, Vector3 to, float speed)
        {
            return Tween.Instance.EulerAnglesTweenAtSpeed(t, to, speed);
        }

        public static BaseTween EulerAnglesTweenAtSpeed(this GameObject go, Vector3 to, float speed)
        {
            return Tween.Instance.EulerAnglesTweenAtSpeed(go, to, speed);
        }

        public static BaseTween EulerAnglesTweenAtSpeed(this RectTransform rect, Vector3 to, float speed)
        {
            return Tween.Instance.EulerAnglesTweenAtSpeed(rect, to, speed);
        }
        
        #endregion
        
        #region LOCAL_EULERANGLES_TWEENS
        
        public static BaseTween LocalEulerAnglesTween(this Transform t, Vector3 to, float time)
        {
            return Tween.Instance.LocalEulerAnglesTween(t, to, time);
        }

        public static BaseTween LocalEulerAnglesTween(this GameObject go, Vector3 to, float time)
        {
            return Tween.Instance.LocalEulerAnglesTween(go, to, time);
        }

        public static BaseTween LocalEulerAnglesTween(this RectTransform rect, Vector3 to, float time)
        {
            return Tween.Instance.LocalEulerAnglesTween(rect, to, time);
        }
        
        #endregion
        
        #region LOCAL_EULERANGLES_TWEEN_AT_SPEED
        
        public static BaseTween LocalEulerAnglesTweenAtSpeed(this Transform t, Vector3 to, float speed)
        {
            return Tween.Instance.LocalEulerAnglesTweenAtSpeed(t, to, speed);
        }

        public static BaseTween LocalEulerAnglesTweenAtSpeed(this GameObject go, Vector3 to, float speed)
        {
            return Tween.Instance.LocalEulerAnglesTweenAtSpeed(go, to, speed);
        }

        public static BaseTween LocalEulerAnglesTweenAtSpeed(this RectTransform rect, Vector3 to, float speed)
        {
            return Tween.Instance.LocalEulerAnglesTweenAtSpeed(rect, to, speed);
        }
        
        #endregion
        
        #region FADE_TWEENS_CANVAS_GROUP
        
        public static BaseTween Fade(this CanvasGroup cg, float to, float t)
        {
            return Tween.Instance.Fade(cg, to, t);
        }

        public static BaseTween FadeIn(this CanvasGroup cg, float t)
        {
            return Tween.Instance.FadeIn(cg, t);
        }

        public static BaseTween FadeOut(this CanvasGroup cg, float t)
        {
            return Tween.Instance.FadeOut(cg, t);
        }
        
        #endregion
        
        #region FADE_TWEENS_CANVAS_GROUP_AT_SPEED
        
        public static BaseTween FadeAtSpeed(this CanvasGroup cg, float to, float speed)
        {
            return Tween.Instance.FadeAtSpeed(cg, to, speed);
        }

        public static BaseTween FadeInAtSpeed(this CanvasGroup cg, float speed)
        {
            return Tween.Instance.FadeInAtSpeed(cg, speed);
        }

        public static BaseTween FadeOutAtSpeed(this CanvasGroup cg, float speed)
        {
            return Tween.Instance.FadeOutAtSpeed(cg, speed);
        }
        
        #endregion
        
        #region FADE_TWEENS_IMAGE
        
        public static BaseTween Fade(this Image image, float to, float t)
        {
            return Tween.Instance.Fade(image, to, t);
        }

        public static BaseTween FadeIn(this Image image, float t)
        {
            return Tween.Instance.FadeIn(image, t);
        }

        public static BaseTween FadeOut(this Image image, float t)
        {
            return Tween.Instance.FadeOut(image, t);
        }
        
        #endregion
        
        #region FADE_TWEENS_IMAGE_AT_SPEED
        
        public static BaseTween FadeAtSpeed(this Image img, float to, float speed)
        {
            return Tween.Instance.FadeAtSpeed(img, to, speed);
        }

        public static BaseTween FadeInAtSpeed(this Image img, float speed)
        {
            return Tween.Instance.FadeInAtSpeed(img, speed);
        }

        public static BaseTween FadeOutAtSpeed(this Image img, float speed)
        {
            return Tween.Instance.FadeOutAtSpeed(img, speed);
        }
        
        #endregion
        
        #region FADE_TWEENS_SPRITE_RENDERER
        
        public static BaseTween Fade(this SpriteRenderer sprite, float to, float t)
        {
            return Tween.Instance.Fade(sprite, to, t);
        }

        public static BaseTween FadeIn(this SpriteRenderer sprite, float t)
        {
            return Tween.Instance.FadeIn(sprite, t);
        }

        public static BaseTween FadeOut(this SpriteRenderer sprite, float t)
        {
            return Tween.Instance.FadeOut(sprite, t);
        }
        
        #endregion
        
        #region FADE_TWEENS_SPRITE_RENDERER_AT_SPEED
        
        public static BaseTween FadeAtSpeed(this SpriteRenderer sprite, float to, float speed)
        {
            return Tween.Instance.FadeAtSpeed(sprite, to, speed);
        }

        public static BaseTween FadeInAtSpeed(this SpriteRenderer sprite, float speed)
        {
            return Tween.Instance.FadeInAtSpeed(sprite, speed);
        }

        public static BaseTween FadeOutAtSpeed(this SpriteRenderer sprite, float speed)
        {
            return Tween.Instance.FadeOutAtSpeed(sprite, speed);
        }
        
        #endregion
        
        #region COLOR_TWEENS
        
        public static BaseTween ColorTween(this Material material, Color to, float t)
        {
            return Tween.Instance.ColorTween(material, to, t);
        }

        public static BaseTween ColorTween(this SpriteRenderer sprite, Color to, float t)
        {
            return Tween.Instance.ColorTween(sprite, to, t);
        }

        public static BaseTween ColorTween(this Image image, Color to, float t)
        {
            return Tween.Instance.ColorTween(image, to, t);
        }
        
        #endregion
        
        #region COLOR_TWEENS_AT_SPEED
        
        public static BaseTween ColorTweenAtSpeed(this Material material, Color to, float speed)
        {
            return Tween.Instance.ColorTweenAtSpeed(material, to, speed);
        }

        public static BaseTween ColorTweenAtSpeed(this SpriteRenderer sprite, Color to, float speed)
        {
            return Tween.Instance.ColorTweenAtSpeed(sprite, to, speed);
        }

        public static BaseTween ColorTweenAtSpeed(this Image img, Color to, float speed)
        {
            return Tween.Instance.ColorTweenAtSpeed(img, to, speed);
        }
        
        #endregion
        
        #region FILL_AMOUNT_TWEENS
        
        public static BaseTween FillAmountTween(this Image img, float to, float t)
        {
            return Tween.Instance.FillAmountTween(img, to, t);
        }
        
        #endregion
        
        #region FILL_AMOUNT_TWEENS_AT_SPEED
        
        public static BaseTween FillAmountTweenAtSpeed(this Image img, float to, float speed)
        {
            return Tween.Instance.FillAmountTween(img, to, speed);
        }
        
        #endregion
        
        #region MOVE_TWEENS_TRANSFORM
        
        public static BaseTween Move(this Transform obj, Transform to, float t)
        {
            return Tween.Instance.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this Transform obj, Transform to, float t)
        {
            return Tween.Instance.LocalMove(obj, to, t);
        }

        public static BaseTween Move(this Transform obj, Vector3 to, float t)
        {
            return Tween.Instance.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this Transform obj, Vector3 to, float t)
        {
            return Tween.Instance.LocalMove(obj, to, t);
        }

        public static BaseTween Move(this Transform obj, GameObject to, float t)
        {
            return Tween.Instance.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this Transform obj, GameObject to, float t)
        {
            return Tween.Instance.LocalMove(obj, to, t);
        }
        
        #endregion
        
        #region MOVE_TWEENS_TRANSFORM_AT_SPEED
        
        public static BaseTween MoveAtSpeed(this Transform obj, Transform to, float speed)
        {
            return Tween.Instance.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this Transform obj, Transform to, float speed)
        {
            return Tween.Instance.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween MoveAtSpeed(this Transform obj, Vector3 to, float speed)
        {
            return Tween.Instance.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this Transform obj, Vector3 to, float speed)
        {
            return Tween.Instance.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween MoveAtSpeed(this Transform obj, GameObject to, float speed)
        {
            return Tween.Instance.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this Transform obj, GameObject to, float speed)
        {
            return Tween.Instance.LocalMoveAtSpeed(obj, to, speed);
        }
        
        #endregion
        
        #region MOVE_TWEENS_GAMEOBJECT
        
        public static BaseTween Move(this GameObject obj, Transform to, float t)
        {
            return Tween.Instance.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this GameObject obj, Transform to, float t)
        {
            return Tween.Instance.LocalMove(obj, to, t);
        }

        public static BaseTween Move(this GameObject obj, Vector3 to, float t)
        {
            return Tween.Instance.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this GameObject obj, Vector3 to, float t)
        {
            return Tween.Instance.LocalMove(obj, to, t);
        }

        public static BaseTween Move(this GameObject obj, GameObject to, float t)
        {
            return Tween.Instance.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this GameObject obj, GameObject to, float t)
        {
            return Tween.Instance.LocalMove(obj, to, t);
        }
        
        #endregion
        
        #region MOVE_TWEENS_GAMEOBJECT_AT_SPEED
        
        public static BaseTween MoveAtSpeed(this GameObject obj, Transform to, float speed)
        {
            return Tween.Instance.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this GameObject obj, Transform to, float speed)
        {
            return Tween.Instance.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween MoveAtSpeed(this GameObject obj, Vector3 to, float speed)
        {
            return Tween.Instance.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this GameObject obj, Vector3 to, float speed)
        {
            return Tween.Instance.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween MoveAtSpeed(this GameObject obj, GameObject to, float speed)
        {
            return Tween.Instance.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this GameObject obj, GameObject to, float speed)
        {
            return Tween.Instance.LocalMoveAtSpeed(obj, to, speed);
        }
        
        #endregion
        
        #region UI_MOVE_TWEENS
        
        public static BaseTween Move(this RectTransform rect, Vector2 pos, float t)
        {
            return Tween.Instance.Move(rect, pos, t);
        }

        public static BaseTween MoveUI(this RectTransform rect, Vector2 absolutePosition, RectTransform canvas, float t,
            PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            return Tween.Instance.MoveUI(rect, absolutePosition, canvas, t, pivotPreset);
        }

        public static BaseTween TranslateUI(this RectTransform rect, Vector2 translation, RectTransform canvas, float t,
            PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            return Tween.Instance.TranslateUI(rect, translation, canvas, t, pivotPreset);
        }
        
        #endregion
        
        #region UI_MOVE_TWEENS_AT_SPEED
        
        public static BaseTween MoveAtSpeed(this RectTransform rect, Vector2 pos, float speed)
        {
            return Tween.Instance.MoveAtSpeed(rect, pos, speed);
        }

        public static BaseTween MoveUIAtSpeed(this RectTransform rect, Vector2 absolutePosition, RectTransform canvas,
            float speed, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            return Tween.Instance.MoveUIAtSpeed(rect, absolutePosition, canvas, speed, pivotPreset);
        }

        public static BaseTween TranslateUIAtSpeed(this RectTransform rect, Vector2 translation, RectTransform canvas,
            float speed, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            return Tween.Instance.TranslateUIAtSpeed(rect, translation, canvas, speed, pivotPreset);
        }
        
        #endregion
        
        #region SHAKE_TWEENS
        
        public static Sequence ShakePosition(this Transform obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return Tween.Instance.ShakePosition(obj, vibrato, shakeCount, t, fadeOut);
        }

        public static Sequence ShakePosition(this GameObject obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return Tween.Instance.ShakePosition(obj.transform, vibrato, shakeCount, t, fadeOut);
        }

        public static Sequence ShakeRotation(this Transform obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return Tween.Instance.ShakeRotation(obj, vibrato, shakeCount, t, fadeOut);
        }

        public static Sequence ShakeRotation(this GameObject obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return Tween.Instance.ShakeRotation(obj.transform, vibrato, shakeCount, t, fadeOut);
        }

        public static Sequence ShakeScale(this Transform obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return Tween.Instance.ShakeScale(obj, vibrato, shakeCount, t, fadeOut);
        }

        public static Sequence ShakeScale(this GameObject obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return Tween.Instance.ShakeScale(obj.transform, vibrato, shakeCount, t, fadeOut);
        }
        
        #endregion
        
        #region SHAKE_TWEENS_AT_SPEED
        
        public static Sequence ShakePositionAtSpeed(this Transform obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return Tween.Instance.ShakePositionAtSpeed(obj, vibrato, shakeCount, speed, fadeOut);
        }

        public static Sequence ShakePositionAtSpeed(this GameObject obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return Tween.Instance.ShakePositionAtSpeed(obj, vibrato, shakeCount, speed, fadeOut);
        }

        public static Sequence ShakeRotationAtSpeed(this Transform obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return Tween.Instance.ShakeRotationAtSpeed(obj, vibrato, shakeCount, speed, fadeOut);
        }

        public static Sequence ShakeRotationAtSpeed(this GameObject obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return Tween.Instance.ShakeRotationAtSpeed(obj, vibrato, shakeCount, speed, fadeOut);
        }

        public static Sequence ShakeScaleAtSpeed(this Transform obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return Tween.Instance.ShakeScaleAtSpeed(obj, vibrato, shakeCount, speed, fadeOut);
        }

        public static Sequence ShakeScaleAtSpeed(this GameObject obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return Tween.Instance.ShakeScaleAtSpeed(obj, vibrato, shakeCount, speed, fadeOut);
        }
        
        #endregion
        
        #region PATH_TWEENS
        
        public static BaseTween PathTween(this GameObject transform, IList<Vector3> path, float duration, 
            PathType pathType = PathType.CatmullRom, PathMode pathMode = PathMode.Ignore, int resolution = 10, bool closePath = false)
        {
            return Tween.Instance.PathTween(transform, path, duration, pathType, pathMode, resolution, closePath);
        }
        
        public static BaseTween LocalPathTween(this GameObject transform, IList<Vector3> localPath, float duration, 
            PathType pathType = PathType.CatmullRom, PathMode pathMode = PathMode.Ignore, int resolution = 10, bool closePath = false)
        {
            return Tween.Instance.LocalPathTween(transform, localPath, duration, pathType, pathMode, resolution, closePath);
        }
        
        public static BaseTween PathTween(this Transform transform, IList<Vector3> path, float duration, 
            PathType pathType = PathType.CatmullRom, PathMode pathMode = PathMode.Ignore, int resolution = 10, bool closePath = false)
        {
            return Tween.Instance.PathTween(transform, path, duration, pathType, pathMode, resolution, closePath);
        }
        
        public static BaseTween LocalPathTween(this Transform transform, IList<Vector3> localPath, float duration, 
            PathType pathType = PathType.CatmullRom, PathMode pathMode = PathMode.Ignore, int resolution = 10, bool closePath = false)
        {
            return Tween.Instance.LocalPathTween(transform, localPath, duration, pathType, pathMode, resolution, closePath);
        }
        
        #endregion
        
        #region PATH_TWEENS_AT_SPEED
        
        public static BaseTween PathTweenAtSpeed(this GameObject transform, IList<Vector3> path, float speed, 
            PathType pathType = PathType.CatmullRom, PathMode pathMode = PathMode.Ignore, int resolution = 10, bool closePath = false)
        {
            return Tween.Instance.PathTweenAtSpeed(transform, path, speed, pathType, pathMode, resolution, closePath);
        }
        
        public static BaseTween LocalPathTweenAtSpeed(this GameObject transform, IList<Vector3> localPath, float speed, 
            PathType pathType = PathType.CatmullRom, PathMode pathMode = PathMode.Ignore, int resolution = 10, bool closePath = false)
        {
            return Tween.Instance.LocalPathTweenAtSpeed(transform, localPath, speed, pathType, pathMode, resolution, closePath);
        }
        
        public static BaseTween PathTweenAtSpeed(this Transform transform, IList<Vector3> path, float speed, 
            PathType pathType = PathType.CatmullRom, PathMode pathMode = PathMode.Ignore, int resolution = 10, bool closePath = false)
        {
            return Tween.Instance.PathTweenAtSpeed(transform, path, speed, pathType, pathMode, resolution, closePath);
        }
        
        public static BaseTween LocalPathTweenAtSpeed(this Transform transform, IList<Vector3> localPath, float speed, 
            PathType pathType = PathType.CatmullRom, PathMode pathMode = PathMode.Ignore, int resolution = 10, bool closePath = false)
        {
            return Tween.Instance.LocalPathTweenAtSpeed(transform, localPath, speed, pathType, pathMode, resolution, closePath);
        }
        
        #endregion
        
        #region STRING_TWEENS
        
        public static BaseTween StringTween(this Text text, string to, float time, bool richTextEnabled = false, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
        {
            return Tween.Instance.StringTween(text, to, time, richTextEnabled, scrambleMode, scrambleChars);
        }
        
        public static BaseTween StringTween(this Text text, string from, string to, float time, bool richTextEnabled = false, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
        {
            return Tween.Instance.StringTween(text, from, to, time, richTextEnabled, scrambleMode, scrambleChars);
        }
        
        #endregion
        
        #region STRING_TWEENS_AT_SPEED
        
        public static BaseTween StringTweenAtSpeed(this Text text, string to, float speed, bool richTextEnabled = false, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
        {
            return Tween.Instance.StringTweenAtSpeed(text, to, speed, richTextEnabled, scrambleMode, scrambleChars);
        }
        
        public static BaseTween StringTweenAtSpeed(this Text text, string from, string to, float speed, bool richTextEnabled = false, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
        {
            return Tween.Instance.StringTweenAtSpeed(text, from, to, speed, richTextEnabled, scrambleMode, scrambleChars);
        }
        
        #endregion
        
        #region TWEEN_CONTROL
        
        public static void CancelAllTweens(this GameObject go)
        {
            Tween.Instance.CancelTween(go);
        }

        public static void CancelTween(this GameObject go, int id)
        {
            Tween.Instance.CancelTween(id);
        }
        
        /// <summary>
        /// 设置路径闭合
        /// </summary>
        public static T SetClosePath<T>(this T tween, bool closePath) where T : BaseTween
        {
            if (tween is PathTween pathTween)
            {
                pathTween.SetClosePath(closePath);
            }
            return tween;
        }
        
        #endregion
    }
}