---
id: OPS-evidence-map
type: operation
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CMP-coverage, DIAG-warning-ledger, INV-index]
---
# Evidence map

Source records: [EVIDENCE.md](../../EVIDENCE.md), files under `evidence\`. Reports summarize; these are closer to the observation. Evidence ids below are headings or `M` entries there. File counts `[checked]`.

| Id | Topic | Files |
|---|---|---|
| F1–F6 | Original conflicts | `evidence\client\`, 102 files; `runtime`, `round2`, `round3` |
| H1, H2 | Burst, crossbow mastery | `evidence\client\` |
| M0–M8 | Migration | `evidence\migration\` |
| M9 | Persistence | `evidence\warnings\` |
| M10 | Late constructor | `evidence\ids\` |
| M11–M14 | Sleep, lookups | `evidence\warnings\` |
| M15 | PlanBuild | `evidence\warnings\`, `evidence\ops\census-*` |
| M16–M17 | Consolidated matrix, waves | `evidence\compat\` |
| M18 | Shutdown | `evidence\ops\graceful.txt` |
| M21–M22 | First clean rejoin | `evidence\final\` |
| M24–M25 | Saves, missing prefabs | `evidence\ops\` |
| M28 | Last warning pass | `evidence\baseline\`, `evidence\acceptance\`, `evidence\warnings\final-probe*` |
| M29 | Reconciliation and location census | `evidence\ops\census-locations.txt` |
| M30, M31 | FineDining before and after 1.0.1 | `evidence\fdcheck\` |
| M32 | 1.0.1 acceptance | `evidence\acceptance-1.0.1\` |
| M33 | Older-vault comparison | Commands in `EVIDENCE.md` |

`round4` is empty and superseded.

**Spot-checked for this vault on 2026-11-01:** M28 acceptance `final.txt`, `evidence\ops\census-locations.txt`, `patches\dist\SHA256SUMS.txt`, `evidence\migration\parity-final.tsv`. Other entries are imported, not rechecked.
