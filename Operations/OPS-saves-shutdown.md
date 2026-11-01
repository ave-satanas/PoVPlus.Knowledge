---
id: OPS-saves-shutdown
type: operation
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [INV-persistence, DEC-production-session-hold, OPS-backups]
---
# Saves and shutdown

## Save interval

**15 minutes, supported by observed snapshots.** The real character store's snapshots are 15 minutes apart, for example `03:53:59`, `04:08:59`, `04:23:59`. `[checked]`

**Launch arguments are not directly verified.** A session on 2026-10-31 reported `-saveinterval 900 -backups 4 -backupshort 7200 -backuplong 43200` from a BakaLoader server log, but that log was not re-located on 2026-11-01. Treat those values as `[unknown]` until the source is recovered.

`cron.yml` jobs are empty, so autosave is the only schedule.

## Authoritative character store

`loadServerCharacterOnJoin: true` in `ServerManager.yml`. `[checked]` `ServerManager\characters\<id>\` under the production save root wins over local and cloud copies on join, even if they are newer. It commits with world saves, and character and world roll back as a pair. `[runtime]` M9. Restore with `sm:characterrestore`, per `HowToRestore.txt`.

## Shutdown

- Players use **Log Out**. Closing the window can log `The connection ended before the final character save was acknowledged`.
- Real server: BakaLoader's stop. A real log line reads `World saved ... before the close request`, so it saves first. Not run by an agent.
- Verified on the disposable server: `CloseMainWindow` triggers `OnApplicationQuit`, a world save and store update, with exit in 5 s. `[runtime]` M18
- Ctrl-C did nothing to a hidden test server.
- **Do not rely on `baka_save`.** Over RCON, it authenticated, replied empty and did not save. `[runtime]` M18 Cause unknown. Treat an empty reply as failure.
- Confirm saves with `server.saved` in `events-audit.log`.

A hard kill can lose up to 15 minutes of world and character together.

## Launch

Real launches go through the BakaLoader app. Its profile store and the full command line were not found on 2026-11-01. `[unknown]` A session on 2026-10-31 reported arguments including `-world Wraeclast -public 0 -resetmodifiers -modifier resources more`, plus `-setkey` entries, from the same unrecovered log. `start_headless_server.bat` in the instance is the stock template, not the real launch.
