> **Sanitized snapshot, not raw evidence.** Generated from `EVIDENCE.md`. See `README.md` in this bundle.

# Evidence records

Tiers: **[S]** static, **[H]** server harness, **[C]** real client, **[SV]** real client joined to a dedicated server, **[2P]** two peers (none yet).

Real-client evidence is `evidence/client/`. All E, Alt, Tab, mouse and movement input came from the OS via `tools/ui.ps1` (`SendInput`). The test-only `ClientProbe` set up scenes and logged events; it pressed no keys and called no handlers. Older runs: `evidence/runtime`, `round2`, `round3`. `round4` is empty and was superseded by real client tests.

Instrumentation limits: items, creatures and barrels were spawned by the probe. Barrels and pieces received the `s_creator` value real placement writes, so the hammer was not used. EpicLoot merge cases used marker strings in the real custom-data key. The test forge had its roof rule cleared; the upgrade path itself was untouched.

Disposable data: characters `Povtest`, `Povserver`; world `PovTestWorld`; server world `PovSrvTest` in `.sandbox\srv-saves`. Your other characters and worlds were not used. `<test-character>` was only displayed in the menu.

## M. FeedLikeGrandma 1.2.1 / FineDining 1.1.12 migration

Records: `evidence/migration/`. FeedBarrelStackCompat removed for all of it. Same limits as above, with these differences: rolled EpicLoot items came from EpicLoot's real roller, and timers were set through FineDining's own writer. Items, barrels and tame animals were still probe-spawned; the barrel-protected mark on M3/M4 drops was written by the probe. Characters: `Povtest` and `Povserver`.

**M0. Deployed hashes (SHA-256), identical on client, server and `required`:**

| File | Hash |
|---|---|
| FeedLikeGrandma 1.2.1 | `2C82CFA30DAE1887B63322BE5A48A2CAE3E9BDE56C6EF376F917D184855E0FD0` |
| FineDining 1.1.12 | `D5A1CA660CADE7A2358A88EF9D0A9F47764BB1E3D40E9535721C10464B445849` |
| OdinHorse 1.9.4 | `E209B5387C2C39D735A6F90A065EA46C4A0E63F58BE71B7AE99807F431F8364C` |
| ItemDataStackCompat 1.1.0 | `95F2D0A7451CE3500ECD6305BF95874428DF0AA42A46F63144598C15AFCB1CE2` (1.0.0 `9D0771BC…BD11` withdrawn) |
| BetterSleepHudBridge 1.0.0 | `29513C92366A4A4699A33EA80D8057E7ABE2CD648AACF139AD33B076D0C36E7F` |
| QuietLookups 1.0.0 | `C97DFFDD4F56A83EF65C34C65C3C20CE2D2FD1876FDD22983937BEA9F8262DC5` |
| PlanBuildRejoinCompat 1.0.0 | `1E3C0866D8A19A1A2C99D28306B59B45FA961955EF93DA4488761E56D2C8AF63` |
| HealthRegenCompat 1.0.0 | `4213C5AB69F10BF9A0B0B6293C97411EABBD2041D79BDC0246D24BFFFF3A44AD` |
| RodBagInputBridge 1.3.0 | `93857C9F1CB923974E63B655B729BBBEE809905CB3DD2FD22D24FDB5A6C3DE1D` |
| BurstMultiShotCompat 1.0.0 | `47FDEBAD60A76FFC28CB984AAD8B64858B3CBA12CF29C3D05BAE76BDE8EA5F92` |
| UsefulTankards Bridge 2.1.0 | `4A9DFB3779210F1FD578951283153787CE4A12D97EED08772883ACCB9B40FF83` |

FineDining config hash matches on client and server. Current client, server and `required` compared in `evidence/warnings/final-all.txt`: 96 DLLs, 0 differences, plus 4 BakaLoader server-only.

**M1. Startup [C].** Native contract refused the pack: `native-rejected-LogOutput.log`, ItemDataManager's transpiler bypassed by EpicLoot's replacement. With ItemDataStackCompat, no contract error.

**M2. Feeding [C] [SV].** `1-feeding.txt`, `7-server-feeding.txt`:

| | Client | Dedicated server |
|---|---|---|
| Dispense | True | True |
| Boar | consumed, hungry False | consumed, hungry False |
| Stock | 15→14 | 8→7 |
| Spill | None | None |

**M3. Merges [C].** `2-4-merges.txt`, real `SlowUpdate` with 215 clubs loaded:

| Case | Result |
|---|---|
| Ground, timers 90,000 s + 2,000 s | x2, 1,987 s left: earliest, composed once |
| Inventory, real pickup, 89,998 s + 1,998 s | x2, 1,992 s left: earliest, once |
| Barrel food + ordinary | Separate |
| Barrel + barrel | x2, still protected |
| Two really rolled SwordIron | Separate, magic data kept. Real magic gear is nonstackable |
| Stackable, different real rolled data | Separate |
| Stackable, same real rolled data | x2, data kept |

