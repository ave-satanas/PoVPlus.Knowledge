---
id: INV-feedbarrel-retirement
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
related: [MOD-food-timers, CMP-item-data-stack, CMP-retired]
applies_to:
  - FeedLikeGrandma 1.2.1
  - FineDining 1.1.12
---
# FeedBarrelStackCompat retirement

**Original problem:** EpicLoot's prefix always skips `AutoStackItems`, and FLG refused any prefix, so barrels stopped. Dispensing also needs a player-placed barrel (`s_creator`). `[runtime]` F1. Vanilla merges drops only above 200 loaded.

**Outcome:** FLG 1.2.1 and FineDining 1.1.12 integrate natively with EpicLoot's replacement, so FeedBarrelStackCompat was removed from client, server and `required`. Native integration still needed one patch: [[CMP-item-data-stack]]. `[runtime]` M1, M2.

**Note:** 1.0.0 of FeedBarrelStackCompat had a regression fixed in 1.1.0 during harness Round 2. F1 describes the retired patch; M records describe current behaviour.
