---
id: OPS-memory
type: operation
status: bounded
created: 2026-11-01
updated: 2026-11-01
last_verified: 2026-11-01
related: [OPS-test-tools, DEC-production-session-hold, OPS-saves-shutdown, SYS-environments]
---
# Memory headroom

The machine has 31.9 GB. `FreePhysicalMemory` matched Windows `Available MBytes` in one comparison (18,312 against 18,236 MB), so "free" below is treated as available memory. `[checked]` once, idle.

## Measured runs

From `memory.csv` in each evidence folder. `[runtime]` "Remainder" is total minus available minus client and server working sets. **It is not process memory.** System accounting includes kernel use, caches, compression and shared pages, so subtracting working sets does not identify what else was using memory.

| Run | Free before start | Lowest sampled free | Client WS then | Server WS then | Remainder | Sampled to the end? |
|---|---|---|---|---|---|---|
| `evidence\final` | 21,482 | 3,146 | 11,748 | 6,517 | 10,450 | Yes |
| `evidence\acceptance` | 21,539 | 5,520 | 9,024 | 6,580 | 10,737 | Yes |
| `evidence\acceptance-1.0.1` | 19,389 | **597** | 10,982 | 6,584 | 13,698 | **No** |

## The 597 MB reading

- **Game and server working sets:** 17,566 MB, a measured sum. Shared pages could be counted in both, so it is not an exact share of RAM.
- **Remainder:** 13,698 MB, an accounting difference, not a total of other processes. It cannot name consumers. Only `valheim` and `valheim_server` were sampled, and no commit or private-bytes data exists. An earlier statement that this came "from other load on the machine" was unsupported.
- **The remainder was about 3 GB higher than in the earlier runs.** It was already 12,472 MB before the game started, against about 10,300 MB. The cause is unknown. A `dotnet` build and rebuild ran just before, so that is a `[hypothesis]` only.
- **597 MB is not the true minimum.** It is the final sample. The sampler was a job inside one tool call, and stopped at 13:00:35 with the client still growing and free memory falling about 200 MB per 5 s. Nothing was recorded for the rest of that run. It finished normally, but its real low point is `[unknown]`.

`evidence\baseline\memory.csv` is invalid: two samplers wrote to it.

## Other processes, later snapshot only

Read 2026-11-01 with the game closed and 18.3 GB free. `[checked]` This shows what normally runs here. It does **not** show what used memory at 13:00.

svchost 1.7 GB, zen 1.6 GB, ChatGPT 1.4 GB, claude 1.2 GB, Discord 0.9 GB, Memory Compression 0.8 GB, RazerAppEngine 0.6 GB, msedge 0.5 GB, steamwebhelper 0.4 GB, MsMpEng 0.3 GB.

## Workloads

| Workload | Game processes | Status |
|---|---|---|
| **Combined test:** client and disposable server on this PC | About 18.3 GB, highest sum | `[runtime]` M22 |
| **Dedicated server only:** no client here | About 7.1 GB peak | `[runtime]` on `PovSrvTest`. A `Wraeclast` copy read about 6.6 GB just after a join |
| **Client only:** external player's PC | About 11.8 GB peak | `[runtime]` on this PC |
| **Host and play on this PC, real world** | At least the combined figure | Same two processes. **Not measured** on the real world, with real play time or other players |

So the combined test is not automatically different from normal use. It matches normal deployment only if the host also plays on this PC. Whether that is the normal arrangement was not established. `[unknown]`

Long sessions and more connected players were not measured. All figures come from short tests.

## Practical guidance

- **About 21 GB available is provisional guidance, not a demonstrated requirement or a guarantee.** Two runs starting there stayed above 3 GB at their sampled lows. One run starting at 19.4 GB reached 597 MB at an incomplete sample. Three runs do not establish a threshold.
- Close other applications before a combined run, or run the server alone.
- A low-memory reaper killed a test at about 3.8 GB free while the session was idle, on 2026-10-31.
- For the first production session, sample throughout with the fields under Sampling.

## Sampling

`tools\memlog.ps1` records available memory, committed memory, commit limit, and client and server working set and private bytes. Under 3 GB available it writes top process groups, with working set and private bytes, to `<out>.top.txt`. Start it as a detached process, not a job inside a tool call, so that it covers the whole run. Tested on its own with a raised threshold on 2026-11-01, not during a game run.

**Next timeline:** collect it during the next already-needed cooking and rejoin test. No separate test campaign is needed. Existing `memory.csv` files use the older columns.
