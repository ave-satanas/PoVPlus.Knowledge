---
id: CMP-health-regen
type: compat
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, INV-regen-evidence, MOD-food-timers, MOD-epicloot]
applies_to:
  - EpicLoot 0.15.4
  - FineDining 1.1.12
---
# HealthRegen

Source: [HealthRegen.cs](https://github.com/ave-satanas/PoVPlus.Compat/blob/eed8d74b3700a044eb9dbce7053fe0ec2018ca69/HealthRegen.cs).

**Problem and cause:** FineDining replaces `Player.UpdateFood`, so EpicLoot's transpiler adding `AddHealthRegen` sits in a body that never runs. `[code]`

**Why needed:** real helmet evidence shows the bonus is lost without it. See [[INV-regen-evidence]].

**What it does:** transpiles `FineDining.PlayerFoodLogic:UpdateFood`, inserting EpicLoot's `AddHealthTicks` before the multiplier, keeping `(regen + bonus) * multiplier`.

**Contracts:** `FineDining.PlayerFoodLogic:UpdateFood`, `FineDining.PathOfValheimanCompatibility:HealFood`, `EpicLoot.MagicItemEffects.ModifyPlayerRegen+SEMan_ModifyRegen_Patches:AddHealthTicks`. Expected IL shape: regen, multiplier, `mul`, one `HealFood` call.

**Init and Harmony:** owner `povplusplus.healthregencompat`, deferred stage.

**Guards and failure:** a missing member or changed shape logs a warning and leaves the IL unchanged. It also refuses if `AddHealthTicks` is already present, so an upstream fix cannot apply it twice.

**Evidence:** M5, F2, and `evidence\compat\` regen records from M16.

**Limits:** gear with Cold was not measured with the compat. Rolled legendaries and shard stones were not observed. `[unknown]`

**Revalidate when:** FineDining or EpicLoot updates. Warning text: `FineDining's food heal has changed shape`.
