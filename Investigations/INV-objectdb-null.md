---
id: INV-objectdb-null
type: investigation
status: bounded
created: 2026-11-01
updated: 2026-11-01
related: [DIAG-warning-ledger, MOD-path-of-valheiman]
applies_to:
  - Path of Valheiman 5.3.6
  - Jotunn 2.30.2
---
# ObjectDB null item

**Trigger:** the **second** return to the main menu, not a plain rejoin. An earlier ledger said "rejoin only", which was imprecise. `[runtime]` M28

**Identity:** `PT_BlueRunePebble`, index 3599 of 3785. Jotunn warns and PoV removes the entry. It is back in ObjectDB, the item list and the scene on the next join, total 3,785. `[runtime]` M28, three joins, twice.

**Unknown:** what destroys the prefab. Its `recipes=0` reading was not compared with a first join.
