# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from tools/pc.ps1.
# Sends lines to the test-only client probe and returns what it logged. Dot-source after ui.ps1.
$script:ProbeDir = "$env:APPDATA\com.kesomannen.gale\valheim\profiles\PoVPlus\BepInEx\clientprobe"
function Pc([string[]]$cmds, [int]$wait = 800) {
  $log = "$script:ProbeDir\log.txt"; $n = (Get-Content $log).Count
  $s = [IO.File]::Open("$script:ProbeDir\cmd.txt", 'Append', 'Write', 'ReadWrite'); $b = [Text.Encoding]::UTF8.GetBytes(($cmds -join "`n") + "`n"); $s.Write($b, 0, $b.Length); $s.Close()
  Start-Sleep -Milliseconds $wait
  Get-Content $log | Select-Object -Skip $n
}
function ProbeSince([int]$n) { Get-Content "$script:ProbeDir\log.txt" | Select-Object -Skip $n }
function ProbeCount { (Get-Content "$script:ProbeDir\log.txt").Count }
