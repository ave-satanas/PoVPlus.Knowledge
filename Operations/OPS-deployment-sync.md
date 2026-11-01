---
id: OPS-deployment-sync
type: operation
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [OPS-build-release, SYS-environments, CURRENT-STATE, INV-config-reconciliation]
---
# Deployment and sync

## Sync

With the game closed, run machine-local `%APPDATA%\com.kesomannen.gale\valheim\profiles\PoVPlus\Update Server Mods.ps1`. It copies the profile's `BepInEx` to the server instance and to ServerManager's `required`, with robocopy `/MIR`, excluding the four BakaLoader tools. `[checked]` It fails on locked cache files if the client is running. `[runtime]` F4

`tools\compatsetup.ps1 -NoTest` wraps install and sync for this workspace. It also starts a disposable server and joins, so do not use it for a plain sync.

Config on the server is shared by real and disposable runs on the same instance. Test mode must be off afterwards; a later sync restores it.

## Allow-list

ServerManager's first gate is SHA-256 against `required`. A mismatch logs `validation.hash_not_allowed`. An empty list refuses all mods. A character with local world history can be refused by character protection. `[runtime]` F4

## Parity check

Compare hashes across all three locations, not counts. Compare **all** config files, not only `.cfg`. See [[INV-config-reconciliation]].

## External clients

Each must delete `PoVPlusPlus-Compat` and `SenealUI-UsefulTankards-Bridge-*`, remove PlanBuild, install the release to one folder, and match the pack, including FLG 1.2.1, FineDining 1.1.12 and OdinHorse 1.9.4.

Installing through a mod manager changes the folder name, so remove `plugins\PoVPlus.Compat` first or the plugin loads twice, then sync. Server-held DropNSpawn config reaches clients by its own sync. `[runtime]` log line `Loaded 1 synchronized character configuration`.
