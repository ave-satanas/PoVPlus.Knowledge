---
id: DIAG-logging
type: diagnostic
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [DIAG-warning-ledger, SYS-compat-architecture, OPS-test-tools]
applies_to:
  - LoadTimeProfiler 1.3.2
---
# Logging

**Primary log:** `BepInEx\LogOutput.log` in the profile and in the server instance. `BepInEx.cfg` allows Fatal, Error, Warning, Message and Info.

## Filtering

**LoadTimeProfiler filters mod loggers.** Its config section `[Logging.Mods]` says unlisted plugins use `Errors, Warnings`, and there are no listed plugins in this profile. `LogFiltering.ManualLogPrefix` applies it. `[checked]`

So PoVPlus.Compat's Info and Message lines, including its install summary, **never reach `LogOutput.log`**, and neither do most mods' Info lines.

**A missing Info or Message line is not evidence that initialization failed.** Unity logs and unattributed loggers pass through, and warnings and errors from all mods remain visible.

This explains why the bridge's Info lines did not appear in this workspace. The cause was found independently, with a working override, on 2026-10-24 in the development install. See [[HIST-early-diagnostics]].

## Alternate sources

- To see a mod's Info lines, add `plugin.guid = Errors, Warnings, Information` under `[Logging.Mods]`, for a test only, then restore it.
- Test probes write their own log. See [[OPS-test-tools]].
- Load timing: `BepInEx\config\LoadTimeProfiler\*.log`.
- Dedicated server output for test runs: `.sandbox\srv-saves\server.log`.
- ServerManager: `ServerManager\logs\events-audit.log`, where `server.saved` is the save confirmation.
