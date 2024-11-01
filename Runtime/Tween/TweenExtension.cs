using UnityEngine;
using UnityEngine.UI;
using F8Framework.Core;

public static class TweenExtension
{
    public static BaseTween ScaleTween(this Transform t , Vector3 to , float time)
    {
        return Tween.Instance.ScaleTween(t , to , time);
    }

    public static BaseTween ScaleTween(this GameObject go, Vector3 to, float time)
    {
        return Tween.Instance.ScaleTween( go.transform, to, time );
    }

    public static BaseTween ScaleTween(this RectTransform rect, Vector3 to, float time)
    {
        return Tween.Instance.ScaleTween( rect, to, time );
    }

    public static BaseTween ScaleAtSpeed(this Transform t, Vector3 to, float speed)
    {
        return Tween.Instance.ScaleTweenAtSpeed(t , to , speed);
    }

    public static BaseTween ScaleAtSpeed(this GameObject go, Vector3 to, float speed)
    {
        return Tween.Instance.ScaleTweenAtSpeed(go , to , speed);
    }

    public static BaseTween ScaleAtSpeed(this RectTransform rect, Vector3 to, float speed)
    {
        return Tween.Instance.ScaleTweenAtSpeed(rect , to , speed);
    }

    public static BaseTween ScaleX(this Transform obj, float value, float t)
    {
        return Tween.Instance.ScaleX(obj , value , t);
    }

    public static BaseTween ScaleX(this GameObject obj, float value, float t)
    {
        return Tween.Instance.ScaleX(obj , value , t);
    }

    public static BaseTween ScaleX(this RectTransform obj, float value, float t)
    {
        return Tween.Instance.ScaleX(obj , value , t);
    }

    public static BaseTween ScaleXAtSpeed(this Transform obj, float value, float speed)
    {
        return Tween.Instance.ScaleXAtSpeed(obj , value , speed);
    }

    public static BaseTween ScaleXAtSpeed(this GameObject obj, float value, float speed)
    {
        return Tween.Instance.ScaleXAtSpeed(obj , value , speed);
    }

    public static BaseTween ScaleXAtSpeed(this RectTransform obj, float value, float speed)
    {
        return Tween.Instance.ScaleXAtSpeed(obj , value , speed);
    }
    
    public static BaseTween ScaleY(this Transform obj, float value, float t)
    {
        return Tween.Instance.ScaleY( obj, value, t );
    }

    public static BaseTween ScaleY(this GameObject obj, float value, float t)
    {
        return Tween.Instance.ScaleY( obj, value, t );
    }

    public static BaseTween ScaleY(this RectTransform obj, float value, float t)
    {
        return Tween.Instance.ScaleY( obj, value, t );
    }

    public static BaseTween ScaleYAtSpeed(this Transform obj, float value, float speed)
    {
        return Tween.Instance.ScaleYAtSpeed( obj, value, speed );
    }

    public static BaseTween ScaleYAtSpeed(this GameObject obj, float value, float speed)
    {
        return Tween.Instance.ScaleYAtSpeed( obj, value, speed );
    }

    public static BaseTween ScaleYAtSpeed(this RectTransform obj, float value, float speed)
    {
        return Tween.Instance.ScaleYAtSpeed( obj, value, speed );
    }

    public static BaseTween ScaleZ(this Transform obj, float value, float t)
    {
        return Tween.Instance.ScaleX( obj, value, t );
    }

    public static BaseTween ScaleZ(this GameObject obj, float value, float t)
    {
        return Tween.Instance.ScaleX( obj, value, t );
    }

    public static BaseTween ScaleZ(this RectTransform obj, float value, float t)
    {
        return Tween.Instance.ScaleX( obj, value, t );
    }

    public static BaseTween ScaleZAtSpeed(this Transform obj, float value, float speed)
    {
        return Tween.Instance.ScaleXAtSpeed( obj, value, speed );
    }

    public static BaseTween ScaleZAtSpeed(this GameObject obj, float value, float speed)
    {
        return Tween.Instance.ScaleXAtSpeed( obj, value, speed );
    }

