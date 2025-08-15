# F8 Tween

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 Tween Component**  
Play/Pause/Cancel Animations, Freely Combine animations including rotation/position/scale/fade/fill/shake effects, with layout-relative motion animations for UI.

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
// Reference to Canvas RectTransform
public RectTransform canvasRect;

void Start()
{
    /*----------------------------------------- Basic Usage -----------------------------------------*/
    // Play animation, set Ease type, and set OnComplete callback
    int id = gameObject.ScaleTween(Vector3.one, 1f).SetEase(Ease.Linear).SetOnComplete(OnViewOpen).ID;

    void OnViewOpen()
    {

    }

    // Rotation
    gameObject.RotateTween(Vector3.one, 1f);
    // Position movement
    gameObject.Move(Vector3.one, 1f);
    gameObject.MoveAtSpeed(Vector3.one, 2f);
    gameObject.LocalMove(Vector3.one, 1f);
    gameObject.LocalMoveAtSpeed(Vector3.one, 1f);
    // Scaling
    gameObject.ScaleTween(Vector3.one * 2f, 1f);
    // Fading
    gameObject.GetComponent<CanvasGroup>().Fade(0f, 1f);
    gameObject.GetComponent<Image>().ColorTween(Color.green, 1f);
    // Fill amount
    gameObject.GetComponent<Image>().FillAmountTween(1f, 1f);
    // Shaking effects
    gameObject.ShakePosition(Vector3.one, shakeCount: 8, t: 0.05f, fadeOut: false);
    gameObject.ShakeRotation(Vector3.one);
    gameObject.ShakeScale(Vector3.one);
    gameObject.ShakePositionAtSpeed(Vector3.one, shakeCount: 8, speed: 5f, fadeOut: false);

    // Method chaining
    gameObject.Move(Vector3.one, 1f)
        .SetEase(Ease.EaseOutQuad) // Set easing type
        .SetOnComplete(OnViewOpen) // Set completion callback
        .SetDelay(2f) // Set delay
        .SetEvent(OnViewOpen, 2.5f) // Set event to trigger at specific time
        .SetLoopType(LoopType.Yoyo, 3) // Set loop type (Restart, Flip, Incremental, Yoyo) and count
        .SetUpdateMode(UpdateMode.Update) // Set update mode (default is Update)
        .SetOwner(gameObject) // Set animation owner
        .SetIsPause(false); // Set pause state
        .SetIgnoreTimeScale(true); // Set whether to ignore timeScale
    
    // Pause control
    FF8.Tween.SetIsPause(id, true);
        
    // Alternative usage with OnUpdate
    // Numeric value tweening
    BaseTween valueTween = FF8.Tween.ValueTween(0f, 100f, 3f).SetOnUpdateFloat((float v) =>
    {
        LogF8.Log(v);
    });
    
    // Cancel animation by ID (base tween will be recycled but ID remains unique)
    int id2 = valueTween.ID;
    FF8.Tween.CancelTween(id2);
    
    // Object movement
    BaseTween gameObjectTween = FF8.Tween.Move(gameObject, Vector3.one, 3f).SetOnUpdateVector3((Vector3 v) =>
    {
        LogF8.Log(v);
    });
        
    // Alternative cancellation using owner object
    gameObjectTween.SetOwner(gameObject);
    FF8.Tween.CancelTween(gameObject);
    gameObject.CancelAllTweens();
    
    // UI movement using relative coordinates
    // (0.0 , 1.0) _______________________(1.0 , 1.0)
    //            |                      |
    //            |                      |                  
    //            |                      |
    //            |                      |
    // (0.0 , 0.0)|______________________|(1.0 , 0.0)
    transform.GetComponent<RectTransform>().MoveUI(new Vector2(1f, 1f), canvasRect, 1f)
        .SetEase(Ease.EaseOutBounce);
    
    
    /*----------------------------------------- Animation Sequences -----------------------------------------*/
    // Initialize sequence (sequential/parallel execution with callbacks)
    var sequence = SequenceManager.GetSequence();
    
    sequence.Append(valueTween); // First animation
    sequence.Join(gameObjectTween);   // Runs concurrently with first animation
    sequence.Append(valueTween); // Runs after first animation completes
    sequence.Append(() => LogF8.Log("Complete!")); // Callback after sequence
    sequence.SetOnComplete(() => LogF8.Log("Sequence complete"));
    sequence.SetIgnoreTimeScale(true); // Set whether to ignore timeScale
    
    // Set loop count (-1 for infinite)
    sequence.SetLoops(3);
    
    // Schedule events/animations at specific times
    sequence.RunAtTime(() => LogF8.Log("Mid-sequence event"), 1.5f); // Callback at 1.5s
    sequence.RunAtTime(gameObjectTween, 2.0f); // Start animation at 2.0s
    
    // Recycle sequence and stop all animations
    SequenceManager.KillSequence(sequence);
}

/*-------------------------------------Coroutine Animation Control-------------------------------------*/
IEnumerator Coroutine() {
    yield return gameObject.Move(Vector3.one, 1f);
    
    var sequence = SequenceManager.GetSequence();
    var baseTween = gameObject.Move(Vector3.one, 1f);
    sequence.Append(baseTween);
    yield return sequence;
}

/*-------------------------------------Async Animation Control-------------------------------------*/
async void Async() {
    await gameObject.Move(Vector3.one, 1f);
    
    var sequence = SequenceManager.GetSequence();
    var baseTween = gameObject.Move(Vector3.one, 1f);
    sequence.Append(baseTween);
    await sequence;
}
```


