---
id: CMP-tankards
type: compat
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, CMP-rod-bag, MOD-epicloot, MOD-seneal-ui]
applies_to:
  - UsefulTankards 1.0.9
  - EpicLoot 0.15.4
  - SeneaL API 1.2.0
---
# Tankards

Source: [Tankards.cs](../../patches/PoVPlus.Compat/Tankards.cs). Replaces the SeneaLUI UsefulTankards Bridge. An earlier version lives in `S:\Valheim-Modding\UsefulTankards-Bridge` (machine-local).

**Problem and cause:** UsefulTankards 1.0.9 binds `EpicLoot.ShardStones.SocketsUI`, gone since EpicLoot 0.15, then yields every Use press. `[code]`, F3 `[runtime]`.

**What it does:** docks tankard storage under SeneaL's inventory, registers tankards with EpicLoot, and postfixes the gate. `E` is storage. `Alt+E` is shard slots.

**Correction kept:** an earlier bridge used SeneaL's hover override, which went stale after a toggle and let a third press close the inventory. Vanilla's `InventoryGrid.GetItem` stayed right, so this uses that lookup. `[runtime]`

**Contracts:** `UsefulTankards.TankardStorageSystem:IsTankardStorageContainer`, `TankardTweaks:TryGetProfile`, `TankardStorageSlots`, `EpicLootStorageCompatibility` (`_unavailable`, `ShouldYieldUse`).

**Init and Harmony:** owner `avesatanas.senealui.usefultankards.bridge`, deferred stage. The EpicLoot part sits in a nested class with `NoInlining`, so the dock works without EpicLoot.

**Guards and failure:** a missing core contract logs an **error** and disables this module. A missing gate logs a warning; the dock stays.

**Evidence:** F3, M6, M16.

**Limits:** gamepad gesture not tested.

**Revalidate when:** UsefulTankards, EpicLoot or SeneaL API updates.
