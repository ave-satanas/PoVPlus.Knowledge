> **Sanitized snapshot, not raw evidence.** Generated from `COVERAGE.md`. See `README.md` in this bundle.

# Coverage ledger

Partial coverage. This is not proof that the rest of the pack is compatible.

Patch map: 8,114 patches on 1,921 methods, 598 shared by two or more owners (`evidence/runtime/B-patch/harmony-dump.tsv`).
It is the **server's** map; client-only runtime patching is not in it. Two-peer behaviour remains untested.

## Examined deeply

| Interaction | Why selected | Outcome |
|---|---|---|
| EpicLoot × FLG × FineDining × ItemDataManager, `AutoStackItems` | Only `[Error]` in the log | F1, patched, A/B/A |
| FineDining × EpicLoot × PoV, `UpdateFood` | Always-skip prefix over two transpilers | F2 patched; PoV verified safe |
| EpicLoot 0.15 × UsefulTankards × TrollingFishing input | Repeated warning, prior open question | F3 |
| Client × server deployment | Multiplayer | F4 |
| SouthsilArmor recipe identifier | Progression | F5 |

## Screened, no defect established

| Item | Finding |
|---|---|
| ServerManager "capture not accepted" | Fires in teardown windows; at logout the character was saved first. `GetPlayerID` returns 0 on an invalid view. No patch targets those members. Timing inferred from log order, not traced live. |
| `HitData.ApplyResistance` | AdditiveDamageModifier is the sole replacer; TouchGrass's prefix is void. |
| `InventoryGui.RepairOneItem`, `Fish.FindFloat`, `Vagon.Update`, `ItemDrop.GetHoverText` | Always-skip, but neighbours use void prefix and postfix or order around them. |
| `Feaster prefab not found` | First pre-vanilla pass only; later passes populate the table. |
| `Seeker_Brood` | Typo in Magic_Supremacy; drop restored through `DNS_character.yml`, measured 0.505%. The warning remains. |
| `BeeQueen` | OdinPlus-BeeQueen is not in this pack. Its dead spawn entry was removed; 11 saved creatures stay dormant (EVIDENCE M25, M26). |
| `Expand World Prefabs` no data | No rule files present. |
| Optional-mod probes (BetterRiding, QuickStackStore, ItemDrawers and others) | Absent optional targets. |
| DataForge `Food` category, HarnessPrefabs icons | Cosmetic or diagnostic; not traced further. |
| `AccessTools` misses | All traced to callers; see REPORT warnings. `_adventure*` are now served by EpicMapChips, not just missed. |

## Hypotheses

| Item | Status |
|---|---|
| SecondaryAttacks × EpicLoot Burst | **Confirmed in real client and patched.** See H1 |
| PoV Crossbows category | Rejected at `Masteries.Selected`. See H2 |
| Five DLLs changed after the last client log | **Resolved:** all loaded in the real client, 91 loaded, 0 failed |

## Unexamined

- 254 `can-skip` prefixes in `evidence/runtime/harmony-skip-screen.tsv`. Regex-classified and mostly conditional early-outs; only 7 `ALWAYS-SKIPS` were read. The classifier misread one of those, so treat the file as a lead list.
- Transpiler-to-transpiler ordering and postfix interplay on the other shared methods.
- Prefab and recipe identity across all content mods, beyond what load warnings reveal.
- Real-world save migration. Timer conversion was exercised only on disposable data (EVIDENCE M4).
- FineDining's winter pause: no seasonal mod installed.
- Multiplayer sync beyond one client; client UI and gameplay balance otherwise.
- A client-hosted run of Wraeclast, where vanilla may delete missing-prefab objects. Reinstalling BeeQueen or PlanBuild.
- The PlanHammer and BlueprintRune records on real characters when loaded on this pack.
- `baka_save` over RCON, and BakaLoader's stop on the real server.
- What destroys `PT_BlueRunePebble` at the second return to the menu. It is restored on the next join.
- A populated PortalRules catalog through recovery; test worlds showed `entries=0`.
- A full cook of the restored Monstrum, Warfare and Wizardry foods. A live server-owned FineDining container.
