---
id: INV-seekerbrood
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [DIAG-warning-ledger]
applies_to:
  - Magic Supremacy 3.1.6
  - DropNSpawn 1.4.9
---
# SeekerBrood page drop

**Cause:** `Magic_SupremacyPlugin.cs:615`, `item22.DropsFrom.Add("Seeker_Brood", 0.005f, 1, 2)`. A typo; the prefab is `SeekerBrood`. `[checked]`

**Fix:** configuration only. `DNS_character.yml` has one `SeekerBrood` entry holding `RoyalJelly` (chance 0.5, no level multiplier) and `PageArcane_DO` (1~2, 0.005). DropNSpawn lists replace a creature's drops, so `RoyalJelly` had to be kept. Backup: `backups\dropnspawn-pre-seekerbrood\`.

**Measured:** 20,000 rolls through the game's drop path on a live creature: page 101 (0.505%), jelly 9,980 (49.9%). `[runtime]` M28

**Limits:** rolls, not real kills. Starred scaling not measured. The warning stays, because the typo is compiled into the mod. Fixing it upstream means removing the entry.