The stackable rows place real rolled JSON on stackable items, since EpicLoot never rolls those itself. Container stacking: M8.

**M4. Persistence [C] [SV].** World clock moved 32.6 s across exit and reload; inventory, chest and ground timers each moved 33 s. `5-persistence-*.txt`:

| Check | Result |
|---|---|
| Chest, player 2,500 m away | Aged normally, 5,997→5,936 |
| Save, exit, reload | No reset, no double aging |
| Protected ground drop, with timer | Expiry record identical, protected True |
| Server: world saved, client exit, server stop, both restart | All four timers moved 57 s against 56.7 s, carrot still protected (`7-server-persistence.txt`) |

Two earlier server attempts were invalid because `save` was never submitted, so the run above used `-saveinterval 60`. Both explained in M9.

**M5. Regen [C].** Real rolled `HelmetLeather`, 12 rolls to hit AddHealthRegen 1.4. RMB eat and equip. `6-regen-*.txt`:

| State | With compat | Without compat |
|---|---|---|
| 1 food, no gear | 1.2066 | Not repeated |
| Helmet worn, not Cold | **2.6066** | **1.2066** |
| Helmet worn, Cold | Not measured | 0.6033 |

The bonus is exactly +1.4, once. The compat is still needed.

**M6. Existing fixes [C].** `8-smoke.txt`: tankard E open/close, rod E open/close, socketed tankard E storage and Alt+E overlay, one action per press. Real Arbalest with `TripleBowShot`: three bursts, 3 bolts each, proc on first shot only. Needed boss keys set on the disposable world.

**M7. Final smoke [SV].** `final-smoke.txt`, `final-smoke.png`: no test plugins, 91 and 95 loaded, 0 failed, joined through the allow-list, no refusal.

**M8. Container stacking [C], ItemDataStackCompat 1.1.0.** Real E, real LMB pick and drop. `9-container-*.txt`:

| Interaction | Before | After |
|---|---|---|
| Fresh (345,598 s) onto chest stack (909 s) | x2 + x2 | x4, 906 s, same absolute expiry |
| Fresh onto near-expiry chest stack | x1, 146 s | x3, 143 s |
| Chest stack (899 s) onto fresh inventory stack | x4 + x2 | x6, 896 s, same absolute expiry |
| Different rolled data, Thistle, dropped in chest | 14.0 / 11.0 | Swapped, not merged, data kept |
| Same rolled data, Dandelion, dropped in chest | x1 + x1 | x2, data kept, earliest timer |
| Save, exit, reload | 824 s, 67 s | 802 s, 45 s: absolute expiry identical, magic values identical |
| Near-expiry stack reaches zero | 8 s | x3 all `Spoiled=1` |

Consequence of earliest-wins, as observed: fresh food added to a nearly expired stack spoils with it.

**M9. Persistence anomalies [SV].** `evidence/warnings/`: `save-repro.txt`, `save-*.png`, `restore-repro.txt`, and ServerManager's audit and player logs.

| Time | Event | World | Character store |
|---|---|---|---|
| 21:29 | Blind F5, `save`, Enter with rules popup | No `server.saved` | Unchanged |
| 21:30:18 | Console open, 80 ms Enter | Text stays in input, no save | Unchanged |
| 21:30:41 | 200 ms Enter, admin | `Saving..`, `_main.20`, `server.saved` | Content unchanged |
| 21:31:13 | State B: Dandelion x3, Thistle x2, inventory timer changed, new ground drop | Memory only | Unchanged |
| 21:31:17 | Forced server stop | | |
| 21:31:48 | Client writes local character holding B | | Unchanged |
| Rejoin | Server restarted | State A, all timers on the saved clock | **State A wins over newer local B** |

After rejoin, absolute expiry values matched A exactly, the world clock moved 22.8 s and remaining time 23 s, the protected carrot stayed protected, and no history warning appeared.

Earlier failed runs, from the audit log: joined 01:09 and 01:18 UTC, no `server.saved` in either session. The store file stayed at 16:30 until the first save at 18:28, which is why starter gear returned. The source is inferred from those timestamps and confirmed by the deliberate run above.

**M10. ItemDataStackCompat [S] [H] [C] [SV].** `evidence/ids/`. Cases B to F ran 1.0.0 and `2A`, `2B`, `2D` ran 1.1.0, on a disposable headless server copy with test-only `IdsProbe`.

