using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class DemoTween : MonoBehaviour
    {
        // 画布的RectTransform
        public RectTransform canvasRect;

        void Start()
        {
            // 播放动画，设置Ease动画，设置OnComplete完成回调
            int id = gameObject.ScaleTween(Vector3.one, 1f).SetEase(Ease.Linear).SetOnComplete(OnViewOpen).ID;

            void OnViewOpen()
            {

            }

            // 旋转
            gameObject.RotateTween(Vector3.one, 1f);
            // 位移
            gameObject.Move(Vector3.one, 1f);
            gameObject.MoveAtSpeed(Vector3.one, 2f);
            // 渐变
            gameObject.GetComponent<CanvasGroup>().Fade(0f, 1f);
            gameObject.GetComponent<Image>().ColorTween(Color.green, 1f);
            // 填充
            gameObject.GetComponent<Image>().FillAmountTween(1f, 1f);

            // 终止动画
            gameObject.CancelTween(id);
            gameObject.CancelAllTweens();

            // 设置Delay
            gameObject.Move(Vector3.one, 1.0f).SetDelay(2.0f);
            
            // 设置Event，在动画的某一时间调用
            gameObject.Move(Vector3.one, 5.0f).SetEvent(OnViewOpen, 2.5f);
            
            // 你也可以这样使用，设置OnUpdate
            // 数字缓动变化
            BaseTween valueTween = FF8.Tween.ValueTween(0f, 100f, 3f).SetOnUpdateFloat((float v) =>
            {
                LogF8.Log(v);
            });
            
            FF8.Tween.CancelTween(valueTween);
            
            // 物体移动
            BaseTween gameObjectTween = FF8.Tween.Move(gameObject, Vector3.one, 3f).SetOnUpdateVector3((Vector3 v) =>
            {
                LogF8.Log(v);
            });
            
            FF8.Tween.CancelTween(gameObjectTween.ID);
            
            // 根据相对坐标移动UI
            // (0.0 , 1.0) _______________________(1.0 , 1.0)
            //            |                      |
            //            |                      |                  
            //            |                      |
            //            |                      |
            // (0.0 , 0.0)|______________________|(1.0 , 0.0)
            transform.GetComponent<RectTransform>().MoveUI(new Vector2(1f, 1f), canvasRect, 1f)
                .SetEase(Ease.EaseOutBounce);
        }
    }
}
