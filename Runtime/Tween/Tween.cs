using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace F8Framework.Core
{

    /// <summary>
    /// Tween engine
    /// </summary>
    [UpdateRefresh]
    [LateUpdateRefresh]
    [FixedUpdateRefresh]
    public class Tween : ModuleSingleton<Tween>, IModule
    {
        #region PRIVATE
        private List<BaseTween> tweens = TweenPool.activeTweens;
        private Dictionary<GameObject, List<int>> tweenConnections = new Dictionary<GameObject, List<int>>();
        #endregion

        public Action<float> OnUpdateAction;
        
        #region UNITY_EVENTS
        
        public void OnInit(object createParam)
        {
            
        }

        public void OnUpdate()
        {
            if(OnUpdateAction != null)
                OnUpdateAction.Invoke(Time.deltaTime);
            
            for(int n = 0; n < tweens.Count; n++)
            {
                int lastTweenSize = tweens.Count;
                
                if (tweens[n].UpdateMode == UpdateMode.Update)
                    tweens[n].Update(Time.deltaTime);
                
                //was any tween remove in this update?
                if (lastTweenSize != tweens.Count)
                {
                    n--;
                }
            }
        }

        public void OnLateUpdate()
        {
            for (int n = 0; n < tweens.Count; n++)
            {
                int lastTweenSize = tweens.Count;

                if (tweens[n].UpdateMode == UpdateMode.LateUpdate)
                    tweens[n].Update(Time.deltaTime);

                //was any tween remove in this update?
                if (lastTweenSize != tweens.Count)
                {
                    n--;
                }
            }
        }

        public void OnFixedUpdate()
        {
            for (int n = 0; n < tweens.Count; n++)
            {
                int lastTweenSize = tweens.Count;

                if (tweens[n].UpdateMode == UpdateMode.FixedUpdate)
                    tweens[n].Update(Time.deltaTime);

                //was any tween remove in this update?
                if (lastTweenSize != tweens.Count)
                {
                    n--;
                }
            }
        }

        public void OnTermination()
        {
            base.Destroy();
        }

        #endregion



        private BaseTween ProcessTween(BaseTween tween)
        {
            tween.SetOnComplete(() => tween.IsComplete = true);
            tween.SetOnComplete(() => tween.Recycle(tween));
            return tween;
        }

        public void ProcessConnection(BaseTween tween)
        {
            List<int> idList;
            if (tweenConnections.TryGetValue(tween.Owner, out idList))
            {
                if (idList == null)
                {
                    idList = new List<int>();
                }

                idList.Add(tween.ID);
            }

            else
            {
                tweenConnections[tween.Owner] = new List<int>() { tween.ID };
            }
        }

        public void CancelTween(int id)
        {

            for (int n = 0; n < tweens.Count; n++)
            {
                if (tweens[n].ID == id)
                {
                    tweens[n].Recycle(tweens[n]);
                    break;
                }
            }            

        }

        public void CancelTween(BaseTween tween)
        {
            if (tween != null)
                CancelTween(tween.ID);
        }


        public void CancelTween(GameObject owner)
        {
            List<int> idList = null;

            if (tweenConnections.TryGetValue(owner, out idList))
            {
                if (idList == null)
                    return;

                for (int n = 0; n < idList.Count; n++)
                {
                    CancelTween(idList[n]);                    
                }

                tweenConnections.Remove(owner);
            }
        }

        #region SCALE_TWEENS
        public BaseTween ScaleTween(Transform t, Vector3 to, float time)
        {
            Vector3Tween tween = TweenPool.GetVector3Tween(t.localScale, to, time);
            tween.PauseReset += () => tween.Init(t.localScale, to, time);
            tween.SetOnUpdateVector3(delegate (Vector3 v)
            {
                if (t == null)
                {
                    CancelTween(tween);
                    return;
                }


                t.localScale = v;
            });
            return ProcessTween(tween);
        }

        public BaseTween ScaleTween(GameObject go, Vector3 to, float time)
        {
            return ScaleTween(go.transform, to, time);
        }

        public BaseTween ScaleTween(RectTransform rect, Vector3 to, float time)
        {
            return ScaleTween(rect.transform, to, time);
        }

        public BaseTween ScaleTweenAtSpeed(Transform t, Vector3 to, float speed)
        {
            float time = Vector3.Distance(t.position, to) / speed;

            return ScaleTween(t, to, time);
        }

        public BaseTween ScaleTweenAtSpeed(GameObject go, Vector3 to, float speed)
        {
            float time = Vector3.Distance(go.transform.position, to) / speed;

            return ScaleTween(go, to, time);
        }

        public BaseTween ScaleTweenAtSpeed(RectTransform rect, Vector3 to, float speed)
        {
            float time = Vector3.Distance(rect.transform.position, to) / speed;

            return ScaleTween(rect.transform, to, time);
        }
        public BaseTween ScaleX(Transform obj, float value, float t)
        {
            return ValueTween(obj.localScale.x, value, t).SetOnUpdateFloat((float v) =>
            {
                if (obj == null)
                {
                    return;
                }

                Vector3 currentScale = obj.localScale;
                currentScale.x = v;
                obj.localScale = currentScale;
            });
        }

        public BaseTween ScaleX(GameObject obj, float value, float t)
        {
            return ScaleX(obj.transform, value, t);
        }

        public BaseTween ScaleX(RectTransform obj, float value, float t)
        {
            return ScaleX(obj.transform, value, t);
        }

        public BaseTween ScaleXAtSpeed(Transform obj, float value, float speed)
        {
            float time = Math.Abs(obj.localScale.x - value) / speed;
            return ScaleX(obj, value, time);
        }

        public BaseTween ScaleXAtSpeed(GameObject obj, float value, float speed)
        {
            float time = Math.Abs(obj.transform.localScale.x - value) / speed;
            return ScaleX(obj, value, time);
        }

        public BaseTween ScaleXAtSpeed(RectTransform obj, float value, float speed)
        {
            float time = Math.Abs(obj.transform.localScale.x - value) / speed;
            return ScaleX(obj, value, time);
        }


        public BaseTween ScaleY(Transform obj, float value, float t)
        {
            return ValueTween(obj.localScale.y, value, t).SetOnUpdateFloat((float v) =>
            {
                if (obj == null)
                    return;

                Vector3 currentScale = obj.localScale;
                currentScale.y = v;
                obj.localScale = currentScale;
            });
        }

        public BaseTween ScaleY(GameObject obj, float value, float t)
        {
            return ScaleY(obj.transform, value, t);
        }


        public BaseTween ScaleY(RectTransform obj, float value, float t)
        {
            return ScaleY(obj.transform, value, t);
        }

        public BaseTween ScaleYAtSpeed(Transform obj, float value, float speed)
        {
            float time = Math.Abs(obj.localScale.y - value) / speed;
            return ScaleY(obj, value, time);
        }

        public BaseTween ScaleYAtSpeed(GameObject obj, float value, float speed)
        {
            float time = Math.Abs(obj.transform.localScale.y - value) / speed;
            return ScaleY(obj, value, time);
        }

        public BaseTween ScaleYAtSpeed(RectTransform obj, float value, float speed)
        {
            float time = Math.Abs(obj.transform.localScale.y - value) / speed;
            return ScaleY(obj, value, time);
        }


        public BaseTween ScaleZ(Transform obj, float value, float t)
        {
            return ValueTween(obj.localScale.z, value, t).SetOnUpdateFloat((float v) =>
            {
                if (obj == null)
                    return;

                Vector3 currentScale = obj.localScale;
                currentScale.z = v;
                obj.localScale = currentScale;
            });
        }

        public BaseTween ScaleZ(GameObject obj, float value, float t)
        {
            return ScaleZ(obj.transform, value, t);
        }

        public BaseTween ScaleZ(RectTransform obj, float value, float t)
        {
            return ScaleZ(obj.transform, value, t);
        }

        public BaseTween ScaleZAtSpeed(Transform obj, float value, float speed)
        {
            float time = Math.Abs(obj.localScale.z - value) / speed;
            return ScaleZ(obj, value, time);
        }

        public BaseTween ScaleZAtSpeed(GameObject obj, float value, float speed)
        {
            float time = Math.Abs(obj.transform.localScale.z - value) / speed;
            return ScaleZ(obj, value, time);
        }

        public BaseTween ScaleZAtSpeed(RectTransform obj, float value, float speed)
        {
            float time = Math.Abs(obj.transform.localScale.z - value) / speed;
            return ScaleZ(obj, value, time);
        }

        #endregion

        #region ROTATE_TWEENS

        public BaseTween RotateTween(Transform t, Quaternion to, float time)
        {
            QuaternionTween tween = TweenPool.GetQuaternionTween(t.rotation, to, time);

            tween.SetOnUpdateQuaternion(delegate (Quaternion v)
            {
                if (t != null)
                {
                    t.rotation = v;
                }
                else
                {
                    CancelTween(tween);
                }

            });
            
            return ProcessTween(tween);
        }

        public BaseTween RotateTween(Transform t, Vector3 to, float time)
        {
            Quaternion toRotation = Quaternion.Euler(to);
            return RotateTween(t, toRotation, time);
        }

        public BaseTween RotateTween(Transform t, Vector3 axis, float to, float time)
        {
            Quaternion toRotation = Quaternion.Euler(axis * to);
            return RotateTween(t, toRotation, time);
        }

        public BaseTween RotateTween(GameObject go, Vector3 axis, float to, float time)
        {
            return RotateTween(go.transform, axis, to, time);
        }

        public BaseTween RotateTween(RectTransform rect, Vector3 axis, float to, float time)
        {
            return RotateTween(rect.transform, axis, to, time);
        }

        #endregion

        #region FADE_TWEENS

        public BaseTween FadeOut(CanvasGroup cg, float t)
        {
            return Fade(cg, 0.0f, t);
        }

        public BaseTween FadeOutAtSpeed(CanvasGroup cg, float speed)
        {
            float t = cg.alpha / speed;
            return Fade(cg, 0.0f, t);
        }

        public BaseTween FadeIn(CanvasGroup cg, float t)
        {
            return Fade(cg, 1.0f, t);
        }

        public BaseTween FadeInAtSpeed(CanvasGroup cg, float speed)
        {
            float t = Mathf.Abs(cg.alpha - 1.0f) / speed;
            return Fade(cg, 1.0f, t);
        }

        public BaseTween Fade(CanvasGroup cg, float to, float t)
        {
            ValueTween tween = TweenPool.GetValueTween(cg.alpha, to, t);
            tween.SetOnUpdateFloat(delegate (float v)
            {
                if (cg == null)
                {
                    CancelTween(tween);
                    return;
                }

                cg.alpha = v;
            });
            return ProcessTween(tween);
        }

        public BaseTween FadeAtSpeed(CanvasGroup cg, float to, float speed)
        {
            float t = Math.Abs(cg.alpha - to) / speed;
            return Fade(cg, to, t);
        }

        public BaseTween Fade(Image image, float to, float t)
        {
            ValueTween tween = TweenPool.GetValueTween(image.color.a, to, t);
            tween.SetOnUpdateFloat(v =>
            {
                if (image == null)
                {
                    CancelTween(tween);
                    return;
                }

                Color c = image.color;
                c.a = v;
                image.color = c;
            });
            return ProcessTween(tween);
        }

        internal BaseTween SetFloatProperty(Material material, int propertyHash, float value, float t)
        {
            ValueTween tween = TweenPool.GetValueTween(material.GetFloat(propertyHash), value, t);
            tween.SetOnUpdateFloat(v => material.SetFloat(propertyHash, v));
            return ProcessTween(tween);
        }

        public BaseTween FadeAtSpeed(Image img, float to, float speed)
        {
            float t = Math.Abs(img.color.a - to) / speed;
            return Fade(img, to, t);
        }

        public BaseTween FadeOut(Image image, float t)
        {
            return Fade(image, 0.0f, t);
        }

        public BaseTween FadeOutAtSpeed(Image img, float speed)
        {
            float t = img.color.a / speed;
            return Fade(img, 0.0f, t);
        }

        public BaseTween FadeIn(Image image, float t)
        {
            return Fade(image, 1.0f, t);
        }

        public BaseTween FadeInAtSpeed(Image img, float speed)
        {
            float t = Mathf.Abs(img.color.a - 1.0f) / speed;
            return Fade(img, 1.0f, t);
        }

        public BaseTween Fade(SpriteRenderer sprite, float to, float t)
        {
            ValueTween tween = TweenPool.GetValueTween(sprite.color.a, to, t);
            tween.SetOnUpdateFloat(delegate (float v)
            {
                if (sprite == null)
                {
                    CancelTween(tween);
                    return;
                }

                Color c = sprite.color;
                c.a = v;
                sprite.color = c;
            });
            return ProcessTween(tween);
        }

        public BaseTween FadeAtSpeed(SpriteRenderer sprite, float to, float speed)
        {
            float t = Math.Abs(sprite.color.a - to) / speed;
            return Fade(sprite, to, t);
        }

        public BaseTween FadeOut(SpriteRenderer sprite, float t)
        {
            return Fade(sprite, 0.0f, t);
        }

        public BaseTween FadeOutAtSpeed(SpriteRenderer sprite, float speed)
        {
            float t = sprite.color.a / speed;
            return Fade(sprite, 0.0f, t);
        }

        public BaseTween FadeIn(SpriteRenderer sprite, float t)
        {
            return Fade(sprite, 1.0f, t);
        }

        public BaseTween FadeInAtSpeed(SpriteRenderer sprite, float speed)
        {
            float t = Mathf.Abs(sprite.color.a - 1.0f) / speed;
            return Fade(sprite, 1.0f, t);
        }

        #endregion

        #region COLOR_TWEEN

        private Vector3 ColorToVector3(Color c)
        {
            return new Vector3(c.r, c.g, c.b);
        }

        private float CalculateColorDistance(Color a, Color b)
        {
            return Vector3.Distance(ColorToVector3(a), ColorToVector3(b));
        }

        public BaseTween ColorTween(Material material, Color to, float t)
        {
            ColorTween tween = TweenPool.GetColorTween(material.color, to, t);
            tween.SetOnUpdateColor(delegate (Color c)
            {
                if (material == null)
                {
                    TweenPool.FinishTween(tween);
                    return;
                }

                material.color = c;
            });
            return ProcessTween(tween);
        }
        
        public BaseTween ColorTween(SpriteRenderer sprite, Color to, float t)
        {
            ColorTween tween = TweenPool.GetColorTween(sprite.color, to, t);
            tween.SetOnUpdateColor(delegate (Color c)
            {
                if (sprite == null)
                {
                    TweenPool.FinishTween(tween);
                    return;
                }

                sprite.color = c;
            });
            return ProcessTween(tween);
        }

        public BaseTween ColorTweenAtSpeed(SpriteRenderer sprite, Color to, float speed)
        {
            float t = CalculateColorDistance(sprite.color, to) / speed;
            return ColorTween(sprite, to, t);
        }

        public BaseTween ColorTween(Image image, Color to, float t)
        {
            ColorTween tween = TweenPool.GetColorTween(image.color, to, t);
            tween.SetOnUpdateColor(delegate (Color c)
            {
                if (image == null)
                {
                    CancelTween(tween);
                    return;
                }

                image.color = c;
            });
            return ProcessTween(tween);
        }

        public BaseTween ColorTweenAtSpeed(Image img, Color to, float speed)
        {
            float t = CalculateColorDistance(img.color, to) / speed;
            return ColorTween(img, to, t);
        }

        public BaseTween ColorTween(Color from, Color to, float t)
        {
            ColorTween tween = TweenPool.GetColorTween(from, to, t);
            return ProcessTween(tween);
        }

        public BaseTween ColorTweenAtSpeed(Color from, Color to, float speed)
        {
            float t = CalculateColorDistance(from, to) / speed;
            return ColorTween(from, to, t);
        }
        #endregion

        #region FILL_AMOUNT_TWEEN
        public BaseTween FillAmountTween(Image img, float to, float t)
        {
            BaseTween tween = ValueTween(img.fillAmount, to, t);
            tween.SetOnUpdateFloat( delegate (float v)
            {
                if (img != null)                
                    img.fillAmount = v;               
                else
                    CancelTween(tween);

            } );
            return tween; 
        }

        public BaseTween FillAmountTweenAtSpeed(Image img, float to, float speed)
        {
            BaseTween tween = ValueTweenAtSpeed(img.fillAmount, to, speed);
            tween.SetOnUpdateFloat(delegate (float v)
            {
                if (img != null)
                    img.fillAmount = v;
                else
                    CancelTween(tween);
            });
            return tween;
        }
        #endregion

        #region VECTOR_TWEEN
        public BaseTween VectorTween(Vector2 from, Vector2 to, float t)
        {
            Vector2Tween tween = TweenPool.GetVector2Tween(from, to, t);
            return ProcessTween(tween);
        }

        public BaseTween VectorTweenAtSpeed(Vector2 from, Vector2 to, float speed)
        {
            float t = Vector2.Distance(from, to) / speed;
            return VectorTween(from, to, t);
        }

        public BaseTween VectorTween(Vector3 from, Vector3 to, float t)
        {
            Vector3Tween tween = TweenPool.GetVector3Tween(from, to, t);
            return ProcessTween(tween);
        }

        public BaseTween VectorTweenAtSpeed(Vector3 from, Vector3 to, float speed)
        {
            float t = Vector3.Distance(from, to) / speed;
            return VectorTween(from, to, t);
        }

        #endregion


        #region VALUE_TWEEN
        public BaseTween ValueTween(float from, float to, float t)
        {
            var tween = TweenPool.GetValueTween(from, to, t);
            return ProcessTween(tween);
        }

        public BaseTween ValueTweenAtSpeed(float from, float to, float speed)
        {
            float t = Math.Abs(from - to) / speed;
            return ValueTween(from, to, t);
        }
        #endregion

        #region MOVE_TWEEN
        public BaseTween Move(Transform obj, Transform to, float t)
        {
            MoveTween tween = TweenPool.GetMoveTween(obj, to, t);
            return ProcessTween(tween);
        }

        public BaseTween MoveAtSpeed(Transform obj, Transform to, float speed)
        {
            float t = Vector3.Distance(obj.position, to.position) / speed;
            return Move(obj, to, t);
        }

        public BaseTween Move(Transform obj, Vector3 to, float t)
        {
            Vector3Tween tween = TweenPool.GetVector3Tween(obj.position, to, t);
            tween.PauseReset += () => tween.Init(obj.position, to, t);
            tween.SetOnUpdateVector3((Vector3 pos) =>
            {
                if (obj == null)
                {
                    CancelTween(tween);
                    return;
                }

                obj.position = pos;
            });
            return ProcessTween(tween);
        }

        public BaseTween MoveAtSpeed(Transform obj, Vector3 to, float speed)
        {
            float t = Vector3.Distance(obj.position, to) / speed;
            return Move(obj, to, t);
        }


        public BaseTween Move(GameObject obj, Transform to, float t)
        {
            return Move(obj.transform, to, t);
        }

        public BaseTween MoveAtSpeed(GameObject obj, Transform to, float speed)
        {
            float t = Vector3.Distance(obj.transform.position, to.position) / speed;
            return Move(obj, to, t);
        }

        public BaseTween Move(GameObject obj, Vector3 to, float t)
        {
            return Move(obj.transform, to, t);
        }

        public BaseTween MoveAtSpeed(GameObject obj, Vector3 to, float speed)
        {
            float t = Vector3.Distance(obj.transform.position, to) / speed;
            return Move(obj, to, t);
        }

        public BaseTween Move(GameObject obj, GameObject to, float t)
        {
            return Move(obj.transform, to.transform, t);
        }

        public BaseTween MoveAtSpeed(GameObject obj, GameObject to, float speed)
        {
            float t = Vector3.Distance(obj.transform.position, to.transform.position) / speed;
            return Move(obj, to, t);
        }

        public BaseTween Move(Transform obj, GameObject to, float t)
        {
            return Move(obj, to.transform, t);
        }

        public BaseTween MoveAtSpeed(Transform obj, GameObject to, float speed)
        {
            float t = Vector3.Distance(obj.position, to.transform.position) / speed;
            return Move(obj, to, t);
        }

        public BaseTween Move(RectTransform rect, Vector2 pos, float t)
        {

            return VectorTween(new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y, 0.0f), new Vector3(pos.x, pos.y, 0.0f), t).SetOnUpdateVector3((Vector3 value) =>
            {
                if (rect == null)
                    return;

                rect.anchoredPosition = new Vector2(value.x, value.y);
            });
        }

        public BaseTween MoveAtSpeed(RectTransform rect, Vector2 pos, float speed)
        {
            float t = Vector2.Distance(new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y), new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y)) / speed;
            return Move(rect, pos, t);
        }

        //use this to position UI in absolute coordenates
        //0.0 , 1.0 _______________________1.0 , 1.0
        //          |                      |
        //          |                      |                  
        //          |                      |
        //          |                      |
        //0.0 , 0.0 |______________________| 1.0 , 0.0


        /// <summary>
        /// Move a UI element using absolute position
        /// Note: dont use this on Awake
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="absolutePosition"></param>
        /// <param name="canvas"></param>
        /// <param name="t"></param>
        /// <returns></returns>        

        public BaseTween MoveUI(RectTransform rect, Vector2 absolutePosition, RectTransform canvas, float t, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            Vector2 pos = rect.FromAbsolutePositionToAnchoredPosition(absolutePosition, canvas, pivotPreset);

            return Move(rect, pos, t);
        }

        public BaseTween MoveUIAtSpeed(RectTransform rect, Vector2 absolutePosition, RectTransform canvas, float speed, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {

            Vector2 pos = rect.FromAbsolutePositionToAnchoredPosition(absolutePosition, canvas, pivotPreset);

            float time = Vector3.Distance(rect.anchoredPosition, pos) / speed;

            return MoveUI(rect, absolutePosition, canvas, time, pivotPreset);
        }

        public BaseTween TranslateUI(RectTransform rect, Vector2 translation, RectTransform canvas, float t, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            Vector2 pos = rect.FromAnchoredPositionToAbsolutePosition(canvas, pivotPreset);
            Vector2 finalPos = pos + translation;

            return MoveUI(rect, finalPos, canvas, t, pivotPreset);
        }

        public BaseTween TranslateUIAtSpeed(RectTransform rect, Vector2 translation, RectTransform canvas, float speed, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            Vector2 pos = rect.FromAnchoredPositionToAbsolutePosition(canvas, pivotPreset);
            Vector2 finalPos = pos + translation;

            return MoveUIAtSpeed(rect, finalPos, canvas, speed, pivotPreset);
        }

        #endregion
    }

}

