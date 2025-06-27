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
    /*-------------------------------------Basic Animations-------------------------------------*/
    // Play animation with easing and completion callback
    int id = gameObject.ScaleTween(Vector3.one, 1f).SetEase(Ease.Linear).SetOnComplete(OnViewOpen).ID;

    void OnViewOpen()
    {
        // Animation complete handler
    }

    // Rotation
    gameObject.RotateTween(Vector3.one, 1f);
    // Position
    gameObject.Move(Vector3.one, 1f);
    gameObject.MoveAtSpeed(Vector3.one, 2f);
    gameObject.LocalMove(Vector3.one, 1f);
    gameObject.LocalMoveAtSpeed(Vector3.one, 1f);
    // Scale
    gameObject.ScaleTween(Vector3.one * 2f, 1f);
    // Fade
    gameObject.GetComponent<CanvasGroup>().Fade(0f, 1f);
    gameObject.GetComponent<Image>().ColorTween(Color.green, 1f);
    // Fill
    gameObject.GetComponent<Image>().FillAmountTween(1f, 1f);
    // Shake
    gameObject.ShakePosition(Vector3.one, shakeCount: 8, t: 0.05f, fadeOut: false);
    gameObject.ShakeRotation(Vector3.one);
    gameObject.ShakeScale(Vector3.one);
    gameObject.ShakePositionAtSpeed(Vector3.one, shakeCount: 8, speed: 5f, fadeOut: false);

    // Set delay
    gameObject.Move(Vector3.one, 1f).SetDelay(2f);
    
    // Set event at specific time
    gameObject.Move(Vector3.one, 5f).SetEvent(OnViewOpen, 2.5f);
    
    // Set loop type and count
    gameObject.Move(Vector3.one, 1f).SetLoopType(LoopType.Yoyo, 3);
    
    // Pause control
    gameObject.Move(Vector3.one, 1f).SetIsPause(true);
    FF8.Tween.SetIsPause(id, true);
    
    // Value tween with update callback
    BaseTween valueTween = FF8.Tween.ValueTween(0f, 100f, 3f).SetOnUpdateFloat((float v) =>
    {
        LogF8.Log(v);
    });
    
    // Cancel animation by ID
    int id2 = valueTween.ID;
    FF8.Tween.CancelTween(id2);
    
    // Object movement with update
    BaseTween gameObjectTween = FF8.Tween.Move(gameObject, Vector3.one, 3f).SetOnUpdateVector3((Vector3 v) =>
    {
        LogF8.Log(v);
    });
        
    // Cancel by owner object
    gameObjectTween.SetOwner(gameObject);
    FF8.Tween.CancelTween(gameObject);
    gameObject.CancelAllTweens();
    
    // UI relative position animation
    // (0.0 , 1.0) _______________________(1.0 , 1.0)
    //            |                      |
    //            |                      |                  
    //            |                      |
    //            |                      |
    // (0.0 , 0.0)|______________________|(1.0 , 0.0)
    transform.GetComponent<RectTransform>().MoveUI(new Vector2(1f, 1f), canvasRect, 1f)
        .SetEase(Ease.EaseOutBounce);
    
    
    /*-------------------------------------Animation Sequences-------------------------------------*/
    // Create animation sequence
    var sequence = SequenceManager.GetSequence();
    
    sequence.Append(valueTween); // First animation
    sequence.Join(gameObjectTween);   // Parallel animation
    sequence.Append(valueTween); // Runs after first completes
    sequence.Append(() => LogF8.Log("Complete!")); // Final callback
    sequence.SetOnComplete(() => LogF8.Log("Sequence complete"));
    
    // Set loop count (-1 for infinite)
    sequence.SetLoops(3);
    
    // Timed events in sequence
    sequence.RunAtTime(() => LogF8.Log("Mid-event"), 1.5f); // Callback at 1.5s
    sequence.RunAtTime(gameObjectTween, 2.0f); // Start animation at 2s
    
    // Cleanup sequence
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


