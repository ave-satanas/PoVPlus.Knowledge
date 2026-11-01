---
id: OPS-backups
type: operation
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [DEC-finedining-rollback, OPS-saves-shutdown]
---
# Backups

**Before any update:** make an ordinary save backup of the production save root and character store, with server and clients stopped.

All under `backups\` in the workspace. `[checked]` folders exist. Leave them untouched.

| Folder | Holds |
|---|---|
| `pre-foodtimer-migration` | World, ServerManager store, local worlds and characters, admin lists, binaries. Verified against live in M19 |
| `pre-deploy-20261030`, `server-pre-sync` | Earlier deployment state |
| `planbuild-trial` | Blueprint index and config copy |
| `dns-spawnsystem-pre-beequeen` | Spawn entry |
| `creaturemanager-pre-toxicaura` | `levels.yml`, `karma.yml` |
| `dropnspawn-pre-seekerbrood` | `DNS_character.yml` |
| `flg-swap` | One DLL from the FLG change |

The game's own world backup settings depend on launch arguments that are not directly verified. See [[OPS-saves-shutdown]].

Removing PoVPlus.Compat has no save impact. Steps: `REPORT.md`, "Backups and rollback".
