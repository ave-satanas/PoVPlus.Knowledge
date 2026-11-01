---
id: INV-toxic-aura
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [DEC-toxic-aura]
applies_to:
  - CreatureManager 1.3.10
---
# Toxic Aura migration

CreatureManager replaced Toxic Death with a continuous aura. `toxicDeath` stayed an alias, preserving power 0.3. `[upstream]`

**Change:** `toxicDeath: n, 0.3, 4, blob_aoe` → `toxicAura: n, 0.1` in `levels.yml` Global and Boss, 10% each; `karma.yml` line 49, 10%; and the BlackForest override, 30%. Chances were kept. `[checked]` No `toxicDeath` keys remain. Backup: `backups\creaturemanager-pre-toxicaura\`.

**New creature:** `cm:spawn Greyling toxicAura 2` gave mask 512, power 0.1, Poison on the player in range, no errors. `[runtime]` M28

**Limits:** damage was not isolated, as melee also hit and the 25 HP character died. Saved creatures falling back to 0.1 is an upstream claim. `[upstream]`
