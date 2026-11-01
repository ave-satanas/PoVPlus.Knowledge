---
id: CMP-item-data-stack
type: compat
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, INV-feedbarrel-retirement, INV-itemdata-late-ctor, MOD-food-timers, MOD-epicloot]
applies_to:
  - EpicLoot 0.15.4
  - FeedLikeGrandma 1.2.1
  - FineDining 1.1.12
---
# ItemDataStack

Source: [ItemDataStack.cs](https://github.com/ave-satanas/PoVPlus.Compat/blob/eed8d74b3700a044eb9dbce7053fe0ec2018ca69/ItemDataStack.cs).

**Problem and cause:** FLG and FineDining support EpicLoot's replacement of `ItemDrop.AutoStackItems` but refuse any other transpiler there. ItemDataManager, bundled in several mods, adds one. EpicLoot 0.15.4's prefix returns false on every path, so that transpiler is dead code, but the contracts cannot prove it. `[code]`

**Why needed:** without it, the native contract refuses the pack and barrels stop. `[runtime]` M1.

**What it does:** runs every loaded `ItemDataManager.ItemInfo` and `Vapok.Common.Managers.ItemInfo` static constructor, then unpatches only their `HandleAutostackableItems`. Running the constructors first means none can fire late, which was the defect in [[INV-itemdata-late-ctor]].

**Contracts:** exactly one EpicLoot prefix, `EpicLoot.MagicItemData+ItemDrop_AutoStackItems_Patch.Prefix`, whose IL SHA-256 is `A040E65A…71D3`.

**Init and Harmony:** owner `povplusplus.itemdatastackcompat`. Must run after all `Awake` calls and before FLG and FineDining's `Start`.

**Guards and failure:** no EpicLoot prefix means return silently, since the transpiler is then live code. A different prefix logs a warning and changes nothing. FLG and FineDining then stop dispensing by their own contracts, which is the safe outcome.

**Evidence:** M1, M3, M8, M10. Package: M16.

**Limits:** any EpicLoot rebuild that changes the prefix declines. A mod bundling ItemDataManager that loads after this runs is outside the boundary. Which normal action triggers a late constructor was never established. `[unknown]`

**Revalidate when:** EpicLoot, FLG, FineDining or any ItemDataManager-bundling mod updates. Check for the warning and for `Feed Barrel dispensing stopped`.
