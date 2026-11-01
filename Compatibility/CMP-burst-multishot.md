---
id: CMP-burst-multishot
type: compat
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, MOD-epicloot]
applies_to:
  - EpicLoot 0.15.4
  - SecondaryAttacks 1.2.21
---
# BurstMultiShot

Source: [BurstMultiShot.cs](../../patches/PoVPlus.Compat/BurstMultiShot.cs).

**Problem and cause:** EpicLoot's multishot multiplies `Attack.m_projectiles` for one shot and clears only the damage scale. SecondaryAttacks' burst fires later shots without returning through `FireProjectileBurst`, so they inherit the count at full damage. SecondaryAttacks' own compat no longer binds to EpicLoot 0.15. `[code]`

**Why needed:** `[runtime]` H1: nine projectiles from three bolts on a proc, with later shots at full damage.

**What it does:** a finalizer on `Attack.FireProjectileBurst` divides the count back when EpicLoot's scale ends.

**Contracts:** `EpicLoot.MagicItemEffects.MultiShot` fields `_scaledBurstAttack`, `ShotProjectiles`; `SecondaryAttacks.EpicLootCompat._api`.

**Init and Harmony:** owner `povplusplus.burstmultishotcompat`. `Priority.First` is required, because it must run before EpicLoot's finalizer clears `_scaledBurstAttack`.

**Guards and failure:** a missing contract logs a warning and changes nothing. It stands down whenever SecondaryAttacks' `_api` binds again.

**Evidence:** H1, M6. Local world only, since boss keys were needed.

**Limits:** other EpicLoot projectile effects in bursts were not tested. Shots after the first do not re-proc. This is a judgment about intended behaviour, recorded in H1.

**Revalidate when:** EpicLoot or SecondaryAttacks updates.