- **Live patches at the probe's Awake:** only BowsBeforeHoes' transpiler. The other copies have not run their lazy constructor.
- **Real client, same dry spot and sequence, constructors forced after startup:** 1.0.0 `dispense result=False`, boar hungry, stock 8, `dispensing stopped … transpiler bypassed` (`client-1.0.0-late-ctor.txt`). 1.1.0 `dispense result=True`, consumed, 8→7, no stop line (`client-1.1.0-late-ctor.txt`).
- **Limit:** the constructors were forced by the probe. Which normal action triggers them first in play was not established.
- **Regression on 1.1.0:** all M3 merge rows reproduced (`regression-merges.txt`). Dedicated-server feed `dispense result=True`, consumed, 16→15 (`regression-server-feeding.txt`). Persistence: M8, M9.

**M11. Sleep HUD and portal warning [S] [SV].** `evidence/warnings/sleep-hud-repro.txt`, ClientProbe `sleepstate`, `hudattach`, `portalstate`. Test mode on the server instance only: Fake Total 1, Simulated In Bed 1, formula 0.6 × 1/1 × 20 = 12. Restored by sync afterwards.

- Hard-coded path false. Real path: `…/hudroot/SeneaLUI_Hud/mapFade/Minimap/minimap/map/small`. `RPC_SleepingPlayerInfo` and `BSB_SyncSleepBoostTotal` unregistered.
- During the boost: extra=0, total=0. After `hudattach`: RPCs registered, extra=12.0, total=785.5, then 1,134.96. World time 12.9×, matching 1 + 12.
- 21 `Gave up` lines in about 14 minutes: `Update` re-arms the retry after each give-up, so the path lookup repeats, on a 0.25 s timer, all session.
- `portalstate` on a join with no warning: revision=1, hasSnapshot=True, entries=0. The earlier warning came with a snapshot before the peer was ready. `RequestRefresh(force)` on game start is the recovery path, read in code and not observed after a drop.

**M12. BetterSleepHudBridge [SV], one client, simulated sleepers.** `evidence/warnings/`: `sleep-ab-baseline.txt`, `sleep-ab-bridge.txt`, `sleep-lifecycle.txt`, `sleep-final.txt`, `sleep-hud-*.png`, `sleep-final-run/`. Test mode on the server instance: Fake Total 1, Simulated In Bed 1. Restored by sync. ClientProbe was test-only and is removed.

The player, not the probe, loaded the smelter in both runs, and also saw the fast burn in the baseline. In the bridge run the probe's `bake` and `ore` read an empty smelter placed inside a runestone, so they are invalid; dropped copper ingots from the loaded smelter are the measure.

| Observed, per real time | Baseline, no bridge | Bridge, 93.4 s |
|---|---|---|
| World clock | +281 in 22 s | +1,144 |
| Crop `TimeSincePlanted` | +281 in 22 s | +94 |
| Smelter | Ore 7→0 in 22 s | +3 ingots |
| Wave clock | +281 in 22 s | +320 |
| Client `CurrentExtraRate` | 0 | 12.0 |

After rejoin, a second night: rate 12.0, total 1,205→2,309, +4 ingots, crop 1×, strip active, then rate 0 and strip inactive at morning.

Lifecycle, `sleep-lifecycle.txt`, each counted with `sleephud`:

| Step | Views | Handlers on live view |
|---|---|---|
| Healthy | 1 | 4 |
| SeneaL minimap element off | 1, parent `small` | 4 |
| Element on | 1, parent `minimap` | 4 |
| View destroyed, BetterSleepBruh rebuild | 1 | 4 |
| `RPC_StopSleep` dropped, health check | 1 | 4 |
| Foreign `RPC_StartSleep`, view destroyed | 1 | 0, foreign entry retained, others cleared, 1 warning, no throw |
| Log Out and rejoin | 1 | 4 |

Strip rectangle during boost: x 1979–2235, y 266–317 on 2560×1440. Exceptions in the lifecycle session and teardown: 0.

Final normal join, no test plugins: client 92, server 96, 0 failed. `Gave up` 0, exceptions 0, final-save warning 0. Test mode off, winter off.

The limits first noted here were closed in M13, except that the bridge's Info lines still do not appear in `LogOutput.log`, so only its warnings are visible there.

**M13. Full single-client matrix [SV], simulated sleepers, player-built homestead.** Not two-peer verification. `evidence/warnings/sleep-matrix-no-bridge.txt`, `-no-bridge-boat.txt`, `-bridge.txt`, `-bridge-boat.txt`, screenshots, `sleep-rejoin-exception.txt`, `sleep-final.txt`. Fake Total 4, In Bed 1, rate 0.6 × 1/4 × 20 = 3.

The player built the homestead, loaded the first no-bridge oven and fermenter, and boarded the no-bridge boat. The probe filled the fermenter and oven through their own handlers in bridge runs, and moved the character onto the deck.

