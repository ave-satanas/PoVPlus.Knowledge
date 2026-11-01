---
id: INV-planbuild
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
related: [DEC-planbuild-removal, MOD-planbuild, INV-missing-prefabs, CMP-retired]
---
# PlanBuild rejoin defect

**Symptom:** a `NullReferenceException` in `DungeonDB.Start` on every rejoin. Stack: `PlanDB.WarnDuplicatesWithDifferentResources` reading destroyed `Piece` objects. DataForge's postfix was skipped, while Expand World Data's ran. `[runtime]` M14.

**Hypothesis tested:** that the blueprints caused it. **Rejected.** `[runtime]` M15

| Case | Setup | Result |
|---|---|---|
| A | Blueprints, no compat | NRE, DataForge skipped, 12 dead keys |
| B | Empty library, 33 files moved and restored | Identical to A |
| C | PlanBuild removed | 0 exceptions, both postfixes ran |

**After removal, real rejoin:** recipes 1991, hammer 718, hoe 7, items 3785, same hash. Native craft Club 0→1, placement `wood_floor` 148→149. `[runtime]` M16 These are registration, one craft and one placement, not all DataForge behaviour. This pack has no active DataForge overrides. `[runtime]`

**Interim fix retired:** PlanBuildRejoinCompat was deployed, then removed with PlanBuild. Without it, the exception returns if PlanBuild is reinstalled.

Evidence: `evidence\warnings\callers-plan{A,B,C}`, `log-plan*`.
