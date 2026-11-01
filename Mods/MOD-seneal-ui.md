---
id: MOD-seneal-ui
type: mod
status: active
created: 2026-11-01
updated: 2026-11-01
related: [CMP-sleep-hud, CMP-epic-map-chips, CMP-rod-bag, CMP-tankards]
applies_to:
  - SeneaL UI 1.2.8
  - SeneaL API 1.2.0
---
# SeneaL UI

SeneaL UI 1.2.8 with SeneaL API 1.2.0. It moves the minimap under `SeneaLUI_Hud`, which breaks fixed-path consumers, and it provides `ContainerDocks`. Client-only layout entries in `seneaL.valheim.ui.cfg` appear after the UI runs, so that file differs from the server's in comments only. `[checked]`

Stale probes for PoV and EpicLoot are listed in [[INV-harmonyx-attribution]]. `Dual=False` is a stale probe result; belt and Wishbone both equip. `[runtime]` M16.
