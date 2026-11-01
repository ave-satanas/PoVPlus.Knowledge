---
id: CMP-coverage
type: inventory
status: active
created: 2026-11-01
updated: 2026-11-01
related: [MOD-inventory, CMP-index, OPS-evidence-map]
---
# Coverage

**This does not establish whole-pack compatibility.** Canonical detail: [COVERAGE.md](../../COVERAGE.md).

**Patch map:** 8,114 patches on 1,921 methods, 598 shared by two or more owners. It is the server's map from `evidence\runtime\B-patch\harmony-dump.tsv`, predating the migration, and omits client-only runtime patching. `[runtime]`

**Examined deeply:** EpicLoot, FLG, FineDining and ItemDataManager on `AutoStackItems`; FineDining, EpicLoot and PoV on `UpdateFood`; EpicLoot input with UsefulTankards and TrollingFishing; client and server deployment; Southsil recipe identifier; BetterSleepBruh under SeneaL UI.

**Unexamined:**
- 254 `can-skip` prefixes. Only 7 were read, and the regex classifier was wrong at least once, so treat the file as leads.
- Transpiler ordering on other shared methods.
- Prefab and recipe identity across content mods.
- Real-world save migration.
- FineDining's winter pause, since no seasonal mod is installed.
- Anything beyond one client.
