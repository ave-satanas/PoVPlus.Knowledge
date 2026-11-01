---
id: MOD-epicloot
type: mod
status: active
created: 2026-11-01
updated: 2026-11-01
related: [CMP-item-data-stack, CMP-health-regen, CMP-burst-multishot, CMP-rod-bag, CMP-tankards, CMP-epic-map-chips, OPS-revalidation]
applies_to:
  - EpicLoot 0.15.4
---
# EpicLoot

Version 0.15.4. Five integrations depend on its internals, more than any other mod, so an update is the highest-risk event for this pack. See [[OPS-revalidation#EpicLoot]].

- `0.15` removed `ShardStones.SocketsUI`, breaking UsefulTankards, TrollingFishing and SecondaryAttacks' binds. Their warnings are expected.
- Its `AutoStackItems` prefix always skips the original. `[code]`
- Generated configs such as `baseconfig\iteminfo.json` can differ in array order between machines. See [[INV-config-reconciliation]].
