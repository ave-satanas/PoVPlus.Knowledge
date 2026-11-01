---
id: SYS-compat-architecture
type: system
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, OPS-build-release, DEC-production-builds, DIAG-logging]
applies_to:
  - BepInEx 5.4.2360
  - Valheim 1.0.21
---
# PoVPlus.Compat architecture

All of this was read from [the project](https://github.com/ave-satanas/PoVPlus.Compat/tree/eed8d74b3700a044eb9dbce7053fe0ec2018ca69) on 2026-11-01. `[checked]`

## Build

| Setting | Value |
|---|---|
| Target framework | `net48` |
| Language version | C# 7.3 |
| Output | `PoVPlus.Compat.dll`, no target-framework folder |
| `AutomaticallyUseReferenceAssemblyPackages` | false |
| Assembly references, `Private=false` | BepInEx, 0Harmony, EpicLoot, SeneaL-API, assembly_valheim, assembly_utils, UnityEngine, CoreModule, InputLegacyModule, UI, UIModule |
| Mismatch override | `ResolveAssemblyReferenceIgnoreTargetFrameworkAttributeVersionMismatch=true`, needed for EpicLoot's reference |

References resolve from the live profile and `G:\STEAM\steamapps\common\Valheim\valheim_Data\Managed`, so a build tracks whatever is installed there.

## Identity

GUID `0.povplus.compat`, name `PoVPlus.Compat`, version `1.0.1`, one `BepInPlugin`, no `BepInDependency` entries. There is no config file and no saved data.

## Initialization

1. BepInEx sorts plugins by GUID with dependencies hoisted. The `0.` prefix and lack of dependencies put this plugin first.
2. `Awake` installs [[CMP-quiet-lookups]], because other mods probe during their own `Awake`, and subscribes to `AppDomain.AssemblyLoad`.
3. When FeedLikeGrandma or FineDining loads, the handler prefixes their `Start` with `RunDeferred`. Their contract checks live in `Start`, so the fixes must run first.
4. `RunDeferred` runs once and installs the remaining modules in this order: item data stacks, health regen, rod bag, tankards, burst multishot, sleep hud, map chips, cuisine conversions.
5. The plugin's own `Start` is the fallback if neither hook fired.
6. `Update` calls `SleepHud.Tick()`.

If neither target has `Start`, a warning says the contract check may run first.

## Failure model

`Install(name, needs, body)` checks required GUIDs and wraps each module in try/catch. A missing dependency logs one warning and skips that module. An exception logs one error. The others continue. Each module keeps its own Harmony owner so existing guards and unpatch logic still match.

`RunDeferred` logs a summary at Message level, which the log filter hides. See [[DIAG-logging]].
