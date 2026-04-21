# [3.1.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v3.0.4...v3.1.0) (2026-03-10)


### Bug Fixes

* adding count == 0 check to SendAll before taking ArrayBuffer ([a43c50c](https://github.com/James-Frowen/SimpleWebTransport/commit/a43c50c732eff53cc28f1774efd7b8d1fae65e61))
* adding thread safe send method to ensure buffer is released ([c0fc993](https://github.com/James-Frowen/SimpleWebTransport/commit/c0fc99362dcbb50234fd7b6e24ac8dd143871060))
* releasing buffers in fragment queue incase loop stops in the middle of a large message ([455d594](https://github.com/James-Frowen/SimpleWebTransport/commit/455d594da2b232fbbc263938d2e427a9e0aa61bb))
* simplify legacy SendAll and calling release inside legacy Send ([6e7f43a](https://github.com/James-Frowen/SimpleWebTransport/commit/6e7f43ab7d22284858a33972f9bb6be5a63267f7))
* using auto dispose to safely release buffers ([c8301d7](https://github.com/James-Frowen/SimpleWebTransport/commit/c8301d71b377f452df0b53232ce61e7953185e79))


### Features

* adding maxSendQueueSize for server to limit how many pending sends each connection can have ([1dda1b0](https://github.com/James-Frowen/SimpleWebTransport/commit/1dda1b0272fb2d0f8f35bfeb1d1c17ee28490c64))

## [3.0.4](https://github.com/James-Frowen/SimpleWebTransport/compare/v3.0.3...v3.0.4) (2026-03-01)


### Bug Fixes

* fixing unity removing new Map ([51e37cb](https://github.com/James-Frowen/SimpleWebTransport/commit/51e37cbec18014a667cc7706d4e8693d749dde08))

## [3.0.3](https://github.com/James-Frowen/SimpleWebTransport/compare/v3.0.2...v3.0.3) (2026-03-01)


### Bug Fixes

* adding SimpleWeb_ prefix to avoid name collision with other jslib ([0c51ad9](https://github.com/James-Frowen/SimpleWebTransport/commit/0c51ad9b280f92e5ffb726c3c97ee508931a09df))
* calling RemoveSocket from onclose to clean up js connections ([7863fe1](https://github.com/James-Frowen/SimpleWebTransport/commit/7863fe13322ef7e134df09bd2454e0f7f6f2e1d6))


### Performance Improvements

* using Map instead of array to avoid memory growth for long lived servers ([fff36f1](https://github.com/James-Frowen/SimpleWebTransport/commit/fff36f1b79d94eb6dcc7ae7faef80492e929c214))

## [3.0.2](https://github.com/James-Frowen/SimpleWebTransport/compare/v3.0.1...v3.0.2) (2026-02-26)


### Bug Fixes

* improving handling of pointer free ([1371b69](https://github.com/James-Frowen/SimpleWebTransport/commit/1371b69ae1d690a46cb06ba513dc0dca5ec6f788))

## [3.0.1](https://github.com/James-Frowen/SimpleWebTransport/compare/v3.0.0...v3.0.1) (2026-02-25)


### Bug Fixes

* fixing negative pointers with WebAssembly2023 ([ffa9ece](https://github.com/James-Frowen/SimpleWebTransport/commit/ffa9ece192580778570ccc82fd8370fa64b2eb6d))
* using #if to avoid compile error outside of unity ([e0a7539](https://github.com/James-Frowen/SimpleWebTransport/commit/e0a7539c4408b51659cc28bc74ec97b04d90fc36))


### Performance Improvements

* removing allocation inside webSocket.onmessage ([b309f64](https://github.com/James-Frowen/SimpleWebTransport/commit/b309f642c0a8cbe9db68fc005ff140e0de7e9d11))

# [3.0.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.3.3...v3.0.0) (2026-01-04)


* feat!: adding IConnection as opaque reference to use instead of int id ([b832298](https://github.com/James-Frowen/SimpleWebTransport/commit/b832298ae6c41ad28351e95c3feb33771065ac87))
* refactor!: changing some ArraySegment to Span ([12d6873](https://github.com/James-Frowen/SimpleWebTransport/commit/12d6873e5930b03452b87d02839dc8f52fb452d8))
* refactor!: removing MonoBehaviour from ProcessMessageQueue ([b634c35](https://github.com/James-Frowen/SimpleWebTransport/commit/b634c35404aa85ef1ed6c8d7d03373aa873f6aa9))


### BREAKING CHANGES

* most methods now use IConnection instead of int id
* ProcessMessageQueue now takes Func<bool> keepProcessing instead of MonoBehaviour
* - Send method now use ReadOnlySpan<byte> instead of ArraySegment<byte>
- CopyTo and CopyFrom methods in ArrayBuffer now use Span<byte>
- jsLib function no longer passing offset

## [2.3.3](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.3.2...v2.3.3) (2025-11-25)


### Bug Fixes

* using makeDynCall macro to support unity 6 ([a524c07](https://github.com/James-Frowen/SimpleWebTransport/commit/a524c07ffcb353a8defe7c74e0f6a9a50da88bfe))

## [2.3.2](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.3.1...v2.3.2) (2025-09-27)


### Bug Fixes

* skipping zero length check for Ping/Pong opcodes ([0f9ee38](https://github.com/James-Frowen/SimpleWebTransport/commit/0f9ee385f6dce827246a5da2d4e19de6080156bd)), closes [#20](https://github.com/James-Frowen/SimpleWebTransport/issues/20)

## [2.3.1](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.3.0...v2.3.1) (2025-09-12)


### Bug Fixes

* do not parse port on reverse proxy ([#19](https://github.com/James-Frowen/SimpleWebTransport/issues/19)) ([2fa1407](https://github.com/James-Frowen/SimpleWebTransport/commit/2fa14074a32a88edb2a761b5641cea335f950046))

# [2.3.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.2.3...v2.3.0) (2025-08-02)


### Features

* adding function to get remotePort ([#17](https://github.com/James-Frowen/SimpleWebTransport/issues/17)) ([0af2ec0](https://github.com/James-Frowen/SimpleWebTransport/commit/0af2ec0f44520f2f11436daca49e74f20cb8561e))

## [2.2.3](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.2.2...v2.2.3) (2025-04-27)


### Bug Fixes

* adding ping/pong support to avoid error on receive ([0828049](https://github.com/James-Frowen/SimpleWebTransport/commit/0828049f4c33272047d83916e2f98b483c83f0ec))

## [2.2.2](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.2.1...v2.2.2) (2024-10-01)


### Bug Fixes

* using on events instead of addEventListener ([a342e9f](https://github.com/James-Frowen/SimpleWebTransport/commit/a342e9f5ff4ccd4323af6da812cffc38f744edbd))

## [2.2.1](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.2.0...v2.2.1) (2024-07-08)


### Bug Fixes

* increasing default handshake max size ([5a8aaa6](https://github.com/James-Frowen/SimpleWebTransport/commit/5a8aaa6398181a38d3ac0a0cc14008cc8dd00c32))

# [2.2.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.1.1...v2.2.0) (2024-04-24)


### Features

* adding versions of SendAll that accept ICollection or IEnumerable ([28b6b25](https://github.com/James-Frowen/SimpleWebTransport/commit/28b6b25f7c1bf26cef04f5bae25377b485eec1ae))

## [2.1.1](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.1.0...v2.1.1) (2024-04-09)


### Bug Fixes

* removing invalid auto import ([a37bc02](https://github.com/James-Frowen/SimpleWebTransport/commit/a37bc023a5de45cac8740f6e6d62e60b04a25335))

# [2.1.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.0.1...v2.1.0) (2024-04-09)


### Features

* adding option to allow ssl errors. Useful when testing with self signed cert ([d956089](https://github.com/James-Frowen/SimpleWebTransport/commit/d9560893c1f8ac2f38a3dd1baf462dea7b42aaff))

## [2.0.1](https://github.com/James-Frowen/SimpleWebTransport/compare/v2.0.0...v2.0.1) (2024-03-16)


### Bug Fixes

* updating unity version in package ([b4f1f26](https://github.com/James-Frowen/SimpleWebTransport/commit/b4f1f260311c431b10221a104bf14a50342cde41))

# [2.0.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.6.5...v2.0.0) (2024-03-16)


* fix!: removing Runtime from global scope ([2e28322](https://github.com/James-Frowen/SimpleWebTransport/commit/2e283225e53c8f1a90f7504d11811d501c2f7f2b))


### BREAKING CHANGES

* no longer supports unity 2020 or earlier

## [1.6.5](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.6.4...v1.6.5) (2024-03-14)


### Bug Fixes

* stopping websocket variable being put in global scope ([074d85b](https://github.com/James-Frowen/SimpleWebTransport/commit/074d85b907d3f9d7e303efdda55295ed9be0678f))

## [1.6.4](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.6.3...v1.6.4) (2023-07-15)


### Bug Fixes

* improving error message from opcode ([41cbed0](https://github.com/James-Frowen/SimpleWebTransport/commit/41cbed01932828640dfac2bb3c41f1f583b2af54))

## [1.6.3](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.6.2...v1.6.3) (2023-06-09)


### Bug Fixes

* converting endpoint to ipv4 ([49340c2](https://github.com/James-Frowen/SimpleWebTransport/commit/49340c2bef3a6ee334cb985431dd2ccbfc7c749e))

## [1.6.2](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.6.1...v1.6.2) (2023-06-08)


### Performance Improvements

* caching remoteAddress ([d2f63ba](https://github.com/James-Frowen/SimpleWebTransport/commit/d2f63ba895ab1936eb830a649a5e7a78503f13ed))

## [1.6.1](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.6.0...v1.6.1) (2023-06-08)


### Bug Fixes

* adding missing function from mirror example ([c15d060](https://github.com/James-Frowen/SimpleWebTransport/commit/c15d060be3b4e832649ea9737d3f7975d116a180))

# [1.6.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.5.0...v1.6.0) (2023-06-08)


### Features

* storing all headers instead of just ([cf90ab6](https://github.com/James-Frowen/SimpleWebTransport/commit/cf90ab6f30719242e9e93aa7c282d123a08bf423))

# [1.5.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.4.1...v1.5.0) (2023-06-08)


### Features

* adding option to change ip real header ([f50f4b2](https://github.com/James-Frowen/SimpleWebTransport/commit/f50f4b2299d36eac30f02b441745b4a4a4242680))

## [1.4.1](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.4.0...v1.4.1) (2023-06-08)


### Bug Fixes

* fixing AssemblyInfo version ([ee8ebb3](https://github.com/James-Frowen/SimpleWebTransport/commit/ee8ebb3ad9608228b69392f52aec99c1cc12848b))

# [1.4.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.3.2...v1.4.0) (2023-06-08)


### Bug Fixes

* using realIp for tostring ([db9ffe6](https://github.com/James-Frowen/SimpleWebTransport/commit/db9ffe6d8b5e581b43e5c599610671c6a0efe732))


### Features

* adding option to get real ip from reverse proxy ([7d8acaa](https://github.com/James-Frowen/SimpleWebTransport/commit/7d8acaaf9c2bbc3b09bbc0f11b33463f6a6a3f0d))

## [1.3.2](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.3.1...v1.3.2) (2022-06-07)


### Bug Fixes

* adding new line before key header ([3495845](https://github.com/James-Frowen/SimpleWebTransport/commit/3495845b8c3fa6838f0838660b652302a20b714a))

## [1.3.1](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.3.0...v1.3.1) (2022-06-07)


### Bug Fixes

* Header lookup needs to be case insensitive ([63aedd8](https://github.com/James-Frowen/SimpleWebTransport/commit/63aedd8086b45b30dbbc6039d47a6c59db7aeded))

# [1.3.0](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.2.7...v1.3.0) (2022-02-12)


### Features

* Allowing max message size to be increase to int32.max ([#2](https://github.com/James-Frowen/SimpleWebTransport/issues/2)) ([4cc60fd](https://github.com/James-Frowen/SimpleWebTransport/commit/4cc60fd67f3c65d90ced0e6f9f97d15d0368076d))

## [1.2.7](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.2.6...v1.2.7) (2022-02-12)


### Bug Fixes

* fixing ObjectDisposedException in toString ([426de52](https://github.com/James-Frowen/SimpleWebTransport/commit/426de52ee4e98ac6212713b2b2272e3affb8fc99))

## [1.2.6](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.2.5...v1.2.6) (2022-02-02)


### Bug Fixes

* fixing Runtime is not defined for unity 2021 ([945b50d](https://github.com/James-Frowen/SimpleWebTransport/commit/945b50dbad5b71c43e2bdaa4033f87d3f62c5572))

## [1.2.5](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.2.4...v1.2.5) (2022-02-02)


### Bug Fixes

* updating Pointer_stringify to UTF8ToString ([2f5a74b](https://github.com/James-Frowen/SimpleWebTransport/commit/2f5a74ba10865e934be8d3b54ebfdeb14ca491f6))

## [1.2.4](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.2.3...v1.2.4) (2021-12-16)


### Bug Fixes

* adding meta file for changelog ([ba5b164](https://github.com/James-Frowen/SimpleWebTransport/commit/ba5b1647aa5cc69ca80f5b52c542a9b5ee749c7f))

## [1.2.3](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.2.2...v1.2.3) (2021-12-16)


### Bug Fixes

* fixing compile error in assemblyInfo ([7ee8380](https://github.com/James-Frowen/SimpleWebTransport/commit/7ee8380b4daf34d4e12017de55d8be481690046f))

## [1.2.2](https://github.com/James-Frowen/SimpleWebTransport/compare/v1.2.1...v1.2.2) (2021-12-16)


### Bug Fixes

* fixing release with empty commit ([068af74](https://github.com/James-Frowen/SimpleWebTransport/commit/068af74f7399354081f25181f90fb060b0fa1524))