    public static BaseTween ScaleZAtSpeed(this RectTransform obj, float value, float speed)
    {
        return Tween.Instance.ScaleXAtSpeed( obj, value, speed );
    }

    public static BaseTween RotateTween(this Transform t, Vector3 axis, float to, float time)
    {
        return Tween.Instance.RotateTween(t , axis , to , time);
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
        return Tween.Instance.RotateTween(go.transform, to, time);
    }

    public static BaseTween RotateTween(this GameObject go, Vector3 axis, float to, float time)
    {
        return Tween.Instance.RotateTween(go, axis, to, time);
    }

    public static BaseTween RotateTween(this GameObject go, Quaternion to, float time)
    {
        return Tween.Instance.RotateTween(go.transform, to, time);
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

    public static BaseTween FadeOutAtSpeed(this CanvasGroup cg, float speed)
    {
        return Tween.Instance.FadeOutAtSpeed(cg , speed);
    }

    public static BaseTween FadeIn(this CanvasGroup cg, float t)
    {
        return Tween.Instance.FadeIn(cg , t);
    }

    public static BaseTween FadeInAtSpeed(this CanvasGroup cg, float speed)
    {
        return Tween.Instance.FadeInAtSpeed(cg , speed);
    }

    public static BaseTween Fade(this CanvasGroup cg, float to, float t)
    {
        return Tween.Instance.Fade(cg , to , t);
    }

    public static BaseTween FadeAtSpeed(this CanvasGroup cg, float to, float speed)
    {
        return Tween.Instance.FadeAtSpeed(cg , to , speed);
    }

    public static BaseTween Fade(Image image, float to, float t)
    {
        return Tween.Instance.Fade(image , to , t);
    }

    public static BaseTween FadeAtSpeed(this Image img, float to, float speed)
    {
        return Tween.Instance.FadeAtSpeed(img , to , speed);
    }

    public static BaseTween FadeOut(this Image image, float t)
    {
        return Tween.Instance.FadeOut(image , t);
    }

    public static BaseTween FadeOutAtSpeed(this Image img, float speed)
    {
        return Tween.Instance.FadeOutAtSpeed(img , speed);
    }

    public static BaseTween FadeIn(this Image image, float t)
    {
        return Tween.Instance.FadeIn(image , t);
    }

    public static BaseTween FadeInAtSpeed(this Image img, float speed)
    {
        return Tween.Instance.FadeInAtSpeed(img , speed);
    }

    public static BaseTween Fade(this SpriteRenderer sprite, float to, float t)
    {
        return Tween.Instance.Fade(sprite , to , t);
    }

    public static BaseTween FadeAtSpeed(this SpriteRenderer sprite, float to, float speed)
    {
        return Tween.Instance.FadeAtSpeed(sprite , to , speed);
    }

    public static BaseTween FadeOut(this CanvasGroup cg , float t)
    {
        return Tween.Instance.FadeOut(cg, t);
    }

    public static BaseTween FadeOut(this SpriteRenderer sprite, float t)
    {
        return Tween.Instance.FadeOut(sprite , t);
    }

    public static BaseTween FadeOutAtSpeed(this SpriteRenderer sprite, float speed)
    {
        return Tween.Instance.FadeOutAtSpeed(sprite , speed);
    }

    public static BaseTween FadeIn(this SpriteRenderer sprite, float t)
    {
        return Tween.Instance.FadeIn(sprite , t);
    }

    public static BaseTween FadeInAtSpeed(this SpriteRenderer sprite, float speed)
    {
        return Tween.Instance.FadeInAtSpeed(sprite , speed);
    }
    
    public static BaseTween ColorTween(this Material material, Color to, float t)
    {
        return Tween.Instance.ColorTween(material , to , t);
    }
    
    public static BaseTween ColorTween(this SpriteRenderer sprite, Color to, float t)
    {
        return Tween.Instance.ColorTween(sprite , to , t);
    }

    public static BaseTween ColorTweenAtSpeed(this SpriteRenderer sprite, Color to, float speed)
    {
        return Tween.Instance.ColorTweenAtSpeed(sprite , to , speed);
    }

    public static BaseTween ColorTween(this Image image, Color to, float t)
    {
        return Tween.Instance.ColorTween(image , to , t);
    }

    public static BaseTween ColorTweenAtSpeed(this Image img, Color to, float speed)
    {
        return Tween.Instance.ColorTweenAtSpeed(img , to , speed);
    }

    public static BaseTween FillAmountTween(this Image img , float to , float t)
    {
        return Tween.Instance.FillAmountTween(img , to , t);
    }

    public static BaseTween FillAmountTweenAtSpeed(this Image img, float to, float speed)
    {
        return Tween.Instance.FillAmountTween(img, to, speed);
    }

    public static BaseTween Move(this Transform obj, Transform to, float t)
    {
        return Tween.Instance.Move(obj , to , t);
    }

    public static BaseTween MoveAtSpeed(this Transform obj, Transform to, float speed)
    {
        return Tween.Instance.MoveAtSpeed(obj , to , speed);
    }

    public static BaseTween Move(this Transform obj, Vector3 to, float t)
    {
        return Tween.Instance.Move(obj , to , t);
    }

    public static BaseTween MoveAtSpeed(this Transform obj, Vector3 to, float speed)
    {
        return Tween.Instance.MoveAtSpeed(obj , to , speed);
    }


    public static BaseTween Move(this GameObject obj, Transform to, float t)
    {
        return Tween.Instance.Move(obj , to , t);
    }

    public static BaseTween MoveAtSpeed(this GameObject obj, Transform to, float speed)
    {
        return Tween.Instance.MoveAtSpeed(obj , to , speed);
    }

    public static BaseTween Move(this GameObject obj, Vector3 to, float t)
    {
        return Tween.Instance.Move(obj , to , t);
    }

    public static BaseTween MoveAtSpeed(this GameObject obj, Vector3 to, float speed)
    {
        return Tween.Instance.MoveAtSpeed(obj , to , speed);
    }

    public static BaseTween Move(this GameObject obj, GameObject to, float t)
    {
        return Tween.Instance.Move(obj , to , t);
    }

    public static BaseTween MoveAtSpeed(this GameObject obj, GameObject to, float speed)
    {
        return Tween.Instance.MoveAtSpeed(obj , to , speed);
    }

    public static BaseTween Move(this Transform obj, GameObject to, float t)
    {
        return Tween.Instance.Move(obj , to , t);
    }

    public static BaseTween MoveAtSpeed(this Transform obj, GameObject to, float speed)
    {
        return Tween.Instance.MoveAtSpeed(obj , to , speed);
    }

    public static BaseTween Move(this RectTransform rect, Vector2 pos, float t)
    {
        return Tween.Instance.Move(rect , pos , t);
    }

    public static BaseTween MoveAtSpeed(this RectTransform rect, Vector2 pos, float speed)
    {
        return Tween.Instance.MoveAtSpeed(rect , pos , speed);
    }

    public static BaseTween MoveUI(this RectTransform rect, Vector2 absolutePosition, RectTransform canvas, float t, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
    {
        return Tween.Instance.MoveUI(rect , absolutePosition , canvas, t , pivotPreset);
    }


    public static BaseTween MoveUIAtSpeed(this RectTransform rect, Vector2 absolutePosition, RectTransform canvas, float speed , PivotPreset pivotPreset = PivotPreset.MiddleCenter)
    {
        return Tween.Instance.MoveUIAtSpeed(rect , absolutePosition , canvas , speed , pivotPreset);
    }

    public static BaseTween TranslateUI(this RectTransform rect, Vector2 translation, RectTransform canvas, float t, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
    {
        return Tween.Instance.TranslateUI(rect , translation  , canvas , t , pivotPreset);
    }

    public static BaseTween TranslateUIAtSpeed(this RectTransform rect, Vector2 translation, RectTransform canvas, float speed, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
    {
        return Tween.Instance.TranslateUIAtSpeed( rect, translation, canvas, speed, pivotPreset );
    }    

    public static void CancelAllTweens(this GameObject go)
    {
        Tween.Instance.CancelTween(go);
    }

    public static void CancelTween(this GameObject go , int id)
    {
        Tween.Instance.CancelTween(id);
    }

}
