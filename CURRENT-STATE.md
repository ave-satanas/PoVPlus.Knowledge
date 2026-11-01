---
id: CURRENT-STATE
type: state
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [SYS-environments, OPS-build-release, OPS-remaining-work, MOD-inventory]
---
# Current state

Observed **2026-11-01, after the Gale profile acceptance run**. Evidence tags: [[SCHEMA#Evidence tags]].

## Directly checked in that snapshot

These were read from files, without launching the game. `[checked]`

- `valheim` and `valheim_server` are not running.
- Live client plugins hold 88 DLLs, the server instance 92, and the ServerManager `required` folder 88.
- Client and `required` have 0 hash differences. Client and server differ only by four server-only tools: `BakaLoaderCommander.dll`, `BakaLoaderItemIndexer.dll`, `BakaKillAll.dll`, `BakaLoaderSpawnHelper.dll`.
- `PoVPlus.Compat.dll` 1.0.1 is `8B766887…75EB` in all three locations, in folder `PoVPlus_Compat`. Full hashes: [[OPS-build-release#Canonical artifacts]].
- Gale holds 88 package records, including PoVPlus.Compat as a local import. Three upstream updates are deliberately not taken. See [[OPS-gale-profile]].
- 86 client and 90 server plugin entries, all GUIDs unique on each side.
- No `ClientProbe`, `FdServerProbe`, `IdsProbe`, `CallerTrace` or `WorldCensus` files in any of the three plugin locations.
- `Enable Testing Mode = false` and `Pause Spoilage In Winter = false` on client and server.
- Four `toxicAura` keys and no `toxicDeath` keys on each side. `SeekerBrood` override present on each side. BeeQueen spawn entries 0.
- `Location data = false` on the client config.
- All 274 frozen config files match the live client.

## Runtime

- Clean acceptance of the corrected profile passed on 2026-11-01: join, `save`, Log Out, rejoin, Log Out, quit, graceful close. `[runtime]` M37.
- That run used the disposable world `PovSrvTest`, never the production world.

## Not established

- **The production world `Wraeclast` has not run on this code.** Its newest save is dated 2026-10-26 11:44. `[checked]`
- The first production session has not run. A task boundary paused it, and it is not forbidden. See [[DEC-production-session-hold]].
- A full cook of the restored foods has not been run. See [[INV-cooking-conversions]].

## Plain summary

PoVPlus.Compat 1.0.1 is deployed and packaged but not uploaded. PlanBuild is removed. The Gale profile matches the tested pack. Next work: [[OPS-remaining-work]].
