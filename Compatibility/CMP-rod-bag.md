---
id: CMP-rod-bag
type: compat
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, CMP-tankards, MOD-epicloot, MOD-seneal-ui]
applies_to:
  - TrollingFishing 1.1.5
  - EpicLoot 0.15.4
  - SeneaL API 1.2.0
---
# RodBag

Source: [RodBag.cs](https://github.com/ave-satanas/PoVPlus.Compat/blob/eed8d74b3700a044eb9dbce7053fe0ec2018ca69/RodBag.cs).

**Problem and cause:** TrollingFishing binds `EpicLoot.ShardStones.SocketsUI`, which EpicLoot 0.15 removed. The failed bind leaves `EpicLootBagCompatibility.ShouldYieldUse` true, so Use over a rod does nothing. `[code]` confirmed in F3 `[runtime]`.

**What it does:** docks the bag through SeneaL's `ContainerDocks`, registers rods with EpicLoot as containers, and postfixes the yield gate. `E` is the bag. `Alt+E` is a socketed rod's shard slots. Docking is required, because an undocked bag shifts the grid and the next press hits another item.

**Contracts:** `TrollingFishing.EpicLootBagCompatibility` (`_unavailable`, `ShouldYieldUse`), `TrollingFishing.FishingOverrideSystem` (`IsFishingRod`, `IsFishingRodBagContainer`), `EpicLoot.ShardStones.ShardUI.IsOpen`, EpicLoot `API.RegisterItemContainer`, `API.IsShardSlotPress`.

**Init and Harmony:** owner `povplusplus.rodbaginputbridge`, deferred stage. Compile-time reference to EpicLoot and SeneaL-API.

**Guards and failure:** the postfix acts only while `_unavailable` is true, so it stands down if upstream binds again. A missing contract logs a warning.

**Evidence:** F3, M6, M16.

**Limits:** gamepad gesture not tested. TrollingFishing's own warning stays, by design.

**Revalidate when:** TrollingFishing, EpicLoot or SeneaL API updates.
