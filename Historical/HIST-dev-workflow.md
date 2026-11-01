---
id: HIST-dev-workflow
type: historical
status: historical
created: 2026-11-01
updated: 2026-11-01
related: [HIST-index, DEC-test-constraints]
specimen: older-vault PoV-Plus a08a373
---
# Development workflow history

From `DEC-003-Workflow-safety`, `DEV-003-Workflow`, `DEV-008-Supervised-testing`, `DEV-009-Shared-resource-coordination`.

**Applies to `S:\Valheim-Modding\PoV-Plus` only.** There, `Scripts/Dev.ps1` is the entry point, with an exact install allowlist, hash-verified backups, DLL-only managed state and explicit authorization for deploy, rollback and launch. Use those rules when working in that repository.

**Superseded here:** this workspace's later briefs authorize machine control and say sandboxing is unnecessary. See [[DEC-test-constraints]]. The older authorization rules do not bind this workspace.

**Useful lessons kept:**
- An `-Authorized` flag is an acknowledgement, not evidence of user approval.
- A general debugging request does not authorize mutation.
- Do not promote an absent Info event into an initialization failure. See [[DIAG-logging]].

**Differences between branches:** `claude/review-c1` and `integration/runtime-tier-a` carry DEV-009, while main carries [[HIST-supervised-testing]]. Unresolved between them: main still describes the worktree-local `workflow.lock`, and its Deploy, Rollback and Launch commands lack `-ValheimDevPath`.

**DEV-009, proposed 2026-10-27, offline synthetic tests only, pending review:** one OS-backed lease per installation, keyed by volume serial plus 128-bit file ID, held across each Deploy or Rollback and the Launch check. Unclean leases need an audited acknowledgement. Reparse points and path aliases are refused. Deploy, Rollback and Launch require an explicit `-ValheimDevPath`, and evidence scripts refuse other worktrees' evidence unless `-AllowExternalEvidence` is passed. It coordinates cooperative controllers. It is not isolation. Rotation 1 closure notes dated 2026-10-28 add framed lease records and refusal of multi-link write targets, pending reproduction.
