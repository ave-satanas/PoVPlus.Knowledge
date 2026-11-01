---
id: INV-harmonyx-attribution
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
related: [CMP-quiet-lookups, CMP-epic-map-chips, DIAG-warning-ledger]
---
# HarmonyX caller attribution

`tools\CallerTrace` recorded each failed lookup's calling method. `[runtime]` M14. It is test-only and removed.

| Lookup | Caller | Target | Verdict |
|---|---|---|---|
| `ItemDrawers.*`, `kg_ItemDrawers.*` | PoV | Absent | Optional |
| `QuickStackStore.*`, `IsSlotFavorited`, `InternalIsEquipOrQuickSlot` | PoV | Absent | Optional |
| `BestAutoSort.*`, `ExtraSlots.Slots`, `BetterArchery`, `RunicBuildCamera.*` | PoV | Absent | Optional |
| `BetterRiding.*`, `Balrond.*` | OdinHorse | Absent | Optional |
| `ConfigurationManager.ConfigSettingEntry` | PoV | Present, loads later | ConfigManager fallback bound. Lock checked in M16 |
| `MagicTooltipPatches`, `GetSocketOpenHint` | BowsBeforeHoes | API changed | Socket tooltip works; open prompt text absent under SeneaL, cosmetic |
| `PassiveTree.Equipment.*`, `Module.Neutralized`, `Restore` | SeneaL UI | API changed | Stale fallbacks; PoV module active, 59 cells |
| `ZInput.GetJoyRightStickY(bool)` | SeneaL UI | API changed | Version check |
| `MinimapController._adventure*` | SeneaL UI | API changed | Genuine gap, fixed by [[CMP-epic-map-chips]], then quieted |

Runtime functional checks for these are in M16. Absent optional mods were not installed to test their paths. Global HarmonyX logging is unchanged.
