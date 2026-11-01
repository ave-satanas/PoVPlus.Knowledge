---
id: DEC-control-click
type: decision
status: deferred
created: 2026-11-01
updated: 2026-11-01
related: [OPS-remaining-work]
applies_to:
  - ServerManager 1.2.6
---
# ServerManager Control+Click

**Status:** **deferred, not implemented, not blocking.** `[decision]` ServerManager's decompiled code had no control-key or editor handling when searched on 2026-10-31. `[checked]`

**Requested behaviour:**
- Click connects normally.
- Alt+Click opens the local menu.
- Control+Click opens a dialog to edit and persist the client's configured server.
- Save affects the next connection. Cancel preserves the old destination.
- Alt takes precedence if both are held.
