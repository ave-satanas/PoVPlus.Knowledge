---
id: INV-corrections
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
related: [INV-config-reconciliation, INV-objectdb-null, INV-portal-race, OPS-saves-shutdown]
---
# Corrections

Kept so wrong claims are not rediscovered. Older ones: `REPORT.md`, "Corrections to previous conclusions".

| Earlier claim | Current | Record |
|---|---|---|
| "No meaningful warning remains unresolved" | Several are bounded and documented, not fixed | [[INV-config-reconciliation]] |
| 597 MB free came "from other load on the machine" | Game and server working sets were 17.6 GB; the 13.7 GB remainder is accounting, not process memory; 597 MB is the last sample, not the low | [[OPS-memory]] |
| Start combined runs with 19 GB free | 19.4 GB left little margin; about 21 GB is provisional, not a requirement | [[OPS-memory]] |
| Eight config files differ | 23 | [[INV-config-reconciliation]] |
| All 128 older-vault records differ | 117 of 125 common records match main and Claude after line endings | [[HIST-index]] |
| FineDining mismatch was probably a stale client report | The stale report was correct; live conversions were being lost | [[INV-cooking-conversions]] |
| Server had "no conversion" as a server-side gap | Both sides lost them equally | [[INV-cooking-conversions]] |
| Production session was prohibited | That was one task's boundary | [[DEC-production-session-hold]] |
| `-saveinterval 900` directly verified | Source log not recovered | [[OPS-saves-shutdown]] |
| "0 PlanBuild objects" in Wraeclast | One `PlanHammer`; a filter miss | [[INV-missing-prefabs]] |
| Prefab `BeeQueen` "exists nowhere" | It is in OdinPlus-BeeQueen | [[INV-missing-prefabs]] |
| Saves every 30 minutes | 15 minutes, from store snapshots. The 900 s launch argument is unverified | [[OPS-saves-shutdown]] |
| EpicLoot filters reachable; then chips do nothing | Neither | [[CMP-epic-map-chips]] |
| ObjectDB null on rejoin | Second return to menu | [[INV-objectdb-null]] |
| Portal race on every join | Frequent, timing-dependent | [[INV-portal-race]] |
| "Location client data" recovers when values arrive | Value stays null; no data exists to recover | [[INV-location-data]] |
| `DungeonDB` NRE was harmless, then also skipped EWD | Skipped DataForge only | [[INV-planbuild]] |
| Comment said transpiler empties `m_customData` | Unsupported | [[INV-itemdata-late-ctor]] |
| "11 mods should be refused" by ServerSync | First gate is ServerManager's SHA-256 list | [[OPS-deployment-sync]] |
| Keep SeneaL hover override | Stale; removed | [[CMP-tankards]] |
| PoVPlus.Compat Info lines missing, cause unknown | LoadTimeProfiler filters them | [[DIAG-logging]] |
