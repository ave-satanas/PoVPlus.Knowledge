> **Sanitized snapshot, not raw evidence.** Generated from `PROGRESS.md`. See `README.md` in this bundle.

# Progress log

Resume point for interrupted work. Newest at the bottom.

## 2026-10-30 — Specimen and first investigation

- Workspace was empty; not a git repo; no commits by instruction. `EnterWorktree` failed (no repo), so work is in place.
- `.tools/` = ilspycmd 11.2 (workspace only). `.cache/decomp/` = decompiled output, never a deliverable.
- Client: Valheim 1.0.21, Unity 6000.0.75, BepInEx 5.4.23.6, pack 5.4.2360. Live profile treated read-only.
- `PoVPlus.txt` is stale (26 version drifts). Use `evidence/specimen/inventory.*`.
- Last load evidence = `BepInEx/LogOutput.log` 10/29 09:52-09:58. Since then 5 DLLs changed (ConfigManager, FeedLikeGrandma, Unshamed, MyDirtyHoe, ValheimCuisine) and the two local patches (Tome Slot, Tankards Bridge) left `plugins/`.
- Prior work in `S:\Valheim-Modding` is Tome-slot and workflow-safety heavy. `INV-002`/`COMP-001` triage is on older versions, and never recorded the Feed Barrel error.
- D1 Feed Barrel: mechanism traced statically. EpicLoot `AutoStackItems` prefix always returns false, so FLG/FineDining/ItemDataManager transpilers are dead; FLG guard identical 1.1.10 → 1.2.0. Next: runtime A/B/A in isolation.

## 2026-10-30 — Finished

- AppContainer isolation proven; 12 valid full-pack server runs complete; evidence in evidence/runtime.
- F1, F2 patched and A/B/A proven server-side. F3 bridge built, binding load only. F5, F6 config proven. F4 needs a redeploy by the user.
- Remaining: client and multiplayer checks in REPORT.md. Nothing installed.

## 2026-10-30 — Round 2-4

- Client blocked: SteamAPI_Init fails inside the AppContainer. No Steam bypass, no run outside the boundary.
- Round 2 exposed a regression in FeedBarrelStackCompat 1.0.0; 1.1.0 fixes it. Round 3 is the best evidence.
- Round 4 stopped by system memory pressure, no data, not restarted. It targets AI dispensing, F5 and F6.
- Remaining: tools\run_round4.ps1, then client and multiplayer checks in REPORT.md.

## 2026-10-30 — Real client deployment

- Authorized by the user. Live profile patched, server synced with the user's script, real client and dedicated server tested.
- Bridges corrected from real input: tankard 2.1.0, rod 1.3.0. BurstMultiShotCompat 1.0.0 added after H1 was observed.
- Remaining: two-peer test. See REPORT.md.

## 2026-10-30 — Food timer migration

- FLG 1.2.1 and FineDining 1.1.12 live on client and server. FeedBarrelStackCompat retired, ItemDataStackCompat added, HealthRegenCompat retained.
- Backup before any upgraded run: `backups\pre-foodtimer-migration\`. Real worlds not yet run on new code.
- Probe removed, deployment re-synced, final smoke passed. Game and server stopped.
- Remaining: two-peer test, winter if a seasonal mod is ever added, first real server start.

## 2026-10-30 — Warnings, ItemDataStackCompat, persistence

- ItemDataStackCompat 1.0.0 failed review; 1.1.0 deployed and regression-tested. Upstream draft unsent.
- Both persistence anomalies explained. Container stacking complete. Warning ledger in REPORT.md.
- Remaining: two-peer test, first real server start, toxicDeath balance decision.

## 2026-10-30 — BetterSleep HUD bridge

- BetterSleepHudBridge 1.0.0 built, validated with one peer and simulated sleepers, deployed. Test mode off, probe removed.
- Single-client matrix complete, including boat, oven, mead, pickable and bed. Two-peer testing is externally blocked: one Steam account.
- Remaining: first real server start, toxicDeath balance decision.

## 2026-10-31 — Log warnings

- Every HarmonyX line traced to a caller. QuietLookups and PlanBuildRejoinCompat deployed; PlanBuild's rejoin exception fixed. Three HarmonyX warnings stay visible: SeneaL UI's dead EpicLoot map toggles.
- Open: SeneaL map chips, strip overlap with SeneaL's minimap element off, wave clock, first real server start, toxicDeath balance decision.

## 2026-10-31 — Consolidation

- PoVPlus.Compat 1.0.0 deployed everywhere, `51956F65…`. PlanBuild removed. Standalones archived. Game and server stopped.
- Clean join, Log Out, rejoin, Log Out passed on the release DLL. Release contents verified. PlanHammer and BeeQueen traced on a copy, both dormant.
- Open: `baka_save`, first real server start, Hexium upload, Control+Click (deferred), two-peer (externally blocked).

## 2026-10-31 — Final warning pass

- No production change; PoVPlus.Compat 1.0.0 retained. SeekerBrood page drop restored by config. Final clean acceptance passed. Game and server stopped.
- Open: `baka_save`, first real server start, Hexium upload of 1.0.1, two-peer (externally blocked).

## 2026-11-01 — Cooking conversions

- FineDining check ran. ValheimCuisine's rebuild was removing 11 conversions; fixed in PoVPlus.Compat 1.0.1, deployed, clean acceptance passed. Older-vault comparison corrected.
