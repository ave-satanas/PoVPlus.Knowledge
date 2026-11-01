# PoVPlus knowledge base

Engineering records and an audit package for PoVPlus.Compat. **PoVPlus** is a personal Valheim mod pack of 87 mods built around Path of Valheiman. **PoVPlus.Compat** is a custom BepInEx plugin that fixes specific conflicts in that pack.

This repository holds records, not the plugin. It does not show that the whole pack is compatible.

**Reviewing this work? Start with [AUDIT.md](AUDIT.md).** Agents should also read [AGENTS.md](AGENTS.md).

## What the evidence concerns

| Item | Value | How it is known |
|---|---|---|
| Release | PoVPlus.Compat 1.0.1 | Tested and deployed. Not uploaded anywhere |
| Tested DLL SHA-256 | `8b766887e53a9fd285d68df4150fe153c8e3831ba07e57ffab0bf4ca487a75eb` | Hashed from deployed files. Exact bytes in [`EvidenceBundle/release/`](EvidenceBundle/release/PoVPlus.Compat.dll) |
| Release zip SHA-256 | `77419c0891888222c794938e2710d172107b5c187a9d91adf4c7b6d2fd2159c1` | [`EvidenceBundle/release/SHA256SUMS.txt`](EvidenceBundle/release/SHA256SUMS.txt) |
| Source commit | [`eed8d74`](https://github.com/ave-satanas/PoVPlus.Compat/tree/eed8d74b3700a044eb9dbce7053fe0ec2018ca69) | See below |
| Managed image digest of tested DLL | `F2B5F7F0ECC30338D23C091EBFC1ADC4106119FC8470324C4154F4F02016CC29`, 124 method bodies, 37 types | From `tools/il-digest.ps1` on `main` of the source repository |
| IL-only digest, limited | `1DC63390417AF3FA3B0626A8DBE6BFDA221EE0CB70BB65AAC29195374B20DF1D` | Instructions only. A changed string does not change it |

**Source relationship:** the DLL was built from the working tree on 2026-11-01, and the git repository was created afterwards. Rebuilds of `eed8d74` match the release's managed image: IL, exception handlers, strings, signatures, attributes and references, excluding only the module ID (M39). **The rebuild is not byte-identical.** With paths aligned, 72 of 42,496 bytes still differ: the timestamp, module ID and debug data. Why those derived values differ is `[unknown]`. The match rests on the same referenced assemblies and SDK on the original machine. An earlier version of this page said the link was proven by IL alone, which overstated it.

**Latest, which may move:** [PoVPlus.Compat `main`](https://github.com/ave-satanas/PoVPlus.Compat). Use the pinned commit above for audit claims.

## Navigating

| Looking for | Go to |
|---|---|
| Audit scope, claims and falsification tests | [AUDIT.md](AUDIT.md) |
| Everything, by question | [INDEX.md](INDEX.md) |
| Current state | [CURRENT-STATE.md](CURRENT-STATE.md) |
| Why each fix exists | [Compatibility/CMP-index.md](Compatibility/CMP-index.md) |
| Past investigations and retractions | [Investigations/INV-index.md](Investigations/INV-index.md), [Investigations/INV-corrections.md](Investigations/INV-corrections.md) |
| Decisions and constraints | [Decisions/DEC-index.md](Decisions/DEC-index.md) |
| Warnings | [Diagnostics/DIAG-warning-ledger.md](Diagnostics/DIAG-warning-ledger.md) |
| Open questions | [Operations/OPS-remaining-work.md](Operations/OPS-remaining-work.md) |
| Older research, other environments | [Historical/HIST-index.md](Historical/HIST-index.md) |
| Evidence and how it was sanitized | [EvidenceBundle/README.md](EvidenceBundle/README.md) |

`[[double bracket]]` links work in Obsidian. On GitHub, they appear as text. The name is the filename.

## Kinds of claim

Records tag claims beside the claim. Full rules: [SCHEMA.md](SCHEMA.md).

| Tag | Means | Reviewer can check it? |
|---|---|---|
| `[checked]` | Files or source read directly, on the note's date | Partly; many read files were on the original machine |
| `[runtime]` | Observed in a run, cited by id such as M31 | Summary here; raw logs are local only |
| `[code]` | Concluded from reading code, not seen running | Yes, with the same mod versions |
| `[upstream]` | A mod author's statement | Not tested here |
| `[decision]` | Chosen by the project owner | Not a technical claim |
| `[hypothesis]`, `[unknown]` | Unproven, or an accepted gap | Open |

## Sanitized or unavailable

- [`EvidenceBundle/`](EvidenceBundle/README.md) is a **sanitized snapshot, not raw evidence**.
- Tested client configuration: a frozen, redacted subset in `EvidenceBundle/config-1.0.1/`. See its [README](EvidenceBundle/README.md).
- Not available: raw logs, saves, screenshots, game files and third-party mods.
- `evidence\`, `backups\` and drive-letter paths in notes are provenance from the original machine, and are not links. List them with `python tools/validate.py --external`.

## Checks

From the root of a clean clone, with Python 3 and nothing else:

```
python tools/validate.py
python tools/make_evidence_bundle.py --verify
```

These check structure and hashes only. Passing does not show that any claim is true.
