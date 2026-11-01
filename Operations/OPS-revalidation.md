---
id: OPS-revalidation
type: operation
status: active
created: 2026-11-01
updated: 2026-11-01
related: [CMP-index, MOD-inventory, OPS-test-tools]
---
# Update revalidation

First run `python Knowledge/tools/validate.py --applies <ModName>`. It lists records tied to that mod. It does not judge compatibility. Then read logs for each module's warning text.

## EpicLoot

Five modules depend on internals. After any update:
1. **Item stacking:** the prefix IL hash is pinned, so expect a decline if the prefix changed. Re-verify the prefix still returns false on every path, then update the hash in [[CMP-item-data-stack]]. Repeat barrel feeding and merges.
2. **Regen:** confirm `AddHealthTicks` exists, and test a real helmet. [[CMP-health-regen]]
3. **Burst:** confirm `_scaledBurstAttack` and `ShotProjectiles`, and test a burst proc. [[CMP-burst-multishot]]
4. **Input:** confirm `ShardUI.IsOpen`, `RegisterItemContainer`, `IsShardSlotPress`, then test E and Alt+E on rods and tankards. [[CMP-rod-bag]], [[CMP-tankards]]
5. **Map:** confirm pin properties, then click both chips. [[CMP-epic-map-chips]]

If `SocketsUI` returns, the three "compatibility unavailable" warnings should vanish and the bridges stand down. Check that happens cleanly.

## Other triggers

| Update | Recheck |
|---|---|
| FLG or FineDining | [[CMP-item-data-stack]], [[CMP-health-regen]], [[MOD-food-timers]] |
| Any mod bundling ItemDataManager | [[CMP-item-data-stack]] |
| BetterSleepBruh | [[CMP-sleep-hud]], waves, protections |
| SeneaL UI or API | [[CMP-sleep-hud]], [[CMP-epic-map-chips]], [[CMP-rod-bag]], [[CMP-tankards]] |
| UsefulTankards, TrollingFishing, SecondaryAttacks | Their module |
| ValheimCuisine, Monstrum, Warfare, Wizardry | [[CMP-cuisine-conversions]]; expect station counts 12/23/32/46 and rerun `tools\fdcheck.ps1` |
| Magic Supremacy | If `Seeker_Brood` is fixed, remove the override, [[INV-seekerbrood]] |
| CreatureManager | [[INV-toxic-aura]] |
| PortalRules | [[INV-portal-race]] |
| Valheim | All contracts; `WaterVolume`, `Minimap`, `ObjectDB`; the IL pin |
| BepInEx or HarmonyX | [[SYS-compat-architecture]], [[CMP-quiet-lookups]] |

Any DLL change needs a sync, see [[OPS-deployment-sync]], and a normal backup first, see [[OPS-backups]].
