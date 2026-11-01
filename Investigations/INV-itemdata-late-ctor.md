---
id: INV-itemdata-late-ctor
type: investigation
status: resolved
created: 2026-11-01
updated: 2026-11-01
related: [CMP-item-data-stack]
applies_to:
  - EpicLoot 0.15.4
  - FeedLikeGrandma 1.2.1
---
# ItemDataStackCompat late-constructor defect

**Defect:** 1.0.0 passed play tests but was unsafe. ItemDataManager patches from a lazy static constructor, and FLG rechecks on every dispense. If a bundled `ItemInfo` was first used after startup, the transpiler returned, the next dispense failed and barrels stopped until restart. `[runtime]` M10, forced by the probe.

| Case | 1.0.0 | Corrected boundary |
|---|---|---|
| Late `ItemInfo` constructor | Barrels stop | Cannot happen, feeds normally |
| EpicLoot prefix replaced | Removed a live transpiler | Declines with a warning |
| Unrelated transpiler, same owner | Removed it | Left alone |

**Retraction:** 1.0.0's comment said the transpiler would empty `m_customData` on every merge. The code does not support that.

**Limit:** the constructors were forced. Which normal action would trigger one was not established. `[unknown]`

Evidence: `evidence\ids\client-1.0.0-late-ctor.txt`, `client-1.1.0-late-ctor.txt`.
