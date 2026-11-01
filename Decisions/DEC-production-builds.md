---
id: DEC-production-builds
type: decision
status: active
created: 2026-11-01
updated: 2026-11-01
related: [OPS-build-release, OPS-test-tools]
---
# Production builds

**Decision:** `[decision]` Probes, tracers, fixture spawning, test commands, simulated-player controls and measurement counters must not be compiled into PoVPlus.Compat. Keep genuine failure diagnostics. Do not add another standalone compatibility plugin or a framework.

**Current status:** verified for 1.0.1, `8B766887…`. `[checked]` M32: compiler inputs are the ten source files plus generated attributes, a clean rebuild reproduces the hash, and a metadata scan of 190 strings found no such terms. The same check passed for 1.0.0 in M23.
