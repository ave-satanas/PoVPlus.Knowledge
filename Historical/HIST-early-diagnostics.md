---
id: HIST-early-diagnostics
type: historical
status: historical
created: 2026-11-01
updated: 2026-11-01
related: [HIST-index, DIAG-logging, DIAG-warning-ledger]
specimen: PoVPlus-Dev, session 2026-10-24
---
# Early diagnostics

From `INV-001-Startup-message`, `EXP-001-Logging-override`, `DIAG-001-Logging`, `COMP-001-Warnings`, `INV-002-Compatibility-triage`.

**Missing startup line:** `povplus.core`'s Info line was absent on 2026-10-24. IL review showed the call existed. With `povplus.core = Errors, Warnings, Information` under `[Logging.Mods]`, the exact line appeared at the unchanged binary. `[runtime]` in the development install. This is the working example of the override in [[DIAG-logging]].

**Warning register:** 31 unique signatures at 2026-10-24, on older versions such as PoV 5.3.2 and EpicLoot 0.15.1. They are superseded by [[DIAG-warning-ledger]]. Its principle remains: a warning is an observation, not an automatic defect.

The note `FineDining food cards: PassiveTree.UI.ItemTooltip.owners missing` was recorded there and is absent from the current ledger. Whether it still applies was not checked. `[unknown]`
