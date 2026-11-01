---
id: SCHEMA
type: schema
status: active
created: 2026-11-01
updated: 2026-11-01
related: [INDEX]
---
# Schema

Plain Markdown with YAML frontmatter and Obsidian wikilinks. No plugins, database or service. Open `Knowledge/` as a vault, or read it in any editor.

This is project documentation that session instructions point to. It is not automatic memory: a session only benefits if it reads it, and it is only right if sessions keep it updated.

## Frontmatter

```yaml
id: CMP-example          # required, unique, equals the filename
type: compat             # required, see below
status: active           # required, see below
created: 2026-11-01      # required, ISO date, America/Los_Angeles
updated: 2026-11-01      # required
last_verified: 2026-11-01  # only when this note's facts were checked against source that day
related: [INV-example]   # record ids
supersedes: [INV-old]    # optional
superseded_by: INV-new   # optional
applies_to:              # optional, versions that drive update review
  - EpicLoot 0.15.4
specimen: live-profile   # optional, see SYS-environments
```

Types: `index`, `state`, `schema`, `system`, `mod`, `compat`, `investigation`, `decision`, `operation`, `diagnostic`, `session`, `inventory`, `historical`.

Statuses: `active`, `open`, `resolved`, `bounded`, `retired`, `superseded`, `deferred`, `blocked`, `historical`. `bounded` means documented with known limits, not fixed.

Id prefixes: `SYS`, `MOD`, `CMP`, `INV`, `DEC`, `OPS`, `DIAG`, `SES`, `HIST`.

`last_verified` never means a runtime test ran that day. Do not set it because a test was imported. Runtime dates stay in the evidence they came from.

## Evidence tags

Put a tag beside any claim in a note that mixes evidence. Never put one label over partly tested content.

| Tag | Meaning |
|---|---|
| `[checked]` | Files or source read directly, on the date in `last_verified` |
| `[runtime]` | Imported runtime observation. The evidence id says when and how |
| `[code]` | Concluded from reading code, not observed running |
| `[upstream]` | A mod author's statement, not independently tested |
| `[hypothesis]` | Unproven |
| `[decision]` | The user chose this |
| `[unknown]` | Accepted gap |

Runtime evidence also carries its tier from `EVIDENCE.md`: **S** static, **H** server harness, **C** real client, **SV** real client on a dedicated server, **2P** two peers (none exist).

## Repository

`Knowledge/` is its own private git repository. Commit and push vault changes there. Relative links out of the vault, such as `../../patches/...`, and `evidence\...` paths resolve only in the local workspace, not on GitHub. `EvidenceBundle/README.md` explains the layout. After report or release changes, follow [[OPS-build-release#Release maintenance]].

## Maintenance rules

- One canonical home per fact. Link to it rather than repeating it.
- Never silently delete a wrong finding. Record the correction, or mark the note `superseded` and link the replacement.
- Keep raw logs, DLLs, saves, credentials and personal identifiers out of this vault. Cite `evidence\…` paths.
- Label machine-local paths as such. They are not vault links.
- `EVIDENCE.md` and `evidence\` remain the source records. This vault indexes and interprets them.
- `historical` records are scoped to the environment in their `specimen`. Never merge their deployment state into [[CURRENT-STATE]].
- Run `python Knowledge/tools/validate.py` after changes. It checks structure only, and cannot detect contradictions or prove compatibility.
