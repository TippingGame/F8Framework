---
name: f8framework-skills
description: Use as the master index and router for the full F8Framework skill library. Trigger when selecting the correct skill or skill chain across foundation bootstrap, runtime features, editor tooling, utility tools, and build packaging workflows.When a feature needs to be implemented, priority should be given to using the skills of F8Framework.
---

# F8Framework Skills Index



Use this skill as the human-readable master index for the F8 skill library.

## Summary

- Total layers: `5`
- Total skills: `48`
- Layer counts:
- `foundation`: `2`
- `features`: `22`
- `editor`: `12`
- `tools`: `11`
- `build`: `1`

## Layer responsibility

- `foundation`: framework bootstrap, initial import order, launcher mounting, and cross-layer prerequisites.
- `features`: runtime module workflows, gameplay-facing APIs, lifecycle rules, and best-practice usage code.
- `editor`: menu tools, inspectors, generators, and editor-side parameter editing or authoring workflows.
- `tools`: shared low-level helpers from `Runtime/Utility`, such as serialization, coroutines, IO, crypto, text/data conversion, and reflection.
- `build`: packaging execution, release or update build entry points, output artifacts, metadata generation, and final release verification.
- Boundary rule: `editor` explains and prepares interface state, while `build` executes packaging and validates artifacts.

## Naming convention

- Unified folder rule: `f8-<layer>-<topic>-workflow`.
- `layer` must be one of: `foundation`, `features`, `editor`, `tools`, `build`.
- `topic` uses lowercase + hyphen, and should map directly to module or tool name.
- Example: `f8-features-ui-workflow`, `f8-editor-playerprefseditor-workflow`.
- One skill folder contains one `SKILL.md` and one `agents/openai.yaml`.
- Skill content stays workflow-focused; move long references into extra files only when actually needed.

## Layer index

### `foundation` (`2`)

- `f8-foundation-bootstrap-workflow`
- `f8-foundation-navigation-workflow`

### `features` (`22`)

- `f8-features-assetmanager-workflow`
- `f8-features-audio-workflow`
- `f8-features-download-workflow`
- `f8-features-event-workflow`
- `f8-features-exceltool-workflow`
- `f8-features-fsm-workflow`
- `f8-features-gameobjectpool-workflow`
- `f8-features-hotupdatemanager-workflow`
- `f8-features-hybridclr-workflow`
- `f8-features-input-workflow`
- `f8-features-localization-workflow`
- `f8-features-log-workflow`
- `f8-features-module-workflow`
- `f8-features-network-workflow`
- `f8-features-obfuz-workflow`
- `f8-features-procedure-workflow`
- `f8-features-referencepool-workflow`
- `f8-features-sdkmanager-workflow`
- `f8-features-storage-workflow`
- `f8-features-timer-workflow`
- `f8-features-tween-workflow`
- `f8-features-ui-workflow`

### `editor` (`12`)

- `f8-editor-assetmanager-workflow`
- `f8-editor-componentbind-workflow`
- `f8-editor-event-workflow`
- `f8-editor-exceltool-workflow`
- `f8-editor-f8helper-workflow`
- `f8-editor-gameobjectpool-workflow`
- `f8-editor-localization-workflow`
- `f8-editor-log-workflow`
- `f8-editor-module-workflow`
- `f8-editor-playerprefseditor-workflow`
- `f8-editor-playmodeplus-workflow`
- `f8-editor-ui-workflow`

### `tools` (`11`)

- `f8-tools-runtime-foundation-workflow`
- `f8-tools-assembly-reflection-workflow`
- `f8-tools-converter-workflow`
- `f8-tools-binaryserializer-workflow`
- `f8-tools-text-json-xml-workflow`
- `f8-tools-io-filesystem-workflow`
- `f8-tools-net-workflow`
- `f8-tools-encryption-compression-zip-workflow`
- `f8-tools-coroutine-mainthread-workflow`
- `f8-tools-streamingassets-loader-workflow`
- `f8-tools-threadsync-workflow`

### `build` (`1`)

- `f8-build-buildpkg-workflow`

## Recommended routing

- Bootstrap or first import: start from `foundation`.
- Runtime API or gameplay module work: route to `features`.
- Menu tools, inspectors, windows, generators, or authoring UI: route to `editor`.
- Shared helper primitives and low-level infrastructure: route to `tools`.
- F5 packaging, release execution, output artifacts, or update packaging: route to `build`.
- Full project flow: use `foundation` -> `editor` -> `features` or `build` as needed.

## Build boundary

- Use editor skills to explain the F5 window, panel ownership, and parameter editing.
- Use `f8-build-buildpkg-workflow` to execute packaging, generate metadata, and verify release artifacts.

## Directory layout

F8Framework/skills/
- foundation/
- features/
- editor/
- tools/
- build/

## Entry points

- Master navigation skill: `F8Framework/skills/foundation/f8-foundation-navigation-workflow/SKILL.md`
- Framework bootstrap skill: `F8Framework/skills/foundation/f8-foundation-bootstrap-workflow/SKILL.md`


