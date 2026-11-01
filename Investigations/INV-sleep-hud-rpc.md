---
id: INV-sleep-hud-rpc
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
related: [CMP-sleep-hud, MOD-bettersleepbruh, INV-sleep-protection]
applies_to:
  - BetterSleepBruh 2.1.2
  - SeneaL UI 1.2.8
---
# BetterSleepBruh HUD and RPC failure

**Symptom:** repeated `Gave up building sleep HUD` and no client protections.

**Cause:** the real path is `…/hudroot/SeneaLUI_Hud/mapFade/Minimap/minimap/map/small`. `RPC_SleepingPlayerInfo` and `BSB_SyncSleepBoostTotal` were unregistered, so `extra=0`, `total=0` during a boost. `Update` re-arms the retry, giving 21 give-ups in 14 minutes. `[runtime]` M11

**Cross-check:** turning SeneaL's minimap off restored the path, and BetterSleepBruh built its own HUD and protection came on. `[runtime]` M13.

**Lifecycle:** one view and four handlers through element off and on, view destruction, dropped RPC and rejoin. A foreign handler was kept with one warning and no throw. `[runtime]` M12
