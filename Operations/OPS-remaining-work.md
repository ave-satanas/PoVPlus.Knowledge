---
id: OPS-remaining-work
type: operation
status: active
created: 2026-11-01
updated: 2026-11-01
related: [CURRENT-STATE, DEC-production-session-hold, INV-config-reconciliation, INV-location-data, DEC-control-click]
---
# Remaining work

| Item | Needs | Record |
|---|---|---|
| Cook a restored food end to end, with a memory timeline | Disposable data | [[INV-cooking-conversions]], [[OPS-memory]] |
| Refresh `PoVPlus.Review`: it predates the Gale record, inventory and M37 | User's go-ahead to upload, then export | [[OPS-gale-profile]] |
| Assign a pack version; ignore or evaluate the three offered updates | User | [[OPS-gale-profile]] |
| Establish whether the host normally plays on this PC | User | [[OPS-memory]] |
| Confirm whether inactive location YAML is intended | User | [[INV-location-data]] |
| Trace the five report-file differences, if they matter | Reading only | [[INV-config-reconciliation]] |
| First production session | Prerequisites met | [[DEC-production-session-hold]] |
| Find the real launch configuration | Reading BakaLoader's store | [[OPS-saves-shutdown#Launch]] |
| `baka_save` cause | Test on disposable | [[OPS-saves-shutdown]] |
| Grant reviewer access to `PoVPlus.Review` and `PoVPlus.Compat` | User, with the reviewer's account | [[SYS-environments]] |
| Hexium upload | User | [[OPS-build-release]] |
| Populated portal catalog through recovery | World with public portals | [[INV-portal-race]] |
| What destroys `PT_BlueRunePebble` | Tracer | [[INV-objectdb-null]] |

**Deferred:** [[DEC-control-click]]. **Excluded:** two-peer testing, [[DEC-test-constraints]]; FineDining downgrade work, [[DEC-finedining-rollback]].
