---
id: HIST-tome-compatibility
type: historical
status: historical
created: 2026-11-01
updated: 2026-11-01
related: [HIST-index, OPS-remaining-work]
specimen: PoVPlus-Dev, PoVPlus_Test / PoVPlus_Lab, 2026-10-25
applies_to:
  - MagicPlugin 2.2.2
  - Magic Supremacy 3.1.6
  - SeneaL UI 1.2.8
---
# Magic books and Tome slots

From `INV-003-Magic-book-tome-compatibility`, `COMP-002-Tome-bridge-design`, `DEV-005` to `DEV-007`, `EXP-004`, `EXP-005`, `ARCH-003`. **Development-install research. The bridge is not part of PoVPlus.Compat or the live profile.** `[checked]` no `Tome*` plugin file in live `plugins`.

**Question:** three MagicPlugin books, `BMP_BeginnersMagicBook`, `BMP_AdvanceMagicBook`, `BMP_DruidsTome`, could not use the Tome slot.

**Findings imported:**
- Two routes exist: PoV's native `utility.tome`, and the visible slot `AzuEPI.emulated/$tome_DO`, rendered by SeneaL UI with Magic Supremacy's `IsTomeItem` callback. `[runtime]` Stage A, in that environment.
- MagicPlugin's Tome classifier is disabled, and the books are absent from Magic Supremacy's registry. The prefab restriction was the confirmed visible rejection. A broad item-type mismatch did not explain it; all four compared prefabs were Utility/18.
- PoV's `utility.tome` returned false for all queried prefabs, and no separate rendered PoV Tome cell was proven.
- User preference then: Magic Supremacy as sole authoritative Tome equipment system, with MagicPlugin supplying books and effects. `[decision]`
- An experimental adapter, default off, passed Stage B **acceptance only**: three books passed descriptor, visible cell and direct internal validator. Native control accepted; Wood and a nonregistered belt rejected. `[runtime]` Session `2026-10-25_19-24-38`.

**Not verified there:** equip operations, effects, max Eitr, regeneration once, Belt and Tome exclusivity, save and load, safe disablement, multiplayer. Stage C1 was never completed.

**Related later work:** `S:\Valheim-Modding\PoVPlus-Claude-Solo` is a separate Tome Slot Compat plugin, putting the slot on the character doll as a SeneaL `Worn` slot. Its README claims verification against MS 3.1.6, MP 2.2.2 and SeneaL 1.2.8. `[upstream]` here: not checked by this project's evidence. `PROGRESS.md` notes it left `plugins` before 2026-10-30.

**Status for this project:** open design question, not on the current work list. It has not been re-examined on the live pack.
