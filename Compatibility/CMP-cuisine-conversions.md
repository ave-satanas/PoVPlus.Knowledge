---
id: CMP-cuisine-conversions
type: compat
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, INV-cooking-conversions, MOD-food-timers]
applies_to:
  - ValheimCuisine 2.3.4
  - Monstrum 1.6.1
  - Warfare 1.9.6
  - Wizardry 1.2.3
---
# CuisineConversions

Source: [CuisineConversions.cs](https://github.com/ave-satanas/PoVPlus.Compat/blob/eed8d74b3700a044eb9dbce7053fe0ec2018ca69/CuisineConversions.cs). Added in 1.0.1.

**Problem and cause:** see [[INV-cooking-conversions]].

**What it does:** after each `ZNetScene.Awake`, adds other mods' conversions to Cuisine's private `vanilla*Conversions` snapshots. Cuisine then keeps them in its own rebuild, including `UpdateAllWorldInstances`. It changes no spoilage setting.

**Why this approach:** a postfix restoring entries after the rebuild would miss Cuisine's world-instance update. A `DataForge` override would need every station's full list kept by hand.

**Contracts:** `ValheimCuisine.ValheimCuisinePlugin+ConversionsZNetScene_AwakePost_Patch`, method `ReapplyConversions`, and fields `vanillaCookingStationConversions`, `vanillaCookingStationIronConversions`, `vanillaOvenConversions`, `vanillaFermenterConversions`.

**Init and Harmony:** owner `povplus.compat.cuisineconversions`, deferred stage. The `ZNetScene.Awake` postfix uses `Priority.Last` so it runs after every mod has registered. A `ReapplyConversions` postfix records what Cuisine itself holds, so its config rows are never adopted.

**Guards and failure:** a missing contract logs a warning and changes nothing. Entries already in the snapshot are skipped, so re-added entries on rejoin do not accumulate. Entries with null items are ignored.

**Evidence:** M31, M32.

**Limits:** an entry removed from Monstrum, Warfare or Wizardry stays in Cuisine's snapshot until the process restarts. Stations other than these four are unaffected by Cuisine's clear, or were not checked. The module has no isolated unit test; acceptance is the runtime before and after in M30 and M31.

**Revalidate when:** ValheimCuisine, Monstrum, Warfare or Wizardry updates. Compare `fdcounts` with 12/23/32/46.
