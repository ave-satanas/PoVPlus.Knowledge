---
id: INV-persistence
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
related: [OPS-saves-shutdown]
---
# Persistence anomalies

**`save` did nothing:** harness fault. An 80 ms Enter left text in the box, the rules popup took Enter, and one attempt added admin while the server was up. A 200 ms press with admin worked. `[runtime]` M9

**Character "restored":** expected. ServerManager's store replaces local and cloud copies on join, even when the local copy is newer. It was reproduced deliberately. Character and world roll back as a pair, and timers stay consistent. `[runtime]` M9

**Code only:** a restored character in the same world with a different clock epoch freezes timers and warns. `[code]`
