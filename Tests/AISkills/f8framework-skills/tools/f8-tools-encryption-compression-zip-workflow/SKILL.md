---
name: f8-tools-encryption-compression-zip-workflow
description: Use when working with Encryption/Compression/Zip tools — AES encryption, data compression, and ZIP archive management in F8Framework.
---

# Encryption/Compression/Zip Tools Workflow



## Use this skill when

- The task involves data encryption (AES), compression, or ZIP operations.
- The user needs to protect data, compress assets, or handle ZIP archives.

## Path resolution

1. Source files:
   - Assets/F8Framework/Runtime/Utility/Encryption.cs (~21KB)
   - Assets/F8Framework/Runtime/Utility/Compression.cs (~6KB)
   - Assets/F8Framework/Runtime/Utility/ZipHelper.cs (~12KB)

## Sources of truth

- Source files: Encryption.cs, Compression.cs, ZipHelper.cs in Runtime/Utility
- Third-party: ICSharpCode.SharpZipLib (built-in)

## Key capabilities

### Encryption (Encryption.cs)
- `OptimizedAES` — AES-256 encryption/decryption
- Key and IV management
- Random IV generation (null IV = random)
- Used by StorageManager for local data encryption
- Used by AssetManager for AB encryption

```csharp
// Example: Create encryptor
var aes = new Util.OptimizedAES(key: "AES_Key", iv: null);
byte[] encrypted = aes.Encrypt(plainBytes);
byte[] decrypted = aes.Decrypt(encrypted);
```

### Compression (Compression.cs)
- Data compression/decompression
- GZip format support
- Used for reducing data transmission size

### Zip (ZipHelper.cs)
- ZIP archive creation and extraction
- File-level ZIP operations
- Used by SyncStreamingAssetsLoader for Android StreamingAssets access
- Based on ICSharpCode.SharpZipLib

## Workflow

1. For encryption: create `OptimizedAES` instance with key.
2. For compression: use Compression class static methods.
3. For ZIP: use ZipHelper for archive operations.
4. Storage module uses encryption automatically when configured.
5. No module initialization needed — pure utility classes.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Decryption fails | Key mismatch | Ensure same key across encrypt/decrypt |
| ZIP extraction fails | Corrupt archive | Verify ZIP integrity before extraction |
| Compression ratio poor | Already compressed data | Check if data is already compressed |

## Output checklist

- Encryption/compression/ZIP method selected.
- Keys and parameters configured.
- Error handling for crypto/IO failures.
