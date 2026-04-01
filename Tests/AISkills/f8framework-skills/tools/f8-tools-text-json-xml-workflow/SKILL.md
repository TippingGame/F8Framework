---
name: f8-tools-text-json-xml-workflow
description: Use when working with Text/JSON/XML tools — text processing, LitJson, and XML utilities in F8Framework.
---

# Text/JSON/XML Tools Workflow



## Use this skill when

- The task involves JSON serialization/deserialization, XML parsing, or text processing.
- The user needs LitJson enhanced features or string manipulation.

## Path resolution

1. Source files:
   - Assets/F8Framework/Runtime/Utility/Text.cs (~10KB)
   - Assets/F8Framework/Runtime/Utility/LitJson.cs (~4KB)
   - Assets/F8Framework/Runtime/Utility/Xml.cs (~4KB)

## Sources of truth

- Source files: Text.cs, LitJson.cs, Xml.cs in Runtime/Utility

## Key capabilities

### Text (Text.cs)
- String formatting and manipulation
- Encoding conversion
- Text matching and searching
- String builder utilities

### LitJson (LitJson.cs)
- JSON serialization/deserialization (enhanced LitJson)
- Supports Unity types: Vector2/3/4, Quaternion, Color, Color32, Transform, GameObject, Bounds, Rect, Matrix4x4, etc.
- Dictionary key supports all basic and enum types
- HashSet support
- DateTime precision fix
- Long type fix

### XML (Xml.cs)
- XML reading and writing
- XML node navigation
- Attribute access

## Workflow

1. For JSON: use `JsonMapper.ToJson(obj)` / `JsonMapper.ToObject<T>(jsonString)`.
2. LitJson automatically handles Unity types — no special configuration.
3. For XML: use `Xml` class methods for reading/writing.
4. For text: use `Text` class utility methods.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| JSON serialization fails | Unsupported type | Check LitJson type support list |
| DateTime precision loss | Default DateTime handling | Already fixed in F8's LitJson |

## Output checklist

- Serialization method selected (JSON/XML/binary).
- Unity type support verified for JSON.
- Text processing patterns identified.
