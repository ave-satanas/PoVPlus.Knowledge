---
id: DEC-production-session-hold
type: decision
status: blocked
created: 2026-11-01
updated: 2026-11-01
related: [OPS-remaining-work, INV-config-reconciliation, CURRENT-STATE]
---
# First production session

On 2026-11-01 the user asked for reconciliation, then a controlled first production session. The knowledge-base task that followed prohibited launching Valheim during that task only. **That was a task boundary, not a project restriction**, and it has ended. `[decision]`

**Status:** not started. It remains the planned next step once its prerequisites below are met.

**Before it runs:** an ordinary backup and the shutdown path in [[OPS-saves-shutdown]]. The FineDining check is done and no longer blocks it, see [[INV-cooking-conversions]]. External clients must be on 1.0.1. Check memory headroom and sample throughout, see [[OPS-memory]]. The first upgraded save converts timers, see [[DEC-finedining-rollback]].

The exact launch command for the real server was not recovered. `[unknown]` See [[OPS-saves-shutdown#Launch]].
