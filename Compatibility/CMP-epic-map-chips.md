---
id: CMP-epic-map-chips
type: compat
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, MOD-epicloot, MOD-seneal-ui, INV-harmonyx-attribution]
applies_to:
  - EpicLoot 0.15.4
  - SeneaL UI 1.2.8
---
# EpicMapChips

Source: [EpicMapChips.cs](https://github.com/ave-satanas/PoVPlus.Compat/blob/eed8d74b3700a044eb9dbce7053fe0ec2018ca69/EpicMapChips.cs).

**Problem and cause:** EpicLoot replaced its bounty and treasure toggles with pin filters in the vanilla panel, which SeneaL's map covers. SeneaL UI still looks for the old toggles, so its chips hide and the filters are unreachable. `[runtime]`

**What it does:** postfixes `SeneaLUI.Mods.EpicMap.Toggle` and `Enabled`, giving SeneaL's existing chips a proxy `Toggle` wired to `Minimap.ToggleIconFilter`. It is not a new map system.

**Contracts:** `SeneaLUI.Mods.EpicMap`, `EpicLoot.EpicLoot.BountyPinType`, `TreasureMapPinType`, private `Minimap.m_visibleIconTypes`.

**Init and Harmony:** owner `povplus.compat.epicmapchips`, deferred stage. Pin types are read when SeneaL asks, because EpicLoot assigns them during game setup.

**Guards and failure:** a missing contract logs a warning and the chips stay hidden. `Ready` checks the minimap and pin bounds.

**Evidence:** `evidence\compat\chips-toggle.txt`, M16. Real clicks toggle both filters before and after rejoin.

**Correction kept:** earlier claims that the filters were reachable, and later that the chips did nothing, were both wrong. See [[INV-corrections]].

**Revalidate when:** EpicLoot or SeneaL UI updates. If SeneaL gains native support, retire this module.
