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
// Canvas RectTransform
public RectTransform canvasRect;

void Start()
{
    /*-----------------------------------------Basic Usage-----------------------------------------*/
    // Play animation, set Ease curve, set OnComplete callback
    int id = gameObject.ScaleTween(Vector3.one, 1f).SetEase(Ease.Linear).SetOnComplete(OnViewOpen).ID;

    void OnViewOpen()
    {

    }

    // Rotation
    gameObject.RotateTween(Vector3.one, 1f);
    // Movement
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
    // Shake effects
    gameObject.ShakePosition(Vector3.one, shakeCount: 8, t: 0.05f, fadeOut: false);
    gameObject.ShakeRotation(Vector3.one);
    gameObject.ShakeScale(Vector3.one);
    gameObject.ShakePositionAtSpeed(Vector3.one, shakeCount: 8, speed: 5f, fadeOut: false);
    // Path animation
    gameObject.PathTween(new Vector3[] { Vector3.zero, Vector3.one * 100f, Vector3.one * 200f }, duration: 1f, pathType: PathType.CatmullRom,
        pathMode: PathMode.Ignore, resolution: 10, closePath: false);
    gameObject.LocalPathTween(new Vector3[] { Vector3.zero, Vector3.one * 100f, Vector3.one * 200f }, duration: 1f);
    // String animation
    gameObject.GetComponent<Text>().StringTween("", "Hello World!", 1f, richTextEnabled: true, ScrambleMode.Custom, scrambleChars: "*");
    
    // Method chaining
    BaseTween baseTween = gameObject.Move(Vector3.one, 10f)
        .SetEase(Ease.EaseOutQuad) // Set Ease curve
        .SetOnComplete(OnViewOpen) // Set completion callback
        .SetDelay(2f) // Set delay
        .SetEvent(OnViewOpen, 2.5f) // Set event to be called at specific time
        .SetLoopType(LoopType.Yoyo, 3) // Set loop type (Restart, Flip, Incremental, Yoyo) and loop count
        .SetUpdateMode(UpdateMode.Update) // Set update mode, default is Update
        .SetOwner(gameObject) // Set animation owner
        .SetIsPause(false) // Set pause state
        .SetIgnoreTimeScale(true) // Set whether to ignore time scale
        .SetCustomId("customId") // Set custom ID
        .SetCurrentTime(5f) // Set current time
        .SetProgress(0.5f) // Set current progress
        .SetAutoKill(false) // Default is true, automatically recycles after use, held BaseTween may have been reused
        .Complete() // Complete animation immediately
        .ReplayReset(); // Replay animation
    
    baseTween.CurrentTime = 5f;  // Set or get current time
    baseTween.Progress = 0.5f;  // Set or get current progress
    
    // Only use ID to control animations, because animations auto-recycle by default. Use SetAutoKill(false) to disable auto-recycle
    // Set custom ID
    FF8.Tween.SetCustomId(id, "customId");
    
    // Set current time (using id, customId, or gameObject)
    FF8.Tween.SetCurrentTime(id, 5f);
    FF8.Tween.SetCurrentTime("customId", 5f);
    FF8.Tween.SetCurrentTime(gameObject, 5f);
    
    // Set current progress (same as above)
    FF8.Tween.SetProgress(id, 0.5f);
    
    // Complete animation immediately (same as above)
    FF8.Tween.Complete(id);
    
    // Replay animation (same as above)
    FF8.Tween.ReplayReset(id);
    
    // Set pause state (same as above)
    FF8.Tween.SetIsPause(id, true);
    
    // Set ignore time scale (same as above)
    FF8.Tween.SetIgnoreTimeScale(id, true);
    
    // Cancel animation (same as above)
    int id2 = baseTween.ID;
    FF8.Tween.CancelTween(id2);
    gameObject.CancelTween(id2);
    gameObject.CancelAllTweens();
    
    // You can also use it like this, setting OnUpdate
    // Numeric value tweening
    BaseTween valueTween = FF8.Tween.ValueTween(0f, 100f, 3f).SetOnUpdateFloat((float v) =>
    {
        LogF8.Log(v);
    });
    
    // GameObject movement
    BaseTween gameObjectTween = FF8.Tween.Move(gameObject, Vector3.one, 3f).SetOnUpdateVector3((Vector3 v) =>
    {
        LogF8.Log(v);
    });
    
    // Move UI based on relative coordinates
    // (0.0 , 1.0) _______________________(1.0 , 1.0)
    //            |                      |
    //            |                      |                  
    //            |                      |
    //            |                      |
    // (0.0 , 0.0)|______________________|(1.0 , 0.0)
    transform.GetComponent<RectTransform>().MoveUI(new Vector2(1f, 1f), canvasRect, 1f)
        .SetEase(Ease.EaseOutBounce);
    
    
    /*-----------------------------------------Animation Sequences-----------------------------------------*/
    // Initialize, execute animations sequentially/parallel, with callbacks
    var sequence = SequenceManager.GetSequence();
    
    sequence.Append(valueTween); // First animation
    sequence.Join(gameObjectTween);   // Execute simultaneously with first animation
    sequence.Append(valueTween); // Execute after second animation completes
    sequence.Append(() => LogF8.Log("Completed!")); // Callback after animation sequence ends
    sequence.SetOnComplete(() => LogF8.Log("Sequence completed"));
    sequence.SetIgnoreTimeScale(true); // Set whether to ignore time scale
    
    // Set loop count, -1 represents infinite loops
    sequence.SetLoops(3);
    
    // Run events or animations at specific times
    sequence.RunAtTime(() => LogF8.Log("Midway event"), 1.5f); // Execute callback at 1.5 seconds
    sequence.RunAtTime(gameObjectTween, 2.0f); // Start specific animation at 2 seconds
    
    // Recycle Sequence and stop all animations
    SequenceManager.KillSequence(sequence);
}

/*-----------------------------------------Using Coroutines to Wait for Animations and Sequences-----------------------------------------*/
IEnumerator Coroutine() {
    yield return gameObject.Move(Vector3.one, 1f);
    
    var sequence = SequenceManager.GetSequence();
    var baseTween = gameObject.Move(Vector3.one, 1f);
    sequence.Append(baseTween);
    yield return sequence;
}

/*-----------------------------------------Using async/await to Wait for Animations and Sequences-----------------------------------------*/
async void Async() {
    await gameObject.Move(Vector3.one, 1f);
    
    var sequence = SequenceManager.GetSequence();
    var baseTween = gameObject.Move(Vector3.one, 1f);
    sequence.Append(baseTween);
    await sequence;
}
```


