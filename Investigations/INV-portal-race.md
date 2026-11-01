---
id: INV-portal-race
type: investigation
status: bounded
created: 2026-11-01
updated: 2026-11-01
related: [DIAG-warning-ledger]
applies_to:
  - PortalRules 1.1.8
---
# PortalRules early snapshot

**Mechanism:** the snapshot reaches the client while `GetServerRPC()` is null, between connect and world load, and is rejected. `[runtime]` Baseline hit 2/2 joins, acceptance 1/2.

**Recovery observed:** rejected, then accepted about 30 s later, with `hasSnapshot=True`. Timelines: t=75.2→109.6, 74.8→105.2, 245.6→276.9. Three of three instrumented joins. `[runtime]` M28

**Limit:** both test worlds gave `entries=0`, so this demonstrates recovery only with an **empty catalog**. A populated list was never observed. What triggers the second send was not traced. `[unknown]`

**Correction:** M11 read `RequestRefresh(force)` as the recovery path. The timeline shows no accepted send from that request, since its RPC was null.