| Interval | Measure | No bridge | Bridge |
|---|---|---|---|
| 31.8 s | World clock | +127 | +127 |
| 31.8 s | Crop | +127 | +32 |
| 31.8 s | Pickable | +127 | +31 |
| 31.8 s | Mead | +293 | +32 |
| 8.3 s vs 31.8 s | Oven | +42.9 | +32.0 |
| ~64 s, aboard | Boat hull | 0.851→0.546, impacts every few s | 0.851→0.851, `lastImpact` unchanged |
| 30 s, not aboard | Boat hull | 0.851→0.851 | 0.851→0.851 |

The no-bridge mead rate exceeds the world clock and was not traced. Bridge bed step: rate 3→6→3, lit 1/5→2/5→1/5, label +12%→+24%→+12%. Away 46 s, objects unloaded: crop, mead and pickable +46. View ID -864476 constant all session; -861294 in the next, replaced by -1921558 after rejoin. Tick 860 at 01:30:28, unchanged at 01:30:56 and 20 s later, 885 after rejoin. 0 exceptions before rejoin.

Invalid or excluded: the first no-bridge run after its element-off step, since BetterSleepBruh built its own view; the first bridge run's oven (loaded too early, burned before the boost) and boat (not boarded). Repeated in `-bridge-boat`.

`DungeonDB.Start` NRE: exactly one on rejoin both with the bridge and without it. Not caused by the bridge; owner traced in M14.

Final: client 92, server 96, 0 failed, `Gave up` 0, final-save warning 0, 94 DLLs identical on client, server and `required`, test mode off.

**Remaining gap, externally blocked (one Steam account):** simultaneous authenticated clients; cross-peer ownership handoff; real players entering or leaving sleep.

**M14. Lookups and rejoin defect [SV].** `evidence/warnings/callers-*.txt`, `log-before.txt`, `log-after.txt`, `final-all.txt`, `final-all-run/`. `tools/CallerTrace`, test-only and removed, recorded the calling method of each failed lookup and wrapped `DungeonDB.Start` in a finalizer.

Full exception on rejoin:

```
NullReferenceException
  UnityEngine.Object.get_name
  PlanBuild.Plans.PlanDB+<>c.<WarnDuplicatesWithDifferentResources>b__9_1(Piece x)
  System.String.Join
  PlanBuild.Plans.PlanDB.WarnDuplicatesWithDifferentResources
  PlanBuild.Plans.PlanDB.ScanPieceTables
  PlanBuild.Plans.PlanManager.DungeonDB_Start_Postfix
  DungeonDB.DMD<DungeonDB::Start>
```

| Second world load | Before | After |
|---|---|---|
| `DungeonDB.Start` | NullReferenceException | ok |
| Expand World Data postfix | Ran | Ran |
| DataForge postfix | **Not run** | Ran |
| HarmonyX warnings, whole client log | 26 | 3 |

A first version of QuietLookups broke ConfigManager's no-argument `GetJoyRightStickY` patch, logged as a `Patching exception`. The rule now matches only a `(bool)` signature. That error is gone. The effect of the skipped DataForge step on actual recipes and pieces was not measured; the evidence is that its setup did not run.

Final, no test plugins, join, Log Out, rejoin, Log Out, quit: client 94, server 98, 0 failed, 0 exceptions, HarmonyX 3 on client and 0 on server.

Single-client limits in M13 still apply.

**Not observed:** dropped-snapshot recovery in PortalRules; winter (no seasonal mod); real worlds and characters; the normal-play trigger for a late constructor.

## M15–M36. Consolidation, overnight round, 2026-10-31

Current facts for these topics. Earlier M records naming standalone DLLs, DungeonDB compat or dead map chips are superseded. One real client, simulated sleepers.

**M15 PlanBuild.** `evidence\warnings\callers-plan{A,B,C}`, `log-plan*`. A and B identical, C clean; 33 blueprints moved and restored. Census on copies: `evidence\ops\census-*.txt`, 178,156 ZDOs. `planbuildObjects=0` is a **filter miss**: unknown `-1813905327` ×1 equals the stable hash of `PlanHammer`, computed from names in PlanBuild's DLL. `-806391820` ×11 unidentified, unknown in both censuses.

