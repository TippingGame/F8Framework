---
name: f8-features-exceltool-workflow
description: Use when implementing or troubleshooting ExcelTool feature workflows — config table loading, Excel binary/JSON generation, optional config data source registration, generated data assemblies, variant support, and runtime data access in F8Framework.
---

# ExcelTool Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first, then create the required module, for example `FF8.Config = ModuleCenter.CreateModule<F8DataManager>();`.


## Use this skill when

- The task is about config tables, Excel data reading, or data class generation.
- The user asks about manual Excel generation, F8 pipeline participation, or F7 runtime reload.
- The user needs to define new data types, containers, enums, or variants in Excel.
- Troubleshooting config loading failures or type mismatches.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. If F8Framework is installed as a package, use Packages/F8Framework.
3. For usage docs, read: Assets/F8Framework/Tests/ExcelTool/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/ExcelTool
- Core data source contract: Assets/F8Framework/Runtime/ConfigData/ConfigDataSourceRegistry.cs
- Editor module: Assets/F8Framework/Editor/ExcelTool
- Generated data classes: Assets/F8Framework/ConfigData
- Excel source: Assets/StreamingAssets/config/*.xlsx
- Test docs: Assets/F8Framework/Tests/ExcelTool

## Key classes and interfaces

| Class | Role |
|-------|------|
| `F8DataManager` | Generated config module. Access via `FF8.Config`. Loads and queries config data. |
| `IConfigDataSource` | Core contract implemented by optional config source modules. |
| `ConfigDataSourceRegistry` | Selects the highest-priority available config source without Core depending on Excel. |
| `ExcelConfigDataSource` | Excel runtime implementation registered before scene load. |
| `ReadExcel` | Reads Excel files. Extensible for custom types. |

## API quick reference

### Loading config data
```csharp
// Load single sheet by name
Sheet1 sheet1 = FF8.Config.Load<Sheet1>("Sheet1");
// Sync load all
FF8.Config.LoadAll();
// Async load all (coroutine)
yield return FF8.Config.LoadAllAsyncIEnumerator();
// Async load all (async/await — WebGL compatible)
await FF8.Config.LoadAllAsyncTask();
// Runtime Excel reload (development only)
FF8.Config.RuntimeLoadAll();
```

### Querying data
```csharp
// Get single row by ID
LogF8.Log(FF8.Config.GetSheet1ByID(2).name);
// Iterate all rows
foreach (var item in FF8.Config.GetSheet1())
{
    LogF8.Log(item.Key);
    LogF8.Log(item.Value.name);
}
```

### Variant support (multi-language/multi-version configs)
```csharp
// Set global variant name
FF8.Config.VariantName = "English";
// Set per-sheet variant name (higher priority)
Sheet1.VariantName = "English";
```

## Excel type system

### Basic types
`char`, `bool`, `byte`, `short`, `int`, `long`, `float`, `double`, `decimal`, `str/string`, `obj/object`, `datetime`, `sbyte`, `ushort`, `uint`, `ulong`
Unity types: `vec2/vector2`, `vec3/vector3`, `vec4/vector4`, `vec2int/vector2int`, `vec3int/vector3int`, `quat/quaternion`, `color`, `color32`, `matrix4x4`
Standard JSON specification type: `json`

### Container types
- Arrays: `int[]`, `string[][]`, `obj[][][]`
- Lists: `list<obj>`
- Dictionaries: `dict<int,list<string>>`
- ValueTuples: `valuetuple<int,string>` (up to 7 types)
- HashSets: `hashset<int>`

### Special types
- Enums: `enum<name,int,Flags>{Value1=1,Value2=2}`
- Variants: `variant<name,variantName>` — switch configs by variant name

### Row/column exclusion
- Skip entire row: leave `id` empty
- Skip entire column: leave `type` or `name` empty

## Workflow

1. Create Excel files in `Assets/StreamingAssets/config/` directory.
2. Set first column as `int` type `id` (unique primary key).
3. Row 1 = types, Row 2 = field names, Row 3+ = data.
4. Generate data with one of these entries:
   - Excel only: menu **开发工具 → 2: Excel导表-F8**.
   - Full F8Run: press **F8** while pre-build Excel generation is enabled.
   - Player/update build: use F5 while pre-build Excel generation is enabled.
5. Let `F8EditorPipeline` compile and resume automatically when generated C# changes.
6. Find generated files in `Assets/F8Framework/ConfigData/` and `Assets/AssetBundles/Config/BinConfigData/`.
7. Initialize `F8DataManager`, then call `FF8.Config.LoadAll()` before accessing data.
8. Press **F7** for runtime Excel reload (development only).
9. For variants, add `variant<fieldName,variantName>` columns.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Compilation errors after import | Naming conflicts in generated classes | Fix the Excel schema or conflicting generated names and regenerate; do not delete the entire `ConfigData/` directory |
| Data class not found | Sheet name mismatch | Ensure sheet name matches exactly (case-sensitive) |
| No available config data source | Excel runtime module is absent, unregistered, or unavailable | Install/enable a compatible `IConfigDataSource` implementation and verify its source path |
| Android runtime Excel read fails | StreamingAssets not directly readable | Use `SyncStreamingAssetsLoader` or binary cache mode |
| Dictionary key error in JSON export | Container key type not supported in JSON | Use `binary` format which supports all key types |

## Cross-module dependencies

- **AssetManager**: Binary/JSON config files loaded via `FF8.Asset`.
- **Core**: Owns only `IConfigDataSource` and `ConfigDataSourceRegistry`; it must not reference Excel runtime/editor assemblies.
- **Generated assembly**: `F8Framework.F8ExcelDataClass` is separate from ExcelTool and is referenced by Launcher; retain it or refactor consumers before removing it.
- **Localization**: Can use variant type as lightweight localization alternative.

## Output checklist

- Excel structure defined (types, names, data).
- Manual import or optional F8/F5 integration verified.
- Config data source registration and availability verified.
- Data access code validated.
- Validation status and remaining risks.
