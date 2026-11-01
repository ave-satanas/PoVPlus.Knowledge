---
id: INV-location-data
type: investigation
status: bounded
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [DIAG-warning-ledger, OPS-remaining-work]
applies_to:
  - Expand World Data 1.79.0
---
# Expand World Data location files

**Setting:** `Location data = false`, in all 11 backups from 2026-10-18 to 10-31. `[checked]`

**Effect:** all five `expand_locations*.yaml` files are inactive. They share one timestamp, 2026-10-18 19:07, so they look generated once. `[checked]` The reader exits, `UpdateData` never runs, the synced value stays null, and clients log `Location client data: Value cannot be null` each join. `[code]` plus `[runtime]` M28.

**Census on a copy of Wraeclast, 2026-11-01:** 183 location types, 12,413 instances. `evidence\ops\census-locations.txt`. `[runtime]`

| File | Rows | In world | Equal to YAML quantity | Above it | Disabled |
|---|---|---|---|---|---|
| monstrum | 6 | 6 | 6 | 0 | 0 |
| rtdlegends | 26 | 0 | 0 | 0 | 0 |
| rtdmythics | 24 | 0 | 0 | 0 | 0 |
| valheimcuisine | 2 | 2 | 0 | 0 | 0 |
| expand_locations | 213 | 175 | 142 | 16 | 29 |

`InfestedTree01` ×700 and `CharredStone_Spawner` ×300 are disabled in YAML but in the world. `Crypt2` is 200 against 140. So `Wraeclast` was not generated from these files. This supports "inactive at generation".

**Not established:** whether that is the pack's *intended* world generation. All 50 RtD Legends and Mythics rows are absent from the world, and no RtD Legends or Mythics mod is installed. Only the user can confirm intent. `[unknown]`

**Decision:** `[decision]` keep it disabled while the intended generation behaviour is established. This is a design question, not a defect.