**M16 Consolidated matrix.** `evidence\compat\` run.txt, run3, run4, chips-toggle.txt, smelter-boost-start.txt, regen, smoke, merges, strip-*.png. run2 invalid: a missed Log Out click left the pause menu open. First run invalid: join clicks before the menu was ready.

**M17 Wave clock.** `evidence\compat\wave-*.csv`. Fix 1.16× vs upstream 3.29× at world 4.08×; normal 1.00×; boost start continuous; boost end +811 s, surface 0.35 m.

**M18 Ops.** `evidence\ops\graceful.txt`: RCON auth OK, `baka_save` empty reply, save delta 0; CloseMainWindow saved `_main.35`, store updated, exit 5 s; markers persisted. The DLL was built 05:12:38, before this 05:33 run, so the rejoin covers hash `51956F65…`, with probe installed.

**M19 Backup.** Compared against live: store 33/33, Wraeclast 73/73, local worlds 123, characters 5.

**M20 Release.** `patches\dist\SHA256SUMS.txt`. Deployed, server and `required` all `51956F65…`.

**M21 Final clean run.** `evidence\final\`: final.txt, client and server logs (user and Steam ID scrubbed), inventory.png, map.png. Client 86 loaded, server 90, 0 failed, 0 exceptions, 0 HarmonyX, test mode off, 0 test files. `tools\finalrun.ps1` was killed by the session's low-memory reaper after the join (free 3.8 GB of 31.9 GB); `tools\finalstop.ps1` then did Log Out (confirmed), quit and graceful server stop. Second join and Log Out not run in that attempt; superseded by M22.

**M22 Clean rejoin.** `evidence\final\`: final.txt (`logout1 True, rejoin True, logout2 True`), logs, rejoin.png, memory.csv (110 samples: client peak 11,828 MB, server peak 7,049 MB, highest sum 18,265 MB, lowest free 3,146 MB). No test plugins, release hash `51956F65…`. Replaces the probe-assisted rejoin as acceptance evidence.

**M23 Release contents.** Compiler source list and `--no-incremental` rebuild hash match; metadata scan of 172 user strings, method definitions and member references found no probe or test terms. The ClientProbe source is a separate project.

**M24 Characters.** 102 `.fch` files across backups, live store and local characters: 74 mention `PlanHammer` or `BlueprintRune` (68 `<real-character-1>`, 6 `<real-character-2>`), all unprefixed text inside records. Length-prefixed entries, which an inventory item requires: 0. Positive control on the same parser: `Wood`, `Club`, `Hammer` found.

**M25 Missing prefabs on a copy.** `evidence\ops\census-baseline.txt`, `census-after-visit.txt`, `census-detail.txt`, `missingprefab\`. `-806391820` = `BeeQueen`, found by stable-hash match in `OdinPlus-BeeQueen\1.1.2\BeeQueen.dll` among 8.4 million strings; a hash match plus creature fields (`level`, `health`, `seed`, `spawntime`, `spawnpoint`, `huntplayer`) and the existing DropNSpawn `BeeQueen` entry. Unknown objects 12 before and after, 0 differing object lines; 131 containers before, 130 after, 0 PlanBuild items in either. The copy's total fell from 178,156 to 178,144 ZDOs and prefabs from 391 to 389, which was not traced. Client log shows `Network object skipped` for both hashes at their coordinates; screenshots show arrival. The disposable character visited three of the 12 objects; the second creature's own skip line is not logged. Server log: 0 `Destroyed invalid prefab`. Original world files hashed identical before and after. The copy was deleted afterwards.

**M26 BeeQueen entry and Hexium package.** Removed the 12-line entry and two blank lines (256 bytes) from `DNS_spawnsystem.yml` on client and server; files identical; backup in `backups\dns-spawnsystem-pre-beequeen\`. Disposable server start: 90 loaded, 0 failed, 0 BeeQueen or invalid-entry lines; client not re-run. `PoVPlus_Compat-1.0.0.zip` passed `pack.py`; all files at the root; DLL inside matches `51956F65…`. Not uploaded or installed through a mod manager.

**M27 Toxic Aura.** `toxicDeath: n, 0.3, 4, blob_aoe` → `toxicAura: n, 0.1` in `levels.yml` lines 58 and 103 and `karma.yml` lines 49 and 71, chances 10, 10, 10, 30; two comment labels also updated; client and server identical. Server start log: `evidence\ops\toxicaura-server-LogOutput.log`. Not observed in combat, and the client notice was not rechecked in a client session.

**M28 Final warning pass.** Baseline `evidence\baseline\`, acceptance `evidence\acceptance\`, both no test plugins, hash `51956F65…`, client 86, server 90, 0 failed, 88/92/88 DLLs, 0 hash differences, 0 old or test files. Saves: `_main.38`→`.39` and `_main.44`→`.45`, with `server.saved` in the audit log for the baseline. Acceptance memory: client peak 9.2 GB, server 7.1 GB, lowest free 5.5 GB.
Instrumented, disposable data or a copy of Wraeclast (original hashed identical before and after, copy deleted): `evidence\warnings\final-probe` Toxic Aura; first rejoin there is **invalid**, a death falsely satisfied the Log Out check. `final-probe2` portal timeline (rejected t=74.8, accepted 105.2; rejected 245.6, accepted 276.9) and roll `SeekerBrood` 20,000: PageArcane_DO 101, RoyalJelly 9,980. Its second Log Out failed, and quit closed that session. `final-probe3` three joins, twice, naming `PT_BlueRunePebble` at index 3599 of 3785, present again on join 3. The first portal sample is in the run before `final-probe2`, in that run's log.
Probe code stays in `tools\ClientProbe`. Release compile inputs are unchanged.

**M29 Reconciliation, 2026-11-01, read-only plus one server-only census on a Wraeclast copy.** DLLs: 96 − 9 + 1 = 88, reconstructed from `evidence\migration\parity-final.tsv` and later additions. Configs: 23 differ, classified in `Knowledge\Investigations\INV-config-reconciliation.md`; FineDining's reference files disagree and no live measurement exists. Locations: `evidence\ops\census-locations.txt`, 183 types, 12,413 instances, not matching the inactive YAML. Original world hashed identical before and after. The disposable-probe build with `fdrule` was never deployed.
**M30 FineDining on 1.0.0 [SV], disposable, 2026-11-01.** `evidence\fdcheck\run1`, `run2-baseline`: `probe-log.txt`, `server-probe.txt`, `steps.txt`. Joins confirmed by `Connecting to server` plus world load; saves by `server.saved` 47→48 and 49→50. `ClientProbe` and server-only `FdServerProbe` observed; fixtures were console `spawn` and auto-pickup, 5 of 12 foods. Client and server rules identical. `BearSteak_TW`, `FoxMeat_TW`, `SharkMeat_TW`, `UncookedBerryCakePlains_TW` `NotTracked`, no timer, unchanged after `skiptime 270000`. Cooked foods 48 h, `FineDining_RottenFood`. Timeline: stations 12/23/32/46 after `ZNetScene.Awake`, 10/20/31/41 after `ValheimCuisinePlugin.Update` called `ReapplyConversions`, on both sides. Server writes and expiries 0. The earlier attempt never joined: an elevated Task Manager blocked `SendInput` (`join-stuck.png`).

**M31 FineDining on 1.0.1 [SV].** `evidence\fdcheck\` top level. Fixture `spawn` plus `Player.Pickup` via `pickall`, all 12 foods. All eight disputed foods `Enabled`: cooked and `BerryCakePlains_TW` 72 h `CookingStationOutput`; raw and `UncookedBerryCakePlains_TW` 48 h `CookingStationInput`. Timers fall with the world clock; `server.saved` 51→52; rejoin confirmed; after a 75 h skip all expire to `RottenMeat`, Raspberry 95.86→20.85 h. Server rules identical; stations hold 12/23/32/46 after each rebuild, with 14/26/33/51 briefly on rejoin. Iron station instance allows the three meats. BetterSleepBruh test mode was on, and boosted the world clock in the first interval. A full cook was not run.

**M32 1.0.1 acceptance [SV], no test plugins.** `evidence\acceptance-1.0.1\`. Client 86, server 90, 0 failed, both joins and Log Outs confirmed, `_main.53`→`.54`, 0 exceptions, 0 HarmonyX, 88/92/88 DLLs, 0 hash differences, 0 test files, test mode and winter off, `Location data = false`. Compile inputs: ten module files plus generated attributes; rebuild reproduces `8B766887…`; 190 strings and method names show no probe or test terms; DLL inside the zip matches. New client line `LiberationSans SDF Font Asset`, also present in run 2 before the fix. Lowest sampled free memory 597 MB, at the last sample: the sampler stopped at 13:00:35 during the first world load, so the rest of the run is unsampled and the true low is unknown. Client working set 10,982 MB, server 6,584 MB; the 13,698 MB remainder is an accounting difference, not process memory. See `Knowledge/Operations/OPS-memory.md`. `evidence\baseline\memory.csv` is invalid; two samplers wrote to it.

**M33 Older-vault comparison.** Raw and CRLF-normalized SHA-256 of all files: main 128, Claude 126, Runtime Integration 126. Main and Claude: 125 common, 107 raw identical, 117 normalized identical, 8 substantive, 3 main-only, 1 Claude-only. Claude and Runtime Integration: 126 common, 1 raw identical, 126 normalized identical. Originals untouched.

**M34 Fresh clone, 2026-11-01.** Clone of `ave-satanas/PoVPlus.Compat` at `eed8d74`, in a temporary folder on this machine. README build: 0 errors, 0 warnings. `pack.py` failed as documented, because the DLL was not in `../dist/`; with it copied, it passed, six files at the zip root. DLL `62F3E78B…`, release `8B766887…`, same size, 482 bytes differ. 124 method bodies hash identically, 37 types, source text identical after line endings. Embedded PDB path and module ID differ. README corrected. Not tested on a machine without the pack installed. The temporary clone was deleted.

**M35 Reviewer workflow, 2026-11-01.** Fresh clones of both repositories in a temporary folder on the original Windows machine, documented steps only. First pass at Knowledge `6870888`: every bundled file failed `--verify`, because `core.autocrlf=true` rewrote line endings on checkout; `.gitattributes` now keeps `EvidenceBundle/` byte-exact. Other results at that pass: all pinned cross-repository URLs resolved in the clones, `--applies` and `--external` ran, source build 0 errors with explicit `-p:GameManaged` and `-p:Profile`, package passed, IL digest of that build equalled the tested release, `1DC63390…DF1D`, 124 bodies, 37 types. Second pass at `01ba857`, new clone: validation and `--verify` passed; a tampered file and an unlisted file were each detected. No other machine, operating system or clean toolchain was tried. The game was not launched and nothing was installed.

**M36 Review package, 2026-11-01.** `PoVPlus.Knowledge` history contains private terms in earlier copies of the bundle script, so reviewers get a separate private repository with one commit. Exact tested DLL and zip added after checking: no embedded resources, own namespace only, references without bundling, MIT. Client config frozen once: 274 files, 8.7 MB, 6 redacted, 295 excluded, no included file changed after acceptance. Excluded player identity files map player IDs to Steam IDs. The leak guard stopped the first attempt on a false positive in the password rule; the rule was corrected, not the guard. `PoVPlus.Review` commit `de91424`, exported from internal `e8e6377`, verified from a fresh clone on this machine: one commit, no private terms in history or tree, validation and `--verify` pass, tampered config and DLL each detected, DLL and zip hashes match, IL digest matches, all pinned links resolve without the internal repository. `PoVPlus.Compat` `77ab9d3` points its audit link there. No reviewer has access yet.

**M37 Gale profile, 2026-11-01 [SV], no test plugins.** Gale 1.23.3, with a copy of its database read for the records. Before: 87 records, no PoVPlus.Compat, `PoVPlus.txt` stale. PoVPlus.Compat 1.0.1 imported through Gale's UI into `plugins\PoVPlus_Compat`; `plugins\PoVPlus.Compat` deleted after backup to `backups\gale-pre-compat-import\`; `PoVPlus.txt` regenerated by Gale, 87 packages. After: 88 records, every DLL matches server and allow-list, 0 repeated DLL names, 0 old folders. 70 DLLs identical to Gale's cached packages, 15 packages with no cache copy. Three offered upstream updates not applied. Protected config unchanged: seven files byte-identical to the backup and identical between client and server, all 274 frozen files match live. `evidence\acceptance-gale\`: both Log Outs and rejoin confirmed, `_main.55`→`.56`, client 86, server 90, 0 failed, 0 exceptions, 0 HarmonyX, 88/92/88 DLLs, 0 hash differences, 0 test files, GUIDs unique on each side, test mode and winter off, DLL and `required` `8B766887…`. Warnings: client 41, server 523, count differences only, including portal race 1 against 2. First full memory timeline: start 17,999 MB available, low 3,296 MB at 13:56:42. See `Knowledge/Operations/OPS-gale-profile.md` and `Knowledge/Operations/OPS-memory.md`. Disposable `PovSrvTest` only, one account, no cooking.

## F1. Feed Barrel and ground merge

*F1 below describes the retired FeedBarrelStackCompat. See M for current behaviour.*

**[S]** EpicLoot's prefix always skips `AutoStackItems`; FLG refuses any prefix. Dispensing also needs a player-placed barrel (`s_creator`), hungry owned animal within 10 m, a rule match and a path. Vanilla only merges drops when more than 200 are loaded.

**[C]** `f1-b2.txt`, `scenario-A-unpatched.txt`:

| | Unpatched | Patched |
|---|---|---|
| Startup | `Feed Barrel dispensing stopped` | no error |
| Hungry tame boar beside stocked, intact barrel | 14 `dispense result=False`, stock unchanged, still hungry | `dispense result=True`, consumed, 8→7, fed, no spill |

**[C]** Real merges with over 200 drops, real `SlowUpdate`, `merge-*.txt`:

| Case | Unpatched | Patched |
|---|---|---|
| Expiry 90,000 s + 2,000 s | Later date kept, 0 compose | **Earliest kept**, compose fired |
| Barrel food + ordinary | **Merged** | Separate |
| Barrel + barrel | Merged | Merged, still protected |
| Different EpicLoot data | Separate | Separate |
| Same EpicLoot data | Merged | Merged, data kept |

**[SV]** `server-barrel.txt`: dispense true, consumed, 6→5, no spill, on a dedicated server with the animal owned by the client.

**[2P]** None. Not observed: rolled magic stackables; a peer without the patch.

## F2. AddHealthRegen

**[S]** `AddHealthTicks` lives only in a transpiler on the body FineDining skips.

**[C]** Real raspberry eaten with RMB, real magic helmet equipped with RMB, visible in the head slot, heal observed from the game's own tick. `f2-B.txt`, `scenario-A-unpatched.txt`:

| State | Unpatched | Patched |
|---|---|---|
| 1 food, no gear | 1.0867 | 1.0867 |
| Helmet, gear reads 3 | **1.0867** | **4.0867** |
| Helmet off | 1.0867 | 1.0867 |

HP gain equalled the amount. The bonus is exactly +3.0 at multiplier 1.0, so it applies once. Food/PoV contribution is the constant 1.0867. Under Cold the tick halved to 0.5434, so the bonus sits before the multiplier by code **[S]**; gear with Cold was not measured.

**Not observed:** rolled legendaries or shard stones.

## F3. Rod and tankard input

**[S]** Stale `SocketsUI` bind latches each mod's gate true. EpicLoot yields bare Use only over registered containers; the installed shortcut is bare `E`.

**[C]** No bridges, `input-A-no-bridges.txt`: game saw the press with correct item; nothing opened. Socketed sword still toggled.

**[C]** With bridges, `input-matrix-v1.txt`, `v3.txt`, each one action per press:

| Action | Result |
|---|---|
| Tankard E / E / E / E, mouse unmoved | open, close, open, close |
| Rapid double E | open then close |
| Rod same sequence | same, docked, grid does not move |
| Tankard open, E over rod, E | closes, then bag opens; never both |
| Bag open, E over tankard, E | closes, then storage opens |
| Socketed sword E / E | overlay opens, closes |
| Socketed tankard or rod: E / E | container opens, closes |
| Socketed tankard or rod: Alt+E / Alt+E | overlay opens, closes |
| Overlay open: plain E | overlay closes |
| Unsocketed tankard or rod: Alt+E | container opens |
| E over raspberry | inventory stays open |
| E on empty cell | inventory closes |
| Drag item, RMB equip | normal |

**Precedence and behaviour change:** EpicLoot's code gives bare Use to a registered container, and `Alt` matches its legacy socket default and BowsBeforeHoes' yield. So `E` = container, `Alt+E` = sockets on those two item types. Only socketed tankards and rods change. Gamepad gesture not tested.

**[SV]** `server-tankard.txt`: open, close, open, close.

## F4. Server access

**[S]** Earlier reasoning covered ServerSync only.

**[SV]** Real outcomes:

| Setup | Outcome |
|---|---|
| Server FLG 1.1.10, client 1.2.0 + patches | Refused. `validation.hash_not_allowed`, "Update or reinstall: FeedLikeGrandma" |
| Empty allow-list (disposable save root) | Refused, all mods "not allowed" |
| Client running during update | Script failed on a locked cache file |
| Matched, no probe, new character | **Joined.** `Got character ZDOID from Povserver` |
| Matched, character with local world history | Refused by ServerManager's character protection |

Final: 93 DLLs identical; only four server-only tools differ (`parity-final.tsv`). The real server world was not started.

## F5. Seekurai Wings

**[C]** Real forge UI, real Upgrade click, `f5-A-typo.txt`, `f5-B-fixed.txt`:

| | Panel | Black Metal | Trophy | Result |
|---|---|---|---|---|
| Typo | Trophy 1 only | 80→80 | 8→7 | q2 |
| Fixed | Black Metal 10, Trophy 1 | 80→70 | 7→6 | q2 |

## F6. Lightning trap

**Intent:** the mod's config text lists `Tared` as a valid name and says trap effects stack on damage effects, so `Tarred` is a misspelling of an authored effect.

**[C]** Real trap, real Greydwarf, trap fired by the game:

| Value | Status applied |
|---|---|
| `Tarred` | Lightning only |
| `Tared` | **Tared and Lightning** |

## H1. SecondaryAttacks × EpicLoot burst: real defect, patched

**Expected:** SA documents each repeated shot as the weapon's pattern. Its own disabled compat restores `m_projectiles`. EpicLoot's multishot trades damage (×0.4) for count.

**[C]** Real Arbalest, real middle-click, `Crossbow Preset = Burst`, `h1-A-unpatched.txt`, `h1-B-patched.txt`:

| Burst | Shot 1 | Shot 2 | Shot 3 | Bolts |
|---|---|---|---|---|
| Plain | 1 × 111 | 1 × 88 | 1 × 79 | 3 |
| Proc, unpatched | 3 × 36 | **3 × 105–158** | **3 × 99** | 3 |
| Proc, patched | 3 × 45 | 1 × 131 | 1 × 105 | 3 |
| No proc | 1 × normal | 1 × normal | 1 × normal | 3 |

Consistent over 4 patched and 3 unpatched bursts. **Judgment:** a free proc on later shots contradicts both contracts. The patch stands down if SA's own compat binds again. **Limit:** EpicLoot's other projectile effects in bursts were not tested, and shots after the first don't re-proc.

## H2. Crossbow mastery: rejected

**[H]** `Masteries.Selected` blocks both same-effect pairs on the real tree. Not reopened.
