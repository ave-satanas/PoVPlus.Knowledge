---
id: MOD-path-of-valheiman
type: mod
status: active
created: 2026-11-01
updated: 2026-11-01
related: [INV-objectdb-null, INV-config-reconciliation, INV-harmonyx-attribution]
applies_to:
  - Path of Valheiman 5.3.6
---
# Path of Valheiman

Version 5.3.6 (`treextr.pathofvalheiman`), the pack's core overhaul.

- `Creature density = 1.2` in its config scales spawn rows when a `SpawnSystem` wakes. `[checked]` See [[INV-config-reconciliation#Spawn reference]].
- It skips objects with missing prefabs, logging `Network object skipped`, and removes empty ObjectDB entries.
- World-settings lock: its load order leaves `EntryType` null, and ConfigManager's fallback binds. Server entries refused edits, even for an admin. `[runtime]` M16. The server-side difficulty RPC path is `[code]` only.
