---
id: INV-cooking-conversions
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
related: [CMP-cuisine-conversions, INV-config-reconciliation, MOD-food-timers, OPS-evidence-map]
applies_to:
  - ValheimCuisine 2.3.4
  - Monstrum 1.6.1
  - Warfare 1.9.6
  - Wizardry 1.2.3
  - FineDining 1.1.12
---
# Lost cooking conversions

**Symptom:** the FineDining reference mismatch in [[INV-config-reconciliation]]. The live check showed something larger: the conversions themselves were missing.

**Root cause:** `[runtime]` M30, identical on client and server. ValheimCuisine's `ZNetScene.Awake` postfix runs at index 38 and snapshots the stations as "vanilla". Warfare, Monstrum and Wizardry postfixes follow at indexes 91 to 119. On each world load, `ValheimCuisinePlugin.Update` calls `ReapplyConversions`, which clears the stations and rebuilds them from that snapshot plus Cuisine's config.

| Point | Cooking | Iron | Oven | Fermenter |
|---|---|---|---|---|
| End of `Awake`, all mods applied | 12 | 23 | 32 | 46 |
| After the rebuild, before the fix | 10 | 20 | 31 | 41 |
| After the rebuild, with 1.0.1 | 12 | 23 | 32 | 46 |

Eleven `_TW` conversions were lost. The same rebuild ran 30 to 40 s after load, and again on rejoin.

**Consequences before the fix:** `[runtime]`
- `BearSteak_TW`, `FoxMeat_TW`, `SharkMeat_TW` and `UncookedBerryCakePlains_TW` resolved to `NotTracked`, held no timer and never spoiled. They were unchanged after a 75 h skip. This was failed classification, not intentional non-spoilage.
- `CookedSharkMeat_TW` and `BerryCakePlains_TW` fell to `OtherEdible`, 48 h, then `FineDining_RottenFood`.
- Rule readings showed the same for the other cooked foods. `[runtime]`
- Those foods could not be cooked at these stations, since the prefab lists lacked them. `[code]`

## Food-by-food results

Each row: rule on 1.0.0 → rule on 1.0.1. `[runtime]` M30, M31. Client and server rules matched in both runs.

| Item | 1.0.0 | 1.0.1 | Timer on 1.0.1 | After 75 h |
|---|---|---|---|---|
| `CookedBearSteak_TW` | 48 h `OtherEdible` | 72 h `CookingStationOutput` | 71.99 h | `RottenMeat` |
| `CookedFoxMeat_TW` | 48 h `OtherEdible` | 72 h `CookingStationOutput` | 71.99 h | `RottenMeat` |
| `CookedSharkMeat_TW` | 48 h `OtherEdible` | 72 h `CookingStationOutput` | 71.99 h | `RottenMeat` |
| `BerryCakePlains_TW` | 48 h `OtherEdible` | 72 h `CookingStationOutput` | 71.96 h | `RottenMeat` |
| `BearSteak_TW` | `NotTracked`, no timer | 48 h `CookingStationInput` | 47.97 h | `RottenMeat` |
| `FoxMeat_TW` | `NotTracked`, no timer | 48 h `CookingStationInput` | 47.96 h | `RottenMeat` |
| `SharkMeat_TW` | `NotTracked`, no timer | 48 h `CookingStationInput` | 47.96 h | `RottenMeat` |
| `UncookedBerryCakePlains_TW` | `NotTracked`, no timer | 48 h `CookingStationInput` | 47.97 h | `RottenMeat` |
| Control `CookedMeat` | 72 h | 72 h | 71.98 h | `RottenMeat` |
| Control `NeckTailGrilled` | 72 h | 72 h | 71.99 h | `RottenMeat` |
| Control `RawMeat` | 48 h | 48 h | 47.98 h | `RottenMeat` |
| Control `Raspberry` | 96 h | 96 h | 95.99 h | 20.85 h left |

Timers follow the world clock, not real time. In run 2, 66 real seconds moved the world clock 64 s and timers fell 0.02 h. In the 1.0.1 run, 66 real seconds moved it 256 s, because BetterSleepBruh test mode was boosting that night, and timers fell 0.07 to 0.08 h. So food ages faster in world time during a sleep boost; whether that is intended was not examined. `[unknown]` After a confirmed save, Log Out and rejoin, timers continued with no reset. `server.saved` rose 51→52. Stacks exceeded 2 where drops left by earlier runs were also picked up.

A placed iron station accepted `BearSteak_TW`, `FoxMeat_TW`, `SharkMeat_TW` and `RawMeat` through `IsItemAllowed`. It held 24 conversions against the prefab's 23; the extra one was not traced.

## Method limits

- Fixtures: console `spawn`, then `Player.Pickup` through the probe command `pickall`. The first two runs relied on auto-pickup and got only 5 of 12 foods. Run 1 and run 2 are `evidence\fdcheck\run1`, `run2-baseline`.
- Pre-fix timers and expiry for `CookedBearSteak_TW`, `CookedFoxMeat_TW` and the controls were not all captured; their pre-fix rows come from `fdrule`.
- `skiptime` is a console fixture for expiry, not normal play.
- **A full cook was not run.** Acceptance by the station is not a finished item.
- Between `Awake` and the rebuild on rejoin, counts briefly show duplicates, 14/26/33/51, for about 30 s. Effect on VNEI or cooking in that window was not measured.
