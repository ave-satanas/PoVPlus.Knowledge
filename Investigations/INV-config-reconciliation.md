---
id: INV-config-reconciliation
type: investigation
status: bounded
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CURRENT-STATE, INV-cooking-conversions, OPS-deployment-sync, MOD-food-timers, MOD-path-of-valheiman, CMP-retired, OPS-remaining-work]
applies_to:
  - FineDining 1.1.12
  - EpicLoot 0.15.4
  - Path of Valheiman 5.3.6
---
# Client and server reconciliation

Performed 2026-10-31 to 11-01, read-only.

**Correction:** the handoff before this said eight configs differed and "no meaningful warning remains unresolved". Both were wrong. A complete comparison found **23** differing files, and several causes are bounded, not resolved. The earlier filter skipped `.txt` and reference files.

## DLL count, 96 → 88

96 − 9 + 1 = 88. `[checked]` against `evidence\migration\parity-final.tsv` (93 names on 10-30) and the later additions. The 96-file list itself was recorded only as a count, so this is a reconstruction.

Removed: `BetterSleepHudBridge`, `BurstMultiShotCompat`, `HealthRegenCompat`, `ItemDataStackCompat`, `PlanBuildRejoinCompat`, `QuietLookups`, `RodBagInputBridge`, `SenealUI.UsefulTankards.Bridge`, and `PlanBuild`. Seven merged into PoVPlus.Compat. `PlanBuildRejoinCompat` retired with PlanBuild. See [[CMP-retired]]. Added: `PoVPlus.Compat`.

## All 23 differences

| File | Class | Finding |
|---|---|---|
| `Therzie.Warfare.cfg`, `seneaL.valheim.ui.cfg`, `sighsorry.ConfigManager.cfg` | Comments only | 0 non-comment differences `[checked]` |
| `EpicLoot\baseconfig\iteminfo.json` | Order only | Equal after normalizing array order `[checked]` |
| `EpicLoot\configstate.json` | Derived | Only `WrittenHash` of iteminfo differs `[checked]` |
| `EpicLoot\creaturesorter-locations.json`, `creaturesorter-report.txt` | Cache and report | Rebuilt per machine `[checked]`. Keyed entries differ; the file is a cache |
| `permissions.yaml` | Generated | A joined-character entry, written at join on the server. Holds a Steam ID, so it stays out of this vault |
| `PortalRules\invite-travel-cooldowns.yml`, `.bak` | Runtime state | Keyed by world `[checked]` |
| `DropNSpawn\cache\*`, `BossRules\cache\*`, `CreatureManager\cache\*` (5 files) | Cache | Hashes and provenance `[checked]` |
| `DropNSpawn\DNS_spawnsystem.reference.yml` | Report | See below |
| `CreatureManager\textures.reference.txt`, `projectile.reference.yml`, `DataForge\z_materials.reference.txt`, `pieces.reference.yml`, `YouAreNotWorthy\keys.reference.yml` | Report | Generated per machine; not read as config. **Individual causes not traced.** `projectile`: `spawnOnHit` `EL_ConjuredArrow` on client, `ArrowFrost` on server. `pieces`: one piece's resources appear only on the client |
| `FineDining\Spoilage.reference.yml`, `FoodTier.reference.yml` | Exposed a real defect | See below |

Also client-only or server-only: profiler logs, BakaLoader configs, the disposable bounty ledger and BetterSleepBruh data. `BountySaves` and `BetterSleepBruh_Data_PovSrvTest.txt` are disposable-world residue on the shared instance. `[checked]`

## Spawn reference

23 of 168 entries differ, all RtD, each client value equal to server ÷ 1.2. PoV's `Creature density = 1.2` scales rows when a `SpawnSystem` wakes. `[code]` So the client snapshot captured scaled rows. Spawn YAML itself is identical.

## FineDining classification

Live policy files are identical. The reference files disagree:

| Item | Client reference, dated 10-30 | Server reference, dated 10-31 |
|---|---|---|
| `CookedBearSteak_TW`, Fox, Shark, `BerryCakePlains_TW` | 72 h, `RottenMeat` | 48 h, `FineDining_RottenFood`, `no_recipe_or_conversion` |
| `BearSteak_TW`, Fox, Shark, `UncookedBerryCakePlains_TW` | 48 h, `RottenMeat` | Not listed |

`[checked]` for the table. The writer runs only on the config authority, so the client file is stale from a local world. `[code]`

**Measured 2026-11-01.** `[runtime]` M30. Client and server rules matched exactly, so this was never a sync or client/server difference. The stale client file happened to show the correct classification. The current server file showed the real defect: conversions had been lost at runtime on both sides. See [[INV-cooking-conversions]], fixed in 1.0.1.

Reference files generated before and after the fix will differ. Client copies remain stale until a config-authority run regenerates them.
