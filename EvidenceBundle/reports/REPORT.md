> **Sanitized snapshot, not raw evidence.** Generated from `REPORT.md`. See `README.md` in this bundle.

# PoVPlus compatibility handoff

**State, 2026-11-01:** all compat fixes are consolidated into one plugin, `PoVPlus.Compat` 1.0.1, deployed to client, server and allow-list. PlanBuild is removed. **Game and server are stopped.** Your real worlds and characters have not run on this code. Two-peer behaviour is untested: only one Steam account exists, and that gap is accepted, not a blocker.

**Knowledge base:** `Knowledge/INDEX.md`. Warnings below are documented or bounded, not all resolved.

**Defect found and fixed in 1.0.1:** ValheimCuisine's post-load rebuild was removing 11 Monstrum, Warfare and Wizardry cooking conversions on client and server. Those foods could not be cooked at those stations, and raw meats never spoiled. Details: `Knowledge/Investigations/INV-cooking-conversions.md`. No warning ever appeared for it, so the 1.0.0 clean logs below did not reveal it. 1.0.1 passed its own clean acceptance, M32. A full cook was not run.

**Final production-readiness pass, 2026-10-31:** no production code change was justified, so PoVPlus.Compat stays 1.0.0 with the same hashes. One config fix was added, the `SeekerBrood` page drop. Final clean acceptance passed: join, gameplay, `save` confirmed by `server.saved`, Log Out, rejoin, Log Out, quit, graceful server close.

**Clean acceptance passed.** The exact release DLL completed join → Log Out → rejoin → Log Out → quit with no test plugins. **Measured memory:** client peak 11.8 GB, server peak 7.0 GB, highest combined working set 18.3 GB, lowest free memory 3.1 GB on this 31.9 GB machine. Earlier, 3.8 GB free with both running and the session idle was enough to trigger the reaper that killed the first attempt. A later run started with 19.4 GB free and reached 597 MB at an incomplete sample. About 21 GB is provisional guidance, not a requirement. See `Knowledge/Operations/OPS-memory.md`.

**Limitations that remain:** boosted waves are improved, not normalized; the boost end still steps; and the 1% hull loss is unattributed. See Feature checks.

Records: [EVIDENCE.md](EVIDENCE.md) M15–M36, coverage: [COVERAGE.md](COVERAGE.md). This does not establish that the whole pack is compatible.

## PoVPlus.Compat 1.0.1

