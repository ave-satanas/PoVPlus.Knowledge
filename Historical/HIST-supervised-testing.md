---
id: HIST-supervised-testing
type: historical
status: historical
created: 2026-11-01
updated: 2026-11-01
related: [HIST-index, HIST-dev-workflow, DEC-test-constraints]
specimen: older-vault PoV-Plus a08a373, main only, 2026-10-27
---
# Supervised testing research

From `DEV-008-Supervised-testing`, `EVID-002-Persistence-and-control`, `EXP-006-Testing-policy-fixtures`. These three records exist only in `PoV-Plus`. Level 0 design only; no isolation was ever certified.

**Isolation assessment, then:**

| Option | Verdict there |
|---|---|
| Copy the install, same Windows and Steam identity | Rejected for autonomous gameplay; profile, preferences and cloud stay shared |
| `-savedir` on the client | Rejected; the ordinary client parser does not handle it, and cloud and direct mod paths bypass it |
| Rewriting `USERPROFILE` or `APPDATA` | Rejected as a security boundary |
| Separate Windows account | Possible, with explicit ACLs and separate Steam identity |
| Disposable VM with a private disk | Preferred; GPU and entitlement unresolved |
| Windows Sandbox | Demonstration target at most |

**Useful facts, `[runtime]` in that environment:** `valheim.exe` and `UnityPlayer.dll` file version 6000.0.75 is a Unity version, not the game version. The registry product name still says Windows 10 on Windows 11 build 26200, so do not rely on it. Hypervisor presence does not prove a usable VM.

**EXP-006:** 28 Python tests in `Scripts/testing_policy.py`, offline only. Its all-true evidence object can be fabricated, so it never authenticates isolation.

**Relationship to this project:** the current position is that sandboxing is unnecessary. See [[DEC-test-constraints]]. This research is retained for if autonomous gameplay on shared saves is ever revisited.
