---
id: INV-missing-prefabs
type: investigation
status: bounded
created: 2026-11-01
updated: 2026-11-01
related: [DEC-planbuild-removal, INV-planbuild, DIAG-warning-ledger]
---
# BeeQueen and PlanHammer

## BeeQueen

Prefab hash `-806391820` ×11 in `Wraeclast` matches `BeeQueen` from OdinPlus-BeeQueen in Gale's cache, which is not in this pack. Hash match plus creature fields. `[runtime]` M25 The dead spawn entry was removed at the user's request. Backup: `backups\dns-spawnsystem-pre-beequeen\`. M26

## PlanHammer

Hash `-1813905327` ×1, on the ground at (-940.6, 1177.2). Characters hold records only, no item entries, across 102 files; a positive control confirmed the parser. `[runtime]` M24 No container or stand holds one, 131 containers checked. M28.

## Behaviour

On a copy, a disposable character visited the hammer and two creatures. PoV logged `Network object skipped`. After world save and reload, all 12 object lines were identical, with 0 `Destroyed invalid prefab`. `[runtime]` M25

**Limits:**
- Three of 12 objects visited, and one creature's own skip line was not logged.
- Vanilla can destroy missing-prefab objects from a server-side scene. `[code]` A client-hosted world might do that. Not tested.
- Reinstalling either mod should restore them. `[hypothesis]`
- The real character was not loaded on this pack with those records.
- The copy's total fell by 12 ZDOs across that test, untraced.
