---
id: OPS-test-tools
type: operation
status: active
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [DEC-production-builds, DEC-test-constraints, OPS-evidence-map]
---
# Test tools

All under `tools\`. `[checked]` None belong in a release. Remove test plugins before acceptance.

| Tool | Use |
|---|---|
| `finalrun.ps1 -Stage setup`, then `-Stage play`, with `-OutDir` | Clean acceptance. Two stages fit foreground time limits |
| `memlog.ps1` | Memory sampling |
| `compatsetup.ps1` | Install, sync, probe, test mode, disposable server, join. `-NoTest` for clean, `-Admin` to keep admin |
| `compatrun.ps1`, `compatpart2-4.ps1` | Consolidated feature matrix |
| `warnprobe*.ps1` | Warning investigations |
| `fdcheck.ps1` | FineDining rules and timers on disputed foods against vanilla controls. Run after `compatsetup.ps1 -ServerPlugin .cache\fdserverprobe\FdServerProbe.dll`; writes `evidence\fdcheck\`. Only a finished join is valid: `joinserver.ps1` exits 3 or 4 if blocked |
| `FdServerProbe\` | Server-only test plugin, in `_ServerTest`, which `server.ps1` keeps out of the disposable allow-list. A sync removes it |
| `missingprefab.ps1`, `snapshot.ps1` | Copied-world tests. `snapshot.ps1` loads a copy on port 2466, `-KeepCopy` reuses it |
| `ClientProbe\`, `WorldCensus\`, `CallerTrace\`, `IdsProbe\` | Test plugins. ClientProbe commands read and set up state |
| `opstest.ps1`, `rcon.ps1` | Shutdown and RCON |
| `tools\archived\` | Pre-consolidation harness |

**Memory:** see [[OPS-memory]]. About 21 GB available is provisional guidance. Run one test session at a time.

**Pitfalls learned here:**
- Build, check the build, then launch. A stale DLL has been tested.
- A long foreground call can be moved to the background, and a call sent alongside it then runs concurrently. Do not send a dependent step in the same batch as a long setup.
- `fdrule` and `fdclock` call FineDining's `Resolve`, which can build the classifier cache. Observe only after the game has acted.
- A death destroys the player and falsely satisfies a Log Out check. Use god mode or match `Net scene destroyed`.
- Probe `Awake` runs before many mods load. Bind to them in `Start`.
- PowerShell's `R` is an alias, so helper functions must not use that name.
- An elevated foreground window such as Task Manager blocks `SendInput` and focus changes. Clicks silently fail, and the join script times out at the menu. Check the foreground process before a UI run.
- Console Enter needs about 240 ms. The rules popup takes the first Enter.

The test character is `Povserver` and the world `PovSrvTest`. UI coordinates assume 2560×1440.
