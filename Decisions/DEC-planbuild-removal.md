---
id: DEC-planbuild-removal
type: decision
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [INV-planbuild, MOD-planbuild, INV-missing-prefabs]
---
# PlanBuild removal

**Decision:** `[decision]` Removed from client, server and allow-list as a simplification. The user accepted the outcome.

**Lost:** planned ghost building and blueprint capture and placement. Dirty Hoe replaces terrain tools only, so it does not cover these.

**Kept:** 33 `.blueprint` files, plus 28 `.png` and 2 `.vbuild`, at machine-local `G:\STEAM\steamapps\common\Valheim\BepInEx\config\PlanBuild\blueprints`. `[checked]` Mod archive: `patches\archived\PlanBuild-removed\`. Trial data: `backups\planbuild-trial\`.

**If restored:** the rejoin exception returns unless PlanBuildRejoinCompat comes back too, and clients and allow-list must change.
