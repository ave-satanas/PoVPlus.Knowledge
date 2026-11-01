---
id: CMP-sleep-hud
type: compat
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, INV-sleep-hud-rpc, INV-sleep-protection, MOD-bettersleepbruh, MOD-seneal-ui, DEC-test-constraints]
applies_to:
  - BetterSleepBruh 2.1.2
  - SeneaL UI 1.2.8
---
# SleepHud

Source: [SleepHud.cs](../../patches/PoVPlus.Compat/SleepHud.cs).

**Problem and cause:** BetterSleepBruh 2.1.2 finds the minimap at a fixed path that SeneaL UI moves. Its client RPCs live on the HUD view, so with no view a client on a dedicated server never learns the boost, and timer and boat protections stay off. `[runtime]` M11.

**What it does:**
- Builds the view from `Minimap.m_smallRoot`, only after BetterSleepBruh's own build failed.
- Places the strip beside the map, outside SeneaL's mask, clear of its info card, and flips right when the left side does not fit.
- Clears only RPC entries whose delegate targets a `SleepHudView`.
- `Tick` runs at 1 Hz on a live view only, re-homing it and rebinding lost handlers.
- `ViewGuard` removes a destroyed view's callbacks.
- `OneWaveStepPerFrame` limits `WaterVolume.StaticUpdate` during a boost. See [[INV-sleep-protection#Waves]].

**Contracts:** `BetterSleepBruh.BetterSleepBruh` (`BuildSleepGui`, `Update`, `_sleepHud`), `Components.SleepHudView` (`RegisterRpcs`, `TryCreate`, `_rpcsRegistered`), `Patches.EnvManPatches:IsInSleepWindow`, `ZRoutedRpc.m_functions`. Wave part: `SleepTracker.CurrentExtraRate`, `ConfigRegistry.DecoupleWaterWaveSpeed`.

**Init and Harmony:** owner `povplusplus.bettersleephudbridge`, deferred stage. Ticked from `Plugin.Update`.

**Guards and failure:** a missing contract logs a warning. A foreign RPC owner is left alone, with one warning. It gives up after 100 missing-minimap builds, with one warning per session. Reset happens when ZNet stops.

**Evidence:** M11, M12, M13, M16, M17.

**Limits:** see [[INV-sleep-protection#Limits]]. Its Info lines do not appear in `LogOutput.log`, because of [[DIAG-logging]].

**Revalidate when:** BetterSleepBruh or SeneaL UI updates, or Valheim changes `WaterVolume`.
