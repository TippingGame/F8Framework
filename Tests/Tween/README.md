# F8 Tween

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 Tween组件，补间动画，播放/终止动画，**自由组合**动画，有旋转/位移/缩放/渐变/填充/震动动画，可根据UI的**相对布局**位移动画。  

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 视频教程：[【Unity框架】（8）补间动画](https://www.bilibili.com/video/BV15EyVY5ELA)

### 代码使用方法
```C#
// 画布的RectTransform
public RectTransform canvasRect;

void Start()
{
    /*-----------------------------------------普通用法-----------------------------------------*/
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
    gameObject.LocalMove(Vector3.one, 1f);
    gameObject.LocalMoveAtSpeed(Vector3.one, 1f);
    // 缩放
    gameObject.ScaleTween(Vector3.one * 2f, 1f);
    // 渐变
    gameObject.GetComponent<CanvasGroup>().Fade(0f, 1f);
    gameObject.GetComponent<Image>().ColorTween(Color.green, 1f);
    // 填充
    gameObject.GetComponent<Image>().FillAmountTween(1f, 1f);
    // 震动
    gameObject.ShakePosition(Vector3.one, shakeCount: 8, t: 0.05f, fadeOut: false);
    gameObject.ShakeRotation(Vector3.one);
    gameObject.ShakeScale(Vector3.one);
    gameObject.ShakePositionAtSpeed(Vector3.one, shakeCount: 8, speed: 5f, fadeOut: false);

    // 链式调用
    gameObject.Move(Vector3.one, 1f)
        .SetEase(Ease.EaseOutQuad) // 设置Ease
        .SetOnComplete(OnViewOpen) // 设置完成回调
        .SetDelay(2f) // 设置Delay
        .SetEvent(OnViewOpen, 2.5f) // 设置Event，在动画的某一时间调用
        .SetLoopType(LoopType.Yoyo, 3) // 设置循环类型（Restart，Flip，Incremental，Yoyo），循环次数
        .SetUpdateMode(UpdateMode.Update) // 设置Update模式，默认为Update
        .SetOwner(gameObject) // 设置动画拥有者
        .SetIsPause(false); // 设置是否暂停
    
    // 设置是否暂停
    FF8.Tween.SetIsPause(id, true);
        
    // 你也可以这样使用，设置OnUpdate
    // 数字缓动变化
    BaseTween valueTween = FF8.Tween.ValueTween(0f, 100f, 3f).SetOnUpdateFloat((float v) =>
    {
        LogF8.Log(v);
    });
    
    // 取消动画，只允许使用ID取消动画，动画基类会回收再利用，但ID唯一递增
    int id2 = valueTween.ID;
    FF8.Tween.CancelTween(id2);
    
    // 物体移动
    BaseTween gameObjectTween = FF8.Tween.Move(gameObject, Vector3.one, 3f).SetOnUpdateVector3((Vector3 v) =>
    {
        LogF8.Log(v);
    });
        
    // 设置动画拥有者后，可使用此取消方式
    gameObjectTween.SetOwner(gameObject);
    FF8.Tween.CancelTween(gameObject);
    gameObject.CancelAllTweens();
    
    // 根据相对坐标移动UI
    // (0.0 , 1.0) _______________________(1.0 , 1.0)
    //            |                      |
    //            |                      |                  
    //            |                      |
    //            |                      |
    // (0.0 , 0.0)|______________________|(1.0 , 0.0)
    transform.GetComponent<RectTransform>().MoveUI(new Vector2(1f, 1f), canvasRect, 1f)
        .SetEase(Ease.EaseOutBounce);
    
    
    /*-----------------------------------------动画组合-----------------------------------------*/
    // 初始化，依次执行动画/并行执行动画，回调
    var sequence = SequenceManager.GetSequence();
    
    sequence.Append(valueTween); // 第一个动画
    sequence.Join(gameObjectTween);   // 与第一个动画同时执行
    sequence.Append(valueTween); // 第二个动画完成后执行
    sequence.Append(() => LogF8.Log("完成了！")); // 动画序列结束后的回调
    sequence.SetOnComplete(() => LogF8.Log("Sequence 完成"));
    
    // 设置循环次数，-1代表无限循环
    sequence.SetLoops(3);
    
    // 设置特定时间运行事件或动画
    sequence.RunAtTime(() => LogF8.Log("半途事件"), 1.5f); // 在1.5秒时执行回调
    sequence.RunAtTime(gameObjectTween, 2.0f); // 在2秒时开始执行特定动画
    
    // 回收Sequence，并停止所有动画
    SequenceManager.KillSequence(sequence);
}

/*-----------------------------------------使用协程等待动画和动画组-----------------------------------------*/
IEnumerator Coroutine() {
    yield return gameObject.Move(Vector3.one, 1f);
    
    var sequence = SequenceManager.GetSequence();
    var baseTween = gameObject.Move(Vector3.one, 1f);
    sequence.Append(baseTween);
    yield return sequence;
}

/*-----------------------------------------使用async/await等待动画和动画组-----------------------------------------*/
async void Async() {
    await gameObject.Move(Vector3.one, 1f);
    
    var sequence = SequenceManager.GetSequence();
    var baseTween = gameObject.Move(Vector3.one, 1f);
    sequence.Append(baseTween);
    await sequence;
}
```


