---
id: DEC-finedining-rollback
type: decision
status: active
created: 2026-11-01
updated: 2026-11-01
related: [MOD-food-timers, OPS-backups]
applies_to:
  - FineDining 1.1.12
---
# FineDining downgrade out of scope

**Decision:** `[decision]` No downgrade testing or rollback preparation. Older FineDining cannot read converted timer records. Existing backups stay untouched, and normal save backups continue before updates. Revisit recovery only if an upgrade actually breaks the pack.
