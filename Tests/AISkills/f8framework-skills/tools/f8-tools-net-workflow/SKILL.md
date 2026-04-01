---
name: f8-tools-net-workflow
description: Use when working with Net tools — HTTP request helpers and web utility functions in F8Framework.
---

# Net Tools Workflow



## Use this skill when

- The task involves HTTP requests, web utilities, or URL manipulation.
- The user needs simple HTTP GET/POST without full networking module.

## Path resolution

1. Source: Assets/F8Framework/Runtime/Utility/Net.cs (~7KB)

## Sources of truth

- Source file: Assets/F8Framework/Runtime/Utility/Net.cs

## Key capabilities

- HTTP GET/POST requests
- URL encoding/decoding
- Web request utilities
- Response parsing
- Timeout handling

## Workflow

1. Use `Util.Net` (or similar) class methods for HTTP operations.
2. For persistent connections, use the Network module instead.
3. Net.cs is for simple one-off HTTP requests.
4. Handle timeouts and network errors in callbacks.

## Comparison with Network module

| Feature | Net (Tools) | Network (Features) |
|---------|-------------|-------------------|
| Use case | Simple HTTP requests | Persistent TCP/KCP/WebSocket |
| Protocol | HTTP only | TCP, KCP, WebSocket |
| Connection | One-off | Persistent |
| Module init | Not needed | Required |

## Output checklist

- HTTP method selected (GET/POST).
- Timeout configured.
- Response handling implemented.
