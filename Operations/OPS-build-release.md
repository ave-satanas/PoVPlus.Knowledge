---
id: OPS-build-release
type: operation
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [SYS-compat-architecture, DEC-production-builds, OPS-deployment-sync]
---
# Build and release

## Source control

`patches\PoVPlus.Compat` is its own git repository, pushed to private `github.com/ave-satanas/PoVPlus.Compat`. DLLs and zips are ignored there, so `patches\dist\` stays the home of release artifacts. `gh` is not installed on this machine; Git Credential Manager holds the GitHub login.

## Build

```powershell
dotnet build patches\PoVPlus.Compat\PoVPlus.Compat.csproj -c Release -o .sandbox\build\compat -p:BaseIntermediateOutputPath=S:\Documents\ChatGPT\PoVPlusPlus\.cache\obj\compat\
```

Run from the workspace root. Stop on a failed build: a stale DLL has been tested by mistake here before.

## Package

```powershell
Copy-Item .sandbox\build\compat\PoVPlus.Compat.dll patches\dist\ -Force
python patches\PoVPlus.Compat\pack.py
```

`pack.py` follows the Hexium rules in the user's earlier tools: manifest fields, name format, 256-character description, version, 256×256 icon, every file at the zip root. It writes `patches\dist\PoVPlus_Compat-<version>.zip`. Update `SHA256SUMS.txt` after packing. Hexium accepts each version once. **Nothing has been uploaded.** Rules came from that tool, not Hexium's live site.

Raise `version_number` in `package\manifest.json` and `Version` in `Plugin.cs` together, and update `package\CHANGELOG.md`.

## Canonical artifacts

| File | SHA-256 |
|---|---|
| `patches\dist\PoVPlus.Compat.dll` 1.0.1 | `8b766887e53a9fd285d68df4150fe153c8e3831ba07e57ffab0bf4ca487a75eb` |
| `patches\dist\PoVPlus_Compat-1.0.1.zip` | `77419c0891888222c794938e2710d172107b5c187a9d91adf4c7b6d2fd2159c1` |

Superseded 1.0.0 package: `patches\archived\PoVPlus_Compat-1.0.0.zip`, zip `ac0b485e…c5da`, DLL `51956f65…b27b`.

`[checked]` from `SHA256SUMS.txt` on 2026-11-01. Do not copy these elsewhere; link here.

Dependencies in the manifest: only `denikson-BepInExPack_Valheim-5.4.2360`, because each integration is optional. A mod manager install will not pull in the mods listed in [[MOD-inventory]].

Icon, empty `website_url` and license attribution are placeholders chosen by the agent.

## Release maintenance

After any change to a report, release file or version, before committing in `Knowledge`:

1. Update `SOURCES` in `tools/make_evidence_bundle.py` if the results files changed, such as a new acceptance folder.
2. Run `python Knowledge/tools/make_evidence_bundle.py`.
3. Run `python Knowledge/tools/validate.py`. A stale bundle is an error when the workspace is present.

This depends on the steps being run. Nothing enforces it at commit time; a git hook could, but none is installed. The bundle is a sanitized snapshot, never the release itself.
