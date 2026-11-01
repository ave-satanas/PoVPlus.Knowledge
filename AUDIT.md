# Audit guide

For an independent reviewer. The aim is to let you challenge these conclusions, so treat them as claims to test, including any written by the original agent.

## Scope

**Audited:** PoVPlus.Compat 1.0.1, the config changes around it, and the evidence for them.

**Not claimed:** that the pack is fully compatible. Most mod interactions were never examined. See [coverage](Compatibility/CMP-coverage.md). In the server's patch map, 598 methods are patched by two or more owners; only a handful of interactions were studied deeply.

**Architecture:** one BepInEx plugin, GUID `0.povplus.compat`, with nine modules, each with its own Harmony owner and guards. `Awake` installs lookups; the rest install just before FeedLikeGrandma and FineDining's `Start`. See [architecture](Systems/SYS-compat-architecture.md).

## Reading order

1. [README.md](README.md) for the artifact and source relationship.
2. This file's claim table.
3. [Corrections](Investigations/INV-corrections.md). Earlier conclusions were wrong often enough that this is a fair measure of reliability.
4. [Source at the tested commit](https://github.com/ave-satanas/PoVPlus.Compat/tree/eed8d74b3700a044eb9dbce7053fe0ec2018ca69), with [module records](Compatibility/CMP-index.md).
5. [Evidence record](EvidenceBundle/reports/EVIDENCE.md) for the ids below.
6. [Evidence bundle](EvidenceBundle/README.md) for what is and is not available.

## Release, source and evidence

| Link | Status |
|---|---|
| Source → DLL | Proven by IL, not by byte identity. See [README](README.md#what-the-evidence-concerns) |
| DLL → deployment | Hashes matched on client, server and allow-list. `[checked]` on the original machine |
| Deployment → runtime evidence | M31 and M32 ran 1.0.1. **Most feature evidence predates it.** See below |
| Evidence → this repository | Sanitized copies and filtered excerpts; originals are local only |

**Which build each result used:** M16 and M17, the consolidated feature matrix, ran on builds before 1.0.1. 1.0.1 added `CuisineConversions.cs` and one line in `Plugin.cs`, leaving the other modules' source unchanged. Rerun on 1.0.1 itself: startup, join, rejoin, save, shutdown, FineDining rules and timers. **Not rerun on 1.0.1:** regen, feeding, rod, tankard, burst, map chips, sleep and wave checks.

## Claims and evidence

Tiers: S static, H server harness, C real client, SV real client on a dedicated server. Bundle paths are under [`EvidenceBundle/`](EvidenceBundle/README.md). All runtime evidence is one Steam account on one machine.

| Claim | Version | Evidence and type | Conditions | Establishes | Does not establish |
|---|---|---|---|---|---|
| 1.0.1 loads, joins and rejoins cleanly | 1.0.1 | M32; `results/acceptance-1.0.1-final.txt`; SV runtime | No test plugins, disposable world, one client | 0 failed plugins, 0 exceptions, 0 HarmonyX warnings in that run | Function of each module; other worlds; length of play |
| ValheimCuisine's rebuild removed 11 conversions | 1.0.0 | M30; `results/fdcheck-1.0.0-*.txt`; SV runtime with probes | Disposable, probes on client and server | Counts fell 12/23/32/46 → 10/20/31/41, caller `ValheimCuisinePlugin.Update` | Exact trigger of Cuisine's reload flag; other mods' stations |
| `CuisineConversions` keeps them | 1.0.1 | M31; `results/fdcheck-1.0.1-*.txt`; SV runtime | As above | Counts hold across rebuilds and rejoin; rules correct on both sides; iron station allows the raw meats | **A finished cook.** Duplicates appear for about 30 s on rejoin, effect unmeasured |
| Disputed foods get correct timers and expire | 1.0.1 | M31; same files | `spawn`, then probe `pickall`; expiry by `skiptime` | Rule, timer, persistence through save and rejoin, rot item | Normal-play expiry; existing saves |
| FineDining processes timers on the owning peer | 1.0.1 | M31 counts plus decompiled `DecayRuntime`; `[code]` | 0 server writes in three runs | Matches the code | A live server-owned container |
| Feeding and merges work with native integration | Pre-1.0.1 | M2, M3, M8, M16; C and SV runtime | Probe-spawned barrels and animals | Dispense, consume, merge rules | Rerun on 1.0.1; two peers |
| Regen fix restores `AddHealthRegen` once | Pre-1.0.1 | F2, M5, M16; C runtime | Real rolled helmet | +3.0 and +1.4 applied once | Gear under Cold with the fix; legendaries |
| Sleep protections work under SeneaL UI | Pre-1.0.1 | M12, M13, M16; SV runtime | **One client, simulated sleepers**, BetterSleepBruh test mode | Crop, pickable, mead, oven, smelter near 1× | Real sleepers, handoff between peers |
| Boosted waves reduced | Pre-1.0.1 | M17; raw CSV local only; SV runtime | One boat, simulated sleepers | 3.29× → 1.16× at world 4.08× | **Normalized waves.** A step remains at boost end, and a 1% hull loss nearby is unattributed |
| Boat protected during a boost | Pre-1.0.1 | M13; SV runtime | Unmatched boarding: user in one run, probe in the other | 85.1→54.6% against 85.1→85.1% | A controlled comparison |
| PlanBuild caused the rejoin NRE, not blueprints | Pre-1.0.1 | M14, M15; SV runtime | Cases A, B, C on disposable data | Identical failure without blueprints; clean after removal | All PlanBuild-dependent content |
| Portal snapshot recovers after rejection | 1.0.0 | M28; SV runtime | 3 of 3 instrumented joins | Recovery about 30 s later | **A populated catalog.** Both worlds had 0 entries |
| `SeekerBrood` page drop restored | 1.0.0 | M28; SV runtime | 20,000 rolls on a live creature | 0.505% against 0.5% target | **Real kills**; starred scaling |
| Missing prefabs stay dormant | Pre-1.0.1 | M25; copy of `Wraeclast` | 3 of 12 objects visited | Unchanged after save and reload on a dedicated server | Client-hosted worlds, where vanilla may delete them |
| Release contains no test code | 1.0.1 | M32; metadata scan | Compile inputs, 190 strings, method names | No probe terms found | That a pattern scan is exhaustive |
| Save interval is 15 minutes | Original world | Store snapshot cadence; `[checked]` | Timestamps only | Cadence of snapshots | **The launch argument.** Its source log was not recovered |
| Shutdown saves gracefully | Pre-1.0.1 | M18; `results/graceful-shutdown.txt` | Disposable, close request | World and store saved, exit 5 s | BakaLoader's stop on the real server; `baka_save` does not work |

## Known weaknesses

- **A clean log proved nothing here.** The worst defect, lost conversions, logged no warning through two "clean" acceptances. Passing acceptance is weak evidence of function.
- The original agent wrote the code, tests and reports. Evidence depends on its probes, which are in `EvidenceBundle/harness/`.
- `ClientProbe` set up scenes and spawned items in many tests. Its `Resolve` calls can trigger a lazy cache build.
- Timing-dependent input automation failed several times; invalid runs are recorded in EVIDENCE.
- Memory headroom fell to 597 MB at an incomplete sample. See [memory](Operations/OPS-memory.md).

## Tests that could falsify these conclusions

| Test | Would show |
|---|---|
| Cook `BearSteak_TW` at an iron station on 1.0.1 | Whether the fix works end to end |
| Rerun regen, feeding, rod, tankard, burst and chips on 1.0.1 | Regression from the new module or load order |
| Remove Cuisine; check counts and rules | Whether the cause is correctly attributed |
| Two real clients sleeping, with ownership handoff | Whether the sleep protections hold at all between peers |
| Join a world with public portals during the race | Whether a populated catalog recovers |
| Kill `SeekerBrood` many times | Whether real death uses the same path |
| Host `Wraeclast` copy from a client | Whether dormant objects are deleted |
| Change EpicLoot's `AutoStackItems` prefix | Whether `ItemDataStack` really declines safely |
| A long session with several players | Memory, and anything short tests miss |

## Open items and accepted limits

- **Two-peer testing:** unavailable, as only one Steam account exists. Simulated sleepers and one-client ownership changes are not two-peer evidence.
- **Original world:** `Wraeclast` has not run on this code. Tests used disposable data or copies, and originals were hashed before and after.
- **Cooking:** no finished cook on 1.0.1.
- **Waves:** improved, not normalized.
- **Unexplained:** what destroys `PT_BlueRunePebble`; five report-file differences; a station instance showing 24 conversions against the prefab's 23.
- **Location data:** disabled, with intent undecided. See [record](Investigations/INV-location-data.md).
- **Full list:** [remaining work](Operations/OPS-remaining-work.md).

## Reproducibility

**Exercised so far (M35):** fresh clones in a temporary folder, on the original Windows machine, with its Python, .NET SDK, git settings and real Valheim and mod assemblies. Levels 1 and 2 passed there. **No other machine or operating system has been tried**, and Level 3 was not exercised as a reviewer workflow.

| Level | Needs | What you can do |
|---|---|---|
| **1. Clean clone** | Python 3 | `python tools/validate.py`, `python tools/make_evidence_bundle.py --verify`, read everything here |
| **2. Build** | .NET SDK, PowerShell 7, your own legally obtained Valheim and the mods | Build the source; compare with `tools/il-digest.ps1`. See the [source README](https://github.com/ave-satanas/PoVPlus.Compat) |
| **3. Runtime** | Valheim on Steam, the 87-mod pack at the versions in [inventory](Mods/MOD-inventory.md), a dedicated server | Rerun checks. `EvidenceBundle/harness/` shows how; adapt paths and UI coordinates, which assume 2560×1440 |
| **4. Historical only** | Cannot be reproduced from here | Raw logs, saves, screenshots, `Wraeclast`, and runs on the original machine |

Level 2's digest should match only with the same referenced assemblies and a similar compiler: SDK 10.0.100 was used. A mismatch needs investigating, but does not prove different source.

The pack itself is not distributed here. Rebuilding it needs the versions in the inventory from their own sources, and config values are only partly recorded in this repository. A reviewer without the original profile can test individual interactions, but cannot exactly reproduce the original environment.
