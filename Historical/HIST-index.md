---
id: HIST-index
type: index
status: active
created: 2026-11-01
updated: 2026-11-01
related: [SYS-environments, INDEX]
---
# Historical research

Unique research imported from older vaults. **Nothing here describes the current deployment.** For that, use [[CURRENT-STATE]]. These summaries were read from the sources on 2026-11-01. Their findings were not retested.

## Sources

Machine-local, in `S:\Valheim-Modding\`. These remain in place, unmodified.

| Vault | Branch, commit | Records |
|---|---|---|
| `PoV-Plus\Knowledge` | `research/dev-008-supervised-testing`, `a08a373` | 127, the base used here |
| `PoV-Plus-Claude\Knowledge` | `claude/review-c1`, `d1a1761` | 125 |
| `PoV-Plus-Runtime-Integration\Knowledge` | `integration/runtime-tier-a`, `d1a1761` | 125 |

**Environment:** these vaults concern the development installation `PoVPlus-Dev`, plugin `povplus.core` 0.1.0, character `PoVPlus_Test`, world `PoVPlus_Lab`. Dates run 2026-10-24 to 10-29. Pack versions differ from the live profile.

## Comparison, 2026-11-01

`[checked]` Raw bytes compared separately from text normalized for CRLF and LF. `core.autocrlf=true` in main and Claude. All three working trees are clean in git, so the differences are line endings as checked out, plus the substantive ones below. 128, 126 and 126 files, of which 127, 125 and 125 are Markdown.

| Pair | Common | Raw identical | Identical after normalizing | Substantive | Only left | Only right |
|---|---|---|---|---|---|---|
| Main, Claude | 125 | 107 | 117 | 8 | 3 | 1 |
| Claude, Runtime Integration | 126 | 1 | 126 | 0 | 0 | 0 |
| Main, Runtime Integration | 125 | 9 | 117 | 8 | 3 | 1 |

**Correction:** an earlier note here said all 128 records differ. That compared raw bytes across all three at once, so line endings dominated. Claude and Runtime Integration hold the same content.

**Substantive, main against Claude:** `DEC-003`, `DEV-003`, `DEV-005`, `DEV-006`, `DEV-007`, `EXP-005`, `INDEX`, and `environment-snapshot.json`, whose only changed key is `repository_sources`. All text changes concern DEV-009 or cross-worktree notes, and all but `DEV-003`, `DEV-007` and `INDEX` only add lines.

**Main only:** `DEV-008`, `EVID-002`, `EXP-006`. **Claude only:** `DEV-009`.

## Coverage

These records are **summaries with links, not a complete merge.** Read directly: the eight diffs above, the four unique records in part, and ARCH, DEV-004, DEC-003, DEC-004, COMP-001, COMP-002, INV-001, EXP-001, EXP-003, and INV-003's conclusions. The rest of INV-003, DEV-005 to DEV-007, EXP-004, EXP-005 and ARCH-003 were not read in full.

## Imported

| Record | Holds |
|---|---|
| [[HIST-project-philosophy]] | Project identity, priorities, intervention classes |
| [[HIST-tome-compatibility]] | Magic book and Tome slot research |
| [[HIST-dev-workflow]] | Development workflow and safety |
| [[HIST-supervised-testing]] | Isolation assessment, main branch only |
| [[HIST-early-diagnostics]] | Early logging and warning register |

## Not imported

- About 118 `MOD-*` pages: mostly metadata snapshots from older versions, marked partial. Current inventory: [[MOD-inventory]]. Read originals only for a mod's older history.
- `MOD-000-Inventory`, `DEP-001-Environment`, `CFG-001`: older environment state.
- Session logs and `EVID-*` ledgers: read the source if a claim needs provenance.
