---
id: INV-sleep-protection
type: investigation
status: bounded
created: 2026-11-01
updated: 2026-11-01
related: [CMP-sleep-hud, DEC-test-constraints, INV-sleep-hud-rpc]
applies_to:
  - BetterSleepBruh 2.1.2
---
# Sleep-boost protections

All runs are **one real client with simulated sleepers**, from BetterSleepBruh test mode. They are not two-peer verification.

## Measured

`[runtime]` M13, 4× boost, 31.8 s, no bridge → bridge:

| Measure | No bridge | Bridge |
|---|---|---|
| World clock | +127 | +127 |
| Crop | +127 | +32 |
| Pickable | +127 | +31 |
| Mead | +293 | +32 |
| Oven | 42.9 s in 8.3 s | 32 s in 31.8 s |
| Boat aboard, ~64 s | 85.1%→54.6% | 85.1%→85.1% |
| Boat not aboard, 30 s | no damage | no damage |

The boat runs were not matched fixtures: the user boarded one, the probe placed the character on the other.

`[runtime]` M16, consolidated build: mead 1×, crop 1×, pickable 1×, oven 34.1→76.3 in 42.3 s, smelter about 1 s/s, continuous across boost start.

Bed step `[runtime]`: rate 3→6→3, pillows 1/5→2/5→1/5, label +12%→+24%→+12%. One real bed on top of simulated sleepers.

Mead: normal rate is about 2.3× from FineDining's environment speed. Unprotected boost was 9.2×, 4 × 2.3. Bridged mead is 1× under boost, slower than normal, but not a safety defect. `[runtime]`

## Simulated or controlled

- The other sleepers were simulated.
- Controlled ownership transition: one client left for 46 s and returned, with crop, mead and pickable each +46. This was not one peer handing to another.

## Waves

Upstream defect, reduced but **not normalized**. `StaticUpdate` runs from three update paths and BetterSleepBruh adds time on each, against its own decouple setting. `[code]`

`[runtime]` M17: upstream 3.29×, fixed 1.16× at world 4.08×, normal 1.00×. Boost start is continuous, with surface change 0.001 m. Boost end still steps: clock +811 s, surface 0.35 m. Hull lost 1% around then, the same size as ordinary shoreline drops, so it is **unattributed**. Hull stayed flat during boost.

## Limits

Untested: simultaneous clients, cross-peer ownership handoff, real players entering or leaving sleep. `[unknown]` See [[DEC-test-constraints]].

- Teardown counter and strip placement passed. `[runtime]` M16
- Screen positions were measured at 2560×1440 only.
