# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from tools/memlog.ps1.
# Samples available memory, commit, and game/server working set and private bytes every 5 s until the Stop file says DONE or the time limit passes.
# Start it detached, not as a job inside a tool call, so that it covers the whole run. Under 3 GB available it also lists top process groups in <Out>.top.txt.
param([string]$Out, [string]$Stop, [int]$Minutes = 25)
"time,availableMB,committedMB,commitLimitMB,clientWsMB,serverWsMB,clientPrivateMB,serverPrivateMB,clientPeakWsMB,serverPeakWsMB" | Set-Content $Out
$t0 = Get-Date
function Mb($v) { [int]($v / 1MB) }
while (((Get-Date) - $t0).TotalMinutes -lt $Minutes -and -not ((Test-Path $Stop) -and (Select-String -Path $Stop -SimpleMatch 'DONE' -Quiet))) {
  $os = Get-CimInstance Win32_OperatingSystem; $c = Get-Process valheim -EA 0 | Select-Object -First 1; $s = Get-Process valheim_server -EA 0 | Select-Object -First 1
  $available = [int]($os.FreePhysicalMemory / 1KB); $limit = [int]($os.TotalVirtualMemorySize / 1KB); $committed = $limit - [int]($os.FreeVirtualMemory / 1KB)
  "{0:HH:mm:ss},{1},{2},{3},{4},{5},{6},{7},{8},{9}" -f (Get-Date), $available, $committed, $limit, (Mb $c.WS), (Mb $s.WS), (Mb $c.PrivateMemorySize64), (Mb $s.PrivateMemorySize64), (Mb $c.PeakWorkingSet64), (Mb $s.PeakWorkingSet64) | Add-Content $Out
  if ($available -lt 3072) { "--- $(Get-Date -Format HH:mm:ss) available $available" | Add-Content "$Out.top.txt"; Get-Process | Group-Object ProcessName | ForEach-Object { "{0,7} MB WS {1,7} MB private  {2} x{3}" -f (Mb ($_.Group | Measure-Object WS -Sum).Sum), (Mb ($_.Group | Measure-Object PrivateMemorySize64 -Sum).Sum), $_.Name, $_.Count } | Sort-Object -Descending | Select-Object -First 10 | Add-Content "$Out.top.txt" }
  [Threading.Thread]::Sleep(5000)
}
