---
id: MOD-food-timers
type: mod
status: active
created: 2026-11-01
updated: 2026-11-01
related: [CMP-item-data-stack, CMP-health-regen, INV-feedbarrel-retirement, INV-config-reconciliation, DEC-finedining-rollback, DEC-winter-off]
applies_to:
  - FeedLikeGrandma 1.2.1
  - FineDining 1.1.12
  - OdinHorse 1.9.4
---
# FeedLikeGrandma and FineDining

FLG 1.2.1 `2C82CFA3…0FD0` and FineDining 1.1.12 `D5A1CA66…5849` replaced earlier versions on 2026-10-30, with OdinHorse 1.9.4. Full hashes: M0.

- Timer records are `2|worldUid|epoch|value|portableRemaining`. **Older FineDining cannot read converted records.** See [[DEC-finedining-rollback]].
- Merges compose to the earliest expiry, once. Fresh food added to a nearly expired stack spoils with it. `[runtime]` M3, M8.
- Timers persisted through exit and reload, and a server restart. `[runtime]` M4.
- `SpoilagePolicy.Resolve` returns `NotTracked` for an item the classifier cannot place, and `Spoilage.yml` overrides are empty. `[checked]`
- Reference files are written only by the config authority, so client copies can be stale. `[checked]` See [[INV-config-reconciliation#FineDining classification]].
- Classification depends on station conversions existing when `FoodClassifier` runs. See [[INV-cooking-conversions]].

## Who processes timers

`DecayRuntime.Tick` runs on every peer, but acts only on what that peer holds. `[code]`
- **Player inventory:** only with `Player.m_localPlayer`, so on the client.
- **Containers and ground drops:** only if `NetworkView().IsOwner()`, so on the peer owning the live object.
- **Dedicated server:** no local player. It processes containers or drops only if it owns a live one.

`[runtime]` Across three runs, the server logged 0 timer writes and 0 expiries while the client logged all of them. That matches the model and is **not a defect**. Timers are absolute, so unloaded items age by clock when next processed. M4 showed a chest aging while the player was 2,500 m away.

A live server-owned container was not observed. `[unknown]`
