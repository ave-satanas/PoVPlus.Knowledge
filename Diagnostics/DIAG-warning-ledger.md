---
id: DIAG-warning-ledger
type: diagnostic
status: active
created: 2026-11-01
updated: 2026-11-01
related: [DIAG-logging, INV-portal-race, INV-objectdb-null, INV-location-data, INV-seekerbrood, INV-missing-prefabs, INV-harmonyx-attribution]
---
# Warning ledger

From the 1.0.1 clean run, 2026-11-01: client 42, server 523 lines. 0 exceptions, 0 HarmonyX. `[runtime]` M32, with causes from M28. Full per-cause fields: `REPORT.md`, "Warning ledger". An absent line is not a fixed line.

| Class | Messages | Significance |
|---|---|---|
| **Fixed** | `toxicDeath`, `BeeQueen`, `DungeonDB` NRE, `Gave up building sleep HUD`, `Feed Barrel dispensing stopped`, HarmonyX lookups, `PT_PackedShipTable` | 0 in both runs |
| **Consequence fixed, line remains** | `Seeker_Brood` | [[INV-seekerbrood]] |
| **Expected** | EpicLoot compatibility unavailable ×3; `Shared cache is busy`; Crossbows category; `Unshamed` cheat bypass with devcommands | Handled or diagnostic |
| **Headless, server only** | Audio mixer, HDR, video shaders, `AsyncResourceUpload`, `[Память]`, `[Редкость]`, `Lux Lit Particles`, `Failed to load any prefab data` | No renderer or audio |
| **Bounded** | `Ignored portal catalog data`, [[INV-portal-race]]; `Found null item` and `убраны пустые записи`, [[INV-objectdb-null]]; `Location client data`, [[INV-location-data]] | No loss observed, within stated limits |
| **Unresolved, untraced** | `/api/link/inbox: Thread was being aborted`, cause `[hypothesis]` a long poll cut off at quit; `The LiberationSans SDF Font Asset was not found`, for labels `Distance` and `Age`, intermittent, also present before 1.0.1; `Only custom filters`; `Missing audio clip in music respawn`; `AssetBundle valheimcuisine already loaded`; `Releasing render texture`; `Food` category; `MagicPlugin cosmetic probe` for `IceScepterNovaRing`, whose patch stays off for that item | None seen, not traced to callers |
| **Merely absent** | `final character save`, `TrinketEffects`, FSB | Did not occur. May return on a window-close exit |

**Consequential today:** none is known to lose data or block play. A clean log did not reveal [[INV-cooking-conversions]]: no warning fired for it, so a quiet log is not proof that content works. Watch the `Seeker_Brood` line, should Magic Supremacy fix its typo.

Shutdown notices: close the window and the `final character save` warning can appear. Use Log Out. See [[OPS-saves-shutdown]].