| | |
|---|---|
| DLL | `PoVPlus.Compat.dll`, SHA-256 `8B766887E53A9FD285D68DF4150FE153C8E3831BA07E57FFAB0BF4CA487A75EB` |
| Release | `patches\dist\PoVPlus_Compat-1.0.1.zip`, Hexium-format, SHA-256 `77419c0891888222c794938e2710d172107b5c187a9d91adf4c7b6d2fd2159c1`. 1.0.0 is in `patches\archived\`; `SHA256SUMS.txt` beside it. Rebuild with `python patches\PoVPlus.Compat\pack.py`. Not uploaded: Hexium accepts each version once |
| Production contents | Compiler inputs are exactly the nine module files plus generated attributes; a `--no-incremental` rebuild reproduces the same hash. All 172 embedded strings and all method and member references contain no probe, test, fake, census, trace or dump terms. References are the game, BepInEx, Harmony, EpicLoot and SeneaL-API only |
| Identity | GUID `0.povplus.compat`, one BepInEx entry, one DLL. The GUID sorts first; the deferred stage runs just before FLG and FineDining `Start`, where their contract checks live |
| Source | `patches\PoVPlus.Compat\` |
| Live | `...\profiles\PoVPlus\BepInEx\plugins\PoVPlus.Compat\` |

**Included:** ItemDataStack, HealthRegen, BurstMultiShot, RodBag, Tankards (the SeneaL UsefulTankards bridge), SleepHud (was BetterSleepHudBridge, plus wave and strip fixes), QuietLookups, EpicMapChips, and CuisineConversions (1.0.1). A missing integration logs one warning and leaves the others on. Original Harmony owner IDs are kept so existing guards still match. No config, no saved data.

**Retired standalone DLLs:** HealthRegenCompat, ItemDataStackCompat, RodBagInputBridge, BurstMultiShotCompat, BetterSleepHudBridge, QuietLookups, PlanBuildRejoinCompat, SeneaLUI UsefulTankards Bridge. Source and last builds: `patches\archived\standalone-superseded\`, marked superseded. FeedBarrelStackCompat stays retired.

**Dependencies:** the Hexium manifest requires only BepInExPack, because each integration is optional; a site install of it does not pull in the mods below. **Tested with:** Valheim 1.0.21, BepInExPack 5.4.2360, EpicLoot 0.15.4, FLG 1.2.1, FineDining 1.1.12, BetterSleepBruh 2.1.2, SeneaL UI 1.2.8, UsefulTankards 1.0.9, TrollingFishing 1.1.5. Upstream DLLs stay separate.

**Other clients must, all at once:** delete `PoVPlusPlus-Compat` and `SenealUI-UsefulTankards-Bridge-*` from `plugins`, remove PlanBuild, install the zip through their mod manager, or into `plugins\PoVPlus.Compat\`, and have FLG 1.2.1, FineDining 1.1.12 and OdinHorse 1.9.4. Otherwise the allow-list refuses them.

**If you switch your own install to the Hexium package,** remove `plugins\PoVPlus.Compat` first or the plugin loads twice, then sync: the folder name changes, and the file path under `required` changes with it.

**After any DLL change:** run `Update Server Mods.ps1` with the game closed. It remains the sync process; `tools\compatsetup.ps1 -NoTest` wraps it.

## PlanBuild: removed

- **Blueprint hypothesis: refuted.** The rejoin exception and 12 dead keys were identical with blueprints present (A) and with an empty library (B). Your 33 `.blueprint` files were not the cause. Removing PlanBuild (C) gave 0 exceptions and both DataForge postfixes ran.
- **What you lose:** planned ghost building and blueprint capture/placement. Dirty Hoe covers terrain tools only, so it is not a replacement for those.
- **One world object is orphaned, and it is dormant.** `Wraeclast` holds one `PlanHammer` on the ground at (-940.6, 1177.2). My earlier "0 PlanBuild objects" was a name-filter miss. On a copy, a disposable character visited it; Path of Valheiman logged `Network object skipped` and the object stayed in the save, unchanged after a world save and reload. It is unusable without PlanBuild and would return if PlanBuild does.
- **No character holds one.** `<real-character-1>` and `<real-character-2>` have record-only entries (a crafted list and timestamps). A parser that finds real items such as `Wood` and `Club` found no `PlanHammer` or `BlueprintRune` item entry in 102 character files, so no character load test applied. None of 131 world containers or any item stand holds one. How the mod reading those records behaves on your real character is not tested.
- **Kept:** blueprints at `<steam-library>\Valheim\BepInEx\config\PlanBuild\blueprints`, the mod at `patches\archived\PlanBuild-removed\`.
- **DataForge after a real rejoin:** recipes 1991, hammer 718, hoe 7, items 3785, same hash. Native craft Club 0→1; `wood_floor` placement 148→149. This pack has no active DataForge overrides, as its yml entries are all commented.

## Feature checks on the consolidated build

One real client, disposable dedicated server. Sleepers are simulated by BetterSleepBruh's test mode, and ownership changes are single-client.

| Check | Result |
|---|---|
| Startup, clean rejoin | Release DLL, no test plugins: client 86, server 90, 0 failed; both joins and both Log Outs confirmed; 0 exceptions, 0 HarmonyX warnings on either side. The rejoin reloaded in 48.3 s |
| Deployment | Client 88 DLLs = `required` 88; server 92, same hashes apart from BakaLoader's own. 0 old or test files |
| Feeding, ground merges | Dispensed and consumed, stock 8→7, merges match |
| Regen, real helmet | 1.397 → 4.147 (+2.2 ×1.25 Rested), applied once |
| Rod, tankard | E and Alt+E pass with real input |
| Burst | 3 bolts, triple on first shot only (local world, for boss keys) |
| World-settings lock | Server entry refused an edit even with an admin session; client-owned entries editable. Server-side difficulty RPC is code-derived, not exercised |
| ConfigManager UI | Opens from MODS SETTINGS with PoV's section. Edit persistence observed only through the lock check above |
| BowsBeforeHoes socket | Tooltip shows shard slots, real E opens the quiver. The open prompt text is absent under SeneaL: cosmetic |
| SeneaL PoV bindings | Module on, 59 cells; belt and Wishbone both equip. `Dual=False` is the stale probe, harmless |
| Map filters | **My two earlier claims were wrong.** EpicLoot's filter icons sit under SeneaL's rail, unreachable; SeneaL's chips did nothing. EpicMapChips connects them to `Minimap.ToggleIconFilter`. Real clicks toggle both, before and after rejoin. Visible in the clean run |
| Sleep strip | Beside the map, clear of SeneaL's card, with the element on (x1979–2235) and off (x1957–2224). Map at left edge flips it to x291–547. Bed 3→6→3, pillows 1/5→2/5→1/5, +12%→+24%. Hides at morning |
| Sleep lifecycle | 1 view, 4/4 handlers, 1 Hz check stops at Log Out and resumes; new view on rejoin; no retry loop, 0 give-up warnings |
| Timers in a 4× boost | Mead +1×, crop 1×, pickable 1×, oven 34.1→76.3 in 42.3 s, smelter about 1 s/s, continuous across boost start |
| Rejoin, persistence | CloseMainWindow saved and exited in 5 s; markers Dandelion ×7, Thistle ×3 survived restart and rejoin, and show in the clean run |

**Wave clock.** Upstream defect, reduced, not fully normalized. `WaterVolume.StaticUpdate` runs from three update paths and BetterSleepBruh adds time on each, against its own "decouple" setting. Under the same boost and boat: upstream 3.29×, fixed 1.16× at 4.08× world speed. Normal play 1.00×. Boost start is continuous (surface 0.001 m). **Boost end still steps** by design: clock +811 s, surface 0.35 m. Hull lost 1% around then, the same size as ordinary shoreline drops, so it is not attributed to the step. Hull stayed flat during the boost.

**Previous boat evidence stands but is not a matched fixture:** no bridge 85.1%→54.6% (you boarded), bridge 85.1%→85.1% (probe boarded).

**Mead anomaly explained, no defect.** FineDining's environment speed gives about 2.3× normally, so unprotected boost was 9.2× (4×2.3). Bridged mead runs 1× in a boost, which is slower than normal but safe.

## Warning ledger, final production-readiness pass

Logs: `evidence\baseline\` (before config change), `evidence\acceptance\` (final clean run). Client 41, server 523 lines, of which 475 are server audio. 0 exceptions and 0 HarmonyX warnings in both runs. **Instrumented runs** are `evidence\warnings\final-probe*`. No zero-warning claim.

| Message | Source and trigger | Consequence | Evidence, confidence | Fix or reason left | Verification | Uncertainty |
|---|---|---|---|---|---|---|
| `Seeker_Brood` drop target | Magic_Supremacy `DropsFrom.Add("Seeker_Brood", 0.005f, 1, 2)`, a typo, on each prefab pass | `SeekerBrood` never dropped `PageArcane_DO` | Source read. High | **Fixed by config:** `DNS_character.yml` adds the drop and keeps `RoyalJelly`. The warning stays because the typo is compiled into the mod | 20,000 live rolls through the game's drop path: page 101 (0.505%), jelly 9,980 (49.9%) | Rolled directly, not from real kills. Starred scaling not measured |
| `Ignored portal catalog data` | PortalRules: the snapshot reaches the client while `GetServerRPC()` is null, between connect and world load. Baseline 2/2 joins, acceptance 1/2, instrumented 3/3 | Client has no snapshot for about 30 s, so public portal data is late | Observed recovery 3/3: rejected, then accepted about 30 s later with `hasSnapshot=True`, on first joins and rejoin. High | **Left.** A startup race with a working recovery | Probe timeline in `final-probe2` | Both test worlds gave `entries=0` to this character, so the mechanism is observed but a populated list is not |
| `Location client data: Value cannot be null` [Error] | Expand World Data `Apply` handles `""`, not `null`. `Location data = false` in `expand_world_data.cfg` means the value is never set, so it stays `null`. Once per join | None. It is caught, and an empty table is right with that feature off | Probe: `yaml=null`, `value=null`, `hasData=False` all session. High | **Left.** Cosmetic, upstream. Not worth a client-wide plugin update | Same line in every run, no functional loss | All five `expand_locations*.yaml` files are inactive under this setting. Turning it on could change world generation, so it stays |
| `Found null item`, `убраны пустые записи` | The `PT_BlueRunePebble` prefab is destroyed by the **second** return to the main menu, not a plain rejoin. Jotunn warns; PoV removes the entry | None seen | Probe named the slot. On the third join it is back in ObjectDB, the list and the scene, total 3,785. High for effect | **Left.** Self-healing | Three-join run in `final-probe3`, twice | What destroys the prefab was not pinned down. `recipes=0` for it in that check; not compared with a first join |
| `/api/link/inbox: Thread was being aborted` | PoV Link's 25 s long poll is cut off at game quit, directly after `Steam manager on destroy`. Only if a poll is in flight | None; the process is exiting | Log order and source. Medium: not instrumented | **Left.** Shutdown noise | Present in some quits only | Depends on the linked-account state |
| EpicLoot compatibility unavailable: UsefulTankards, TrollingFishing, SecondaryAttacks | Their binds fail on EpicLoot 0.15 | Handled by PoVPlus.Compat | Earlier feature checks. High | Expected; stays until they update | Unchanged | None |
| `toxicDeath`, `BeeQueen`, HarmonyX lookups, `DungeonDB`, `Gave up building sleep HUD`, `Feed Barrel dispensing stopped` | Previously fixed causes | None | 0 in baseline and acceptance | Fixed | Both runs, including rejoin triggers | None |
| `toxicAura` behaviour | Migrated config, new creature via `cm:spawn Greyling toxicAura 2` | Works without errors | Probe: mask 512, power 0.1, player Poison in range, on a new creature | No change | `final-probe` | Damage not isolated: melee also hit, and the 25 HP character died. The fallback for saved creatures remains upstream's claim |
| `Only custom filters`, `Missing audio clip in music respawn`, `AssetBundle valheimcuisine already loaded`, `Releasing render texture`, `Food` category, `Feaster prefab`, `Skipping world instance update`, MockManager, altbiome, `Character ID`, `Local player destroyed`, `Shared cache is busy`, shader on `ValheimArmory`, `MagicPlugin cosmetic probe` (`IceScepterNovaRing`), Crossbows category, `Unshamed` cheat bypass | Unity, load-order and mod notices | None seen | Log context only; each is low repetition. Medium | **Left.** No functional consequence shown | No exceptions follow them | Not traced to a caller individually. The Magic probe's own patch stays off for that one item |
| `[Память]`, `[Редкость]`, 475 audio lines, HDR, video shader, `AsyncResourceUpload`, `Failed to load any prefab data`, `Lux Lit Particles` | Headless server: no renderer, no audio. PoV's memory note fires once at world load | None on a server | Server-only lines. High | No change | Client logs do not show them | None |
| `final character save`, `TrinketEffects`, FSB | Window close, or intermittent | Unknown | **Merely absent** | Not fixed; did not occur | n/a | May return on a window-close exit |
| `Unknown prefab` world objects | 11 `BeeQueen`, 1 `PlanHammer` | Dormant | EVIDENCE M25 | Left in place, as instructed | Not modified | Client-hosted worlds |

## Operations

- **Effective save interval: 15 minutes**, supported by the real character store's snapshot cadence. The launch arguments reported earlier (`-saveinterval 900`) are not directly verified, because their source log was not recovered. The earlier "30 minutes" guess is withdrawn.
- **Shutdown:** use BakaLoader's stop on the real server; its log line is `World saved ... before the close request`. That path was read, not run. The same close request was verified on the disposable server: `OnApplicationQuit`, world save and store update, exit in 5 s. Players use Log Out first.
- **`baka_save` is not verified.** Over RCON it authenticated, replied empty, and did not save. Cause unknown. Do not rely on it until you see `server.saved`.
- **Effect:** a hard kill can lose up to 15 minutes of world and character together, as they roll back as a pair.
- **Real server start:** not done, on purpose. Census and backup checks used copies.

## Backups and rollback

`backups\pre-foodtimer-migration\` verified against live data: ServerManager store 33/33, Wraeclast 73/73, Clan, Homestead, 123 local world files, 5 characters, admin lists, all identical. Steam cloud differs only for disposable `povserver` and `PovTestWorld`. Also `backups\planbuild-trial\`, `backups\pre-deploy-20261030\`, `backups\server-pre-sync\`.

**PoVPlus.Compat only** (no save impact, server and clients stopped): delete `plugins\PoVPlus.Compat`, copy `patches\archived\standalone-superseded\PoVPlusPlus-Compat` and `SenealUI-UsefulTankards-Bridge-2.1.0` back, delete `PlanBuildRejoinCompat.dll` unless restoring PlanBuild, run `Update Server Mods.ps1`.

**FineDining save-format boundary:** the first upgraded save converts timers to a format older FineDining cannot read, so older binaries must not be loaded on converted saves. No downgrade testing or rollback preparation is planned; revisit recovery only if an upgrade actually breaks the pack. Existing backups stay in place, and normal save backups continue before updates.

## Decisions and gaps left as you set them

- **Winter:** `Pause Spoilage In Winter = false`. No seasonal mod is installed, so it is untested.
- **`toxicDeath` decision: resolved.** No further migration work is needed; a normal combat session can assess whether the new strength feels appropriate. Migrated on client and server. `levels.yml` Global and Boss, 10% each; `karma.yml` 10% and 30%, all at upstream's `0.1`. The behaviour is still a continuous aura, poison reapplied every second within body radius + 3 m; the on-death burst no longer exists. Saved creatures fall back to `0.1`. Verified on a new creature (EVIDENCE M28); damage was not isolated and the saved-creature fallback is upstream's claim. Backup: `backups\creaturemanager-pre-toxicaura\`.
- **Two-peer:** not tested: simultaneous clients, cross-peer ownership handoff, real players entering or leaving sleep. No account workaround was attempted.
- **SeekerBrood page drop:** restored in `DNS_character.yml` on client and server. Backup: `backups\dropnspawn-pre-seekerbrood\`. Server config syncs it to clients; no client file update is needed for this.
- **Real-world save migration:** not exercised on real data. Timer conversion ran only on disposable data.
- **11 saved `BeeQueen` creatures in Wraeclast.** Prefab `-806391820` matches `BeeQueen` from the OdinPlus-BeeQueen mod in Gale's cache, not this pack, and predates PlanBuild's removal. This is missing creature content, not lost building work. Two sit near the origin at (36.9, 46.8) and (109.2, 34.6). On a copy, a visit, world save and reload left all 11 in place, and every object entry matched before and after. They are invisible and harmless as things stand. Reinstalling the mod should bring them back, which I did not test. Its dead `DNS_spawnsystem.yml` entry was removed at your request; if you reinstall the mod, restore it from `backups\dns-spawnsystem-pre-beequeen\`. Vanilla can delete missing-prefab objects when a **server-side** scene reaches them, which did not happen on this dedicated test; do not host this world from a client if you want to keep them.
- **ServerManager Control+Click editor: deferred.** Absent: ServerManager has no control-key or editor code. It is optional and does not block this release. Wanted behaviour: Click connects, Alt+Click opens the local menu, Control+Click edits and persists the client's configured server; Save affects the next connection, Cancel keeps the old destination, Alt wins if both are held.
- **When EpicLoot, FLG, FineDining or an ItemDataManager-bundling mod updates:** check the log for PoVPlus.Compat warnings. ItemDataStack pins EpicLoot's prefix by IL hash, so a changed build declines, and feeding stops by contract rather than risking data.

## Item data stacking contract (ItemDataStackCompat 1.1.0, now in PoVPlus.Compat)

**Why 1.0.0 was wrong despite passing play tests.** The dead-code reasoning held, but the patch was not safe:

| Case | 1.0.0 | 1.1.0 |
|---|---|---|
| A bundled `ItemInfo` first used after startup | **Transpiler returns. Next dispense fails, barrels stop until restart** (real game) | Cannot happen, feeds normally (real game) |
| EpicLoot prefix replaced, original would run | **Removed live transpiler anyway** | Declines with a warning |
| Unrelated transpiler with the same owner string | **Removed it** | Left alone |
| EpicLoot prefix gone | Declined | Declines |
| EpicLoot absent | Not loaded, hard dependency | Not loaded |

**What it does, in `Awake`, after the four bundling mods:**

1. Requires exactly one `randyknapp.mods.epicloot` prefix: `EpicLoot.MagicItemData+ItemDrop_AutoStackItems_Patch.Prefix`, with IL SHA-256 `A040E65A…1971D3`. Otherwise it changes nothing.
2. Runs every loaded `ItemDataManager.ItemInfo` and `Vapok.Common.Managers.ItemInfo` static constructor, so none can fire later. These are the same constructors the mods trigger on first use.
3. Removes only `HandleAutostackableItems` transpilers declared on those types.

**Why removal is correct.** EpicLoot 0.15.4's prefix returns false on every path, and FineDining's two prefixes can only skip. No mod calls `AutoStackItems` directly or reverse-patches it. ItemDataManager's job there is merging its own data on ground stacks, which already never runs under EpicLoot, so removal changes no behaviour. Its other patches, including inventory stacking, are untouched.

**Supported boundary:** EpicLoot 0.15.4 exactly. Any EpicLoot rebuild that changes that prefix makes this patch decline, then FLG and FineDining stop Feed Barrels and pause ground stacking through their own contracts. That is the safe outcome, and it needs the hash re-verified. A new mod bundling ItemDataManager that loads after this patch is also outside the boundary, with the same safe stop.

**Correction:** 1.0.0's comment said the transpiler would empty `m_customData` on every merge. The code does not support that, and the claim is withdrawn.

## Persistence anomalies

**1. `save` did nothing: harness fault. No defect.** `save` from a client console sends an admin-checked RPC to the server. When actually submitted by an admin it worked: `Saving..`, new `_main.20.db2`, `server.saved` in the audit log. The failed attempts never submitted it:

- First run: the admin entry was added while the server was already up.
- Second run: my scripted input did not submit it. Two ways reproduced: an 80 ms Enter left `save` in the input box while a 200 ms press worked, and the join-time Server Rules popup takes Enter. Which one applied that run cannot be told from the logs.

Neither failed run has any `server.saved` event, so the server never received a completed save. No save was queued, redirected or superseded.

**2. Character "restored": ServerManager, expected.** `loadServerCharacterOnJoin: true` makes `ServerManager\characters\<steamid>\` authoritative on the **real server too**. On join the stored character replaces the local and cloud copy, even when the local copy is newer. The store is committed with world saves. Reproduced on purpose: after a confirmed save (A), changes (B), a forced stop and a rejoin returned exactly A. The newer local file holding B was ignored, and the world rolled back to the same point.

**Effect on food timers:** none harmful. Character and world roll back as a pair, so absolute expiry values and the clock stay consistent, with no FineDining history warning. By code only, not reproduced: a restored character in the **same world with a different clock epoch** keeps its timers frozen and warns rather than resetting. That happens if you restore a character without its matching world. In a **different world**, timers carry over as remaining time.

**Operating rules:**

- Stop a server only after `server.saved` or a graceful shutdown (see Operations). Anything since the last save is lost for world and character together.
- Players should use **Log Out**. Closing the window logged `The connection ended before the final character save was acknowledged`; Log Out did not.
- Restore characters with `sm:characterrestore`, per `ServerManager\characters\HowToRestore.txt`.


## Conflicts, by evidence tier

| # | Finding | Static | Harness | Real client | Server connection | Two-peer |
|---|---|---|---|---|---|---|
| F1 | EpicLoot replaces `AutoStackItems` | Traced | Rounds 1–3 | **Boar fed from barrel, A/B; merges A/B** | **Barrel fed on dedicated server** | None |
| F2 | `AddHealthRegen` lost under FineDining | Traced | Round 3 | **1.0867 → 4.0867 with real helmet, A/B** | Not repeated | n/a |
| F3 | Rod and tankard Use dead | Traced | Gate only | **Full E matrix, A/B** | **Tankard E on server** | None |
| F4 | Server gates mismatched mods | Corrected | None | n/a | **Refusal and join both observed** | None |
| F5 | Seekurai upgrade typo | Traced | Data | **Black Metal 80→80 vs 80→70, A/B** | Not repeated | n/a |
| F6 | Lightning trap effect name | Traced | Value | **Lightning only vs Tared + Lightning, A/B** | Not repeated | n/a |
| H1 | SecondaryAttacks × EpicLoot burst | Traced | Residue | **Real defect, fixed, A then B** | Not repeated | None |
| H2 | Crossbow mastery doubling | Traced | Rejected | Not tested | n/a | n/a |

## What changed in play

- **Rod and tankard:** `E` opens and closes. `Alt+E` opens shard slots on a socketed tankard or rod. Plain `E` closes an open overlay. Other socketed items keep plain `E`.
- **Rod bag** now docks under the inventory like tankards, so the grid no longer shifts.
- **Regen:** EpicLoot `AddHealthRegen` gear now works.
- **Burst crossbows:** a `TripleBowShot` proc now affects the first shot only.
- **Lightning traps** apply tar. **Seekurai Wings** upgrades cost Black Metal.

## Corrections to previous conclusions

| Previous claim | Now | Why |
|---|---|---|
| "11 mods should be refused" by ServerSync | The first gate is ServerManager's **SHA-256 allow-list** | Observed `validation.hash_not_allowed`. Any changed DLL must be in `required` |
| F4 server is stale | Not stale at this session's start | Update log at 14:48:28 predates my first action. Mismatch was recreated with FLG 1.1.10 |
| Keep SeneaL hover override | **Removed: it was a bug** | `HoverCell` goes stale after a toggle; vanilla `GetItem` stayed right. Third press closed the inventory |
| Bridge 2.0.0 candidate | Superseded | It made socket overlays unreachable on socketed tankards |
| Rod bag layout untested | Real defect, fixed by docking | Undocked bag shifted the grid so the next press hit another item |
| Feed Barrel needs only the guard fixed | Also needs a **player-placed** barrel | FLG requires `s_creator`; all earlier harness barrels could never dispense |
| Ground merging is routine | Only above **200 loaded drops** | Vanilla `SlowUpdate` gate; F1 merge effects are rare in normal play |
| H1 unresolved | **Confirmed and patched** | Nine projectiles from three bolts on proc |
| 0.5 multiplier in harness | Consistent with Cold | Real tick halved to 0.5434 while Cold |
| Earlier A-run barrel `stock=0` | Artifact | Unsupported barrel collapsed and spilled |

## Reproducing

`tools\finalrun.ps1` is the clean cycle (`-Stage setup`, then `-Stage play`), with `tools\memlog.ps1` for memory. `tools\missingprefab.ps1` repeats the copied-world visit. `tools\finalstop.ps1` is the shutdown half used for the interrupted run. `tools\compatsetup.ps1` installs the build with test probe and test mode (`-NoTest` for clean). `tools\compatrun.ps1`, `compatpart2-4.ps1` repeat the matrix. `tools\snapshot.ps1` runs the census on a copied world. `tools\opstest.ps1` repeats the graceful stop. Pre-consolidation harnesses are in `tools\archived\`.
