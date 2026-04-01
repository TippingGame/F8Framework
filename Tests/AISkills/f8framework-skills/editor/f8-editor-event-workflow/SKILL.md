---
name: f8-editor-event-workflow
description: Use when working with Event editor tools — event system monitor window for debugging active listeners in F8Framework.
---

# Event Editor Workflow



## Use this skill when

- The task involves debugging event listeners or tracking event dispatch in Editor.
- The user asks about the Event System Monitor window.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/Event

## Sources of truth

- Editor module: Assets/F8Framework/Editor/Event
- Test docs: Assets/F8Framework/Tests/Event/README.md

## Key editor features

| Feature | Description |
|---------|-------------|
| **Event System Monitor** | Editor window showing all registered listeners, their event IDs, and subscriber objects |

## Workflow

1. Open Event System Monitor from the F8Framework menu.
2. View all active event listener registrations.
3. Identify event IDs and which objects are listening.
4. Debug event dispatch issues by verifying listener presence.
5. Detect potential memory leaks from unremoved listeners.

## Output checklist

- Event monitor opened and reviewed.
- Listener registrations verified.
- Dead listeners identified and cleaned.
