---
id: CMP-quiet-lookups
type: compat
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-index, INV-harmonyx-attribution, DIAG-warning-ledger]
applies_to:
  - HarmonyX (BepInExPack 5.4.2360)
  - Path of Valheiman 5.3.6
  - SeneaL UI 1.2.8
  - OdinHorse 1.9.4
  - BowsBeforeHoes 2.0.2
---
# QuietLookups

Source: [QuietLookups.cs](../../patches/PoVPlus.Compat/QuietLookups.cs).

**Problem:** mods probe optional targets through `AccessTools`, which warns on every miss.

**What it does:** prefixes `AccessTools.TypeByName`, `Method`, `DeclaredMethod`, `Field` and `Property`. Only exact listed names are affected. A listed name that exists still resolves, and everything else logs as before. It never fabricates a success. Log levels are untouched.

**Why it is safe:** each entry had a traced caller and verdict. See [[INV-harmonyx-attribution]].

**Init and Harmony:** owner `0.povplusplus.quietlookups`. Runs in `Awake`, first in the pack, so it precedes other mods' `Awake` probes. It has no dependency check.

**Correction kept:** a first version broke ConfigManager's no-argument `ZInput.GetJoyRightStickY`, logged as a `Patching exception`. The rule now matches only the `(bool)` signature. `[runtime]` M14.

**Limits:** the list is static, so a new probe warns until reviewed. That is intended.

**Revalidate when:** a caller mod updates, or new HarmonyX warnings appear. Do not add a name without tracing its caller and confirming a safe fallback.
