---
id: MOD-inventory
type: inventory
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CURRENT-STATE, CMP-coverage, OPS-revalidation]
specimen: live-profile
---
# Mod inventory

Read-only scan of plugin folder manifests in the **live profile**, 2026-11-01 00:40 America/Los_Angeles. 87 folders, 88 DLLs. `[checked]` Upstream mods are separate from the custom plugin. Only versions come from manifests; compatibility is not implied. Rechecked against Gale's records on 2026-11-01: 88 records, all versions as below, none updated upstream by choice. See [[OPS-gale-profile]].

Pack base: Valheim 1.0.21, BepInExPack 5.4.2360. `[runtime]`

**Custom:** `PoVPlus.Compat` 1.0.1, a Gale local import in `plugins\PoVPlus_Compat`, see [[OPS-build-release]].

| Author | Mods |
|---|---|
| sighsorry | AdditiveDamageModifier 1.2.4, Blasted_Swimming_Tarred_Bug_Fix 1.2.6, BossRules 1.1.8, BottleShips 1.1.17, CaptainValheim 1.1.1, Clan 1.1.5, ConfigManager 1.0.8, CreatureManager 1.3.10, DataForge 1.4.0, Dive_In 1.2.7, DropNSpawn 1.4.9, Fast_AssetBundle_Loader 1.0.7, FearNoSpear 1.1.3, FeedLikeGrandma 1.2.1, FineDining 1.1.12, Groundwork 1.1.21, HarnessPrefabs 1.1.6, LoadTimeProfiler 1.3.2, NoCapLumber 1.0.8, NoSnowyBiome 1.1.2, PathOfValheimanCraftyBoxesBridge 1.0.1, PortalRules 1.1.8, ReferToCompendium 1.0.2, RepairRequiresMaterials 1.1.2, SecondaryAttacks 1.2.21, ServerManager 1.2.6, ServeYouRight 1.0.7, SkadiNet 1.1.6, TerrainMistile 1.1.2, TouchGrass 1.0.7, Trolling_Fishing 1.1.5, UsefulRunestones 1.0.3, UsefulTankards 1.0.9, WarfareTweaks 1.0.5, YouAreNotWorthy 1.1.0 |
| JereKuusela | Expand_World_Data 1.79.0, Expand_World_Music 1.16.0, Expand_World_Prefabs 1.65.0, Server_devcommands 1.116.0, Structure_Tweaks 1.37.0, Upgrade_World 1.87.0 |
| Azumatt | AzuCraftyBoxes 1.8.28, BowsBeforeHoes 2.0.2, HelheimHarmonizer 1.0.11, Unshamed 1.0.9 |
| Therzie | Armory 1.4.3, Monstrum 1.6.1, Warfare 1.9.6, Wizardry 1.2.3 |
| OdinPlus | OdinHorse 1.9.4, OdinsKingdom 1.6.3, OdinsTraps 1.2.1 |
| VentureValheim | Muted_Mist 1.0.1, Venture_Floating_Items 1.0.1, Venture_Logout_Tweaks 1.0.0 |
| ValheimModding | Jotunn 2.30.2, JsonDotNET 13.0.4, YamlDotNet 16.3.1 |
| shudnal | ConditionalConfigSync 1.0.11, GammaOfNightLights 1.0.10, MoreGatesExtended 1.0.5 |
| seneaL | SeneaL_API 1.2.0, SeneaL_UI 1.2.8 |
| blacks7ar | MagicPlugin 2.2.2, RegenWhileSitting 1.0.8 |
| Dreanegade | Artisan_Mastery 1.1.1, Magic_Supremacy 3.1.6 |
| treextr | PathOfValheiman 5.3.6, PoV_Link 0.3.0 |
| Voidey | Lich_King 1.1.0, WoWArmors 1.1.2 |
| Others | Advize Armoire 1.2.2, BentoG MissingPieces 2.3.2, Crystal Comfortable 1.2.3, Gurebu Riverheim 1.2.1, MagicMike MyDirtyHoe 2.3.3, MidnightMods ValheimArmory 1.37.0, MSchmoecker VNEI 0.17.6, Radamanto Bestiary 1.2.2, RandyKnapp EpicLoot 0.15.4, Smoothbrain Resurrection 1.0.15, Soloredis RtDOcean 2.3.2, southsil SouthsilArmor 3.2.2, TastyChickenLegs NoSmokeStayLit 2.4.4, Vapok BetterSleepBruh 2.1.2, Xutz ValheimCuisine 2.3.4 |

`Fast_AssetBundle_Loader` and `LoadTimeProfiler` have no DLL under `plugins`, as they are patchers. `ConditionalConfigSync`, `JsonDotNET` and `YamlDotNet` hold two DLLs each.

Server-only additions on the instance: BakaLoader tools, listed in [[CURRENT-STATE]].

**Removed:** PlanBuild, see [[DEC-planbuild-removal]]. **Not installed:** OdinPlus-BeeQueen, see [[INV-missing-prefabs]]; any seasonal mod, see [[DEC-winter-off]].

Dependency counts were read in the same scan, but the graph was not reviewed. `[unknown]`
