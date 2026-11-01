---
id: SYS-environments
type: system
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [CURRENT-STATE, OPS-deployment-sync, MOD-inventory]
---
# Environments

Do not assume these match. Each has its own files and versions.

| Name | Machine-local path | What it is |
|---|---|---|
| Workspace | `S:\Documents\ChatGPT\PoVPlusPlus` | Canonical for releases, evidence and this vault. Not a git repository itself |
| PoVPlus.Compat repository | `patches\PoVPlus.Compat`, remote `github.com/ave-satanas/PoVPlus.Compat`, private, branch `main` | Git repository for the plugin source and `package/` only, created 2026-11-01. The surrounding workspace is not a repository |
| Knowledge repository | `Knowledge`, remote `github.com/ave-satanas/PoVPlus.Knowledge`, private, branch `main` | This vault, its own git repository, in place. Created 2026-11-01 |
| Review repository | `github.com/ave-satanas/PoVPlus.Review`, private | Reviewer copy of this vault with no internal history. Each refresh adds one snapshot commit. `PoVPlus.Knowledge` stays the internal archive, because its history holds private terms. Refresh by exporting a snapshot; never push internal history there. Current review commit `bc8772f` |
| Live profile | `%APPDATA%\com.kesomannen.gale\valheim\profiles\PoVPlus` | Gale client profile. The real deployed client |
| Server instance | `G:\STEAM\steamapps\common\.bakaloader-instances\Path Of Valheiman Plus` | BakaLoader instance. Runs both real and disposable servers |
| Production save root | `%USERPROFILE%\AppData\LocalLow\IronGate\Valheim\servers\Path Of Valheiman Plus` | Real world `Wraeclast`, ServerManager store and `required` |
| Disposable server data | `.sandbox\srv-saves` in the workspace | World `PovSrvTest` |
| Older repository | `S:\Valheim-Modding\PoV-Plus` | Separate git repository with its own `Knowledge` vault from 2026-10-24. Historical, not canonical here. Unique research is imported in [[HIST-index]] |
| Development installation | `S:\Valheim-Modding\PoVPlus-Dev` | Separate game install with `PoVPlus.dll`. 90 DLLs against the live 88 |

`[checked]` for all of the above on 2026-11-01. Other `S:\Valheim-Modding` folders were not examined.

## PoVPlus versus PoVPlus.Compat

- **PoVPlus** is the mod pack, on Gale profile `PoVPlus`.
- **Path of Valheiman** (`treextr-PathOfValheiman` 5.3.6) is the upstream overhaul mod in that pack.
- **PoVPlus.dll** exists in the development installation. It is not in the live profile. Its relationship to this work was not examined. `[unknown]`
- **PoVPlus.Compat** is the custom compatibility plugin built here. See [[SYS-compat-architecture]].
