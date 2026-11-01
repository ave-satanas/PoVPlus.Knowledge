---
id: INDEX
type: index
status: active
created: 2026-11-01
updated: 2026-11-01
---
# Vault index

This vault is the current entry point for PoVPlus.Compat. Reviewers: start with `AUDIT.md`. Start here, then read only the records your task needs. Do not read the whole vault. Rules for notes: [[SCHEMA]].

## Where do I look?

| Question | Go to |
|---|---|
| What is deployed right now? | [[CURRENT-STATE]] |
| Which folder or machine is which? | [[SYS-environments]] |
| How is the plugin built and initialized? | [[SYS-compat-architecture]] |
| Build, package, release hashes | [[OPS-build-release]] |
| Sync client, server and allow-list | [[OPS-deployment-sync]] |
| Gale records, updates not taken, pack version | [[OPS-gale-profile]] |
| Save interval, shutdown, character store | [[OPS-saves-shutdown]] |
| Backups | [[OPS-backups]] |
| Memory headroom, test against normal workload | [[OPS-memory]] |
| A mod's version or dependencies | [[MOD-inventory]] |
| A mod updated: what do I recheck? | [[OPS-revalidation]] |
| Why a fix exists | [[CMP-index]] |
| Retired plugins | [[CMP-retired]] |
| What a log line means | [[DIAG-warning-ledger]] |
| Why a log line is missing | [[DIAG-logging]] |
| Past investigations | [[INV-index]] |
| Decisions and constraints | [[DEC-index]] |
| Tests and evidence | [[OPS-evidence-map]] |
| What has and has not been examined | [[CMP-coverage]] |
| Reports away from the workspace, local-only links | `EvidenceBundle/README.md` |
| How to run a test | [[OPS-test-tools]] |
| Remaining work | [[OPS-remaining-work]] |
| Older research, Tome slots, project philosophy | [[HIST-index]] (historical, other environments) |
| What happened in a session | [[SES-2026-11-01-knowledge-base]] |

## Update lookup

To list records tied to a mod: `python Knowledge/tools/validate.py --applies EpicLoot`.
