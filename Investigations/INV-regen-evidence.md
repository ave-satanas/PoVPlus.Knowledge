---
id: INV-regen-evidence
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
related: [CMP-health-regen]
applies_to:
  - FineDining 1.1.12
  - EpicLoot 0.15.4
---
# Equipped regen evidence

Earlier harness rounds only showed the path statically and with simulated data. Acceptance rests on real equipped items, read from the game's own tick. `[runtime]`

| Run | Tier | State | With compat | Without |
|---|---|---|---|---|
| F2 | C | Real helmet, gear reads 3 | 4.0867 | 1.0867 |
| M5 | C | Rolled `HelmetLeather`, 1.4 | 2.6066 | 1.2066 |
| M16 | Not stated, consolidated build | +2.2 helmet, Rested ×1.25 | 4.147 from 1.397 | Not repeated |

The M16 figures come from the session record; `EVIDENCE.md` cites `evidence\compat\` without restating them. `[unknown]` tier.

The bonus applies once and before the multiplier. Under Cold the tick halved, and gear with Cold was measured only without the compat.
