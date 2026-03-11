---
name: f8-features-tween-workflow
description: Use when implementing or troubleshooting Tween feature workflows — tween animations, sequences, chain calls, UI relative motion, and coroutine/async support in F8Framework.
---

# Tween Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about tween/interpolation animations (move, rotate, scale, fade, etc.).
- The user asks about animation sequencing, loop types, or UI-relative motion.
- Troubleshooting tween timing, easing, or auto-kill behavior.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Tween/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Tween
- Test docs: Assets/F8Framework/Tests/Tween

## Key classes and interfaces

| Class | Role |
|-------|------|
| `TweenManager` | Core module. Access via `FF8.Tween`. |
| `BaseTween` | Tween handle with chain methods. |
| `SequenceManager` | Creates and manages animation sequences. |
| `Ease` | Easing curves (Linear, EaseOutQuad, EaseOutBounce, etc.). |
| `LoopType` | Restart, Flip, Incremental, Yoyo. |

## API quick reference

### Basic tweens (extension methods on GameObject)
```csharp
gameObject.ScaleTween(Vector3.one * 2f, 1f);           // Scale
gameObject.RotateTween(Vector3.one, 1f);                // Rotate
gameObject.EulerAnglesTween(Vector3.one * 360f, 1f);    // Euler rotation
gameObject.Move(Vector3.one, 1f);                       // World move
gameObject.MoveAtSpeed(Vector3.one, 2f);                // Move at speed
gameObject.LocalMove(Vector3.one, 1f);                  // Local move
gameObject.LocalMoveAtSpeed(Vector3.one, 1f);           // Local move at speed
gameObject.GetComponent<CanvasGroup>().Fade(0f, 1f);    // Fade
gameObject.GetComponent<Image>().ColorTween(Color.green, 1f); // Color
gameObject.GetComponent<Image>().FillAmountTween(1f, 1f);     // Fill
```

### Shake
```csharp
gameObject.ShakePosition(Vector3.one, shakeCount: 8, t: 0.05f, fadeOut: false);
gameObject.ShakePositionAtSpeed(Vector3.one, shakeCount: 8, speed: 5f, fadeOut: false);
gameObject.ShakeRotation(Vector3.one);
gameObject.ShakeScale(Vector3.one);
```

### Path
```csharp
gameObject.PathTween(points, duration: 1f, pathType: PathType.CatmullRom,
    pathMode: PathMode.Ignore, resolution: 10, closePath: false);
```

### String
```csharp
text.StringTween("", "Hello!", 1f, richTextEnabled: true,
    ScrambleMode.Custom, scrambleChars: "*");
```

### Chain calls
```csharp
BaseTween tween = gameObject.Move(Vector3.one, 10f)
    .SetEase(Ease.EaseOutQuad)
    .SetOnComplete(OnDone)
    .SetDelay(2f)
    .SetEvent(OnMidway, 2.5f)
    .SetLoopType(LoopType.Yoyo, 3)
    .SetUpdateMode(UpdateMode.Update)
    .SetOwner(gameObject)
    .SetIgnoreTimeScale(true)
    .SetCustomId("myTween")
    .SetAutoKill(false);
```

### Tween control (via FF8.Tween or BaseTween)
```csharp
FF8.Tween.SetCurrentTime(id, 5f);       // By id
FF8.Tween.SetCurrentTime("customId", 5f); // By custom id
FF8.Tween.SetProgress(id, 0.5f);
FF8.Tween.Complete(id);
FF8.Tween.ReplayReset(id);
FF8.Tween.SetIsPause(id, true);
FF8.Tween.CancelTween(id);
gameObject.CancelAllTweens();
```

### Value tweens
```csharp
FF8.Tween.ValueTween(0f, 100f, 3f).SetOnUpdateFloat((float v) => { });
FF8.Tween.Move(gameObject, Vector3.one, 3f).SetOnUpdateVector3((Vector3 v) => { });
```

### UI relative motion
```csharp
// Move UI by viewport-relative coordinates (0,0)=bottom-left, (1,1)=top-right
rectTransform.MoveUI(new Vector2(1f, 1f), canvasRect, 1f)
    .SetEase(Ease.EaseOutBounce);
```

### Sequences
```csharp
var seq = SequenceManager.GetSequence();
seq.Append(tween1);                   // Sequential
seq.Join(tween2);                     // Parallel with previous
seq.Append(() => LogF8.Log("Done"));  // Callback
seq.SetOnComplete(() => { });
seq.SetLoops(3);                      // -1 = infinite
seq.RunAtTime(() => { }, 1.5f);       // Event at specific time
seq.RunAtTime(tween3, 2.0f);          // Tween at specific time
SequenceManager.KillSequence(seq);    // Stop and recycle
```

### Coroutine and async support
```csharp
// Coroutine
yield return gameObject.Move(Vector3.one, 1f);
yield return sequence;

// async/await
await gameObject.Move(Vector3.one, 1f);
await sequence;
```

## Workflow

1. Choose tween type (move, rotate, scale, fade, path, etc.).
2. Chain configuration methods for easing, delay, loop.
3. Use sequences for complex multi-step animations.
4. For UI, use `MoveUI()` with canvas-relative coordinates.
5. Use `SetAutoKill(false)` if you need to replay or hold references.
6. Cancel/complete tweens when game state changes.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Tween reference is stale | Auto-killed and recycled | Use `SetAutoKill(false)` |
| UI animation jumps | Wrong coordinate space | Use MoveUI for viewport-relative |
| Tween not playing during pause | TimeScale is 0 | Use `SetIgnoreTimeScale(true)` |

## Cross-module dependencies

- **UI**: Used for panel open/close animations in BaseView.

## Output checklist

- Tween type and parameters selected.
- Easing and loop configured.
- Sequence order defined if multi-step.
- Validation status and remaining risks.
