# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from tools/compatsetup.ps1.
# Installs PoVPlus.Compat in place of the standalone DLLs, plus test probe and test mode, then starts the disposable server and joins.
param([switch]$NoTest, [switch]$Admin, [string]$ServerPlugin)
$w = 'S:\Documents\ChatGPT\PoVPlusPlus'; $p = "$env:APPDATA\com.kesomannen.gale\valheim\profiles\PoVPlus"; $b = "$p\BepInEx"; $i = '<steam-library>\.bakaloader-instances\Path Of Valheiman Plus'; $sl = "$w\.sandbox\srv-saves\server.log"
$g = Get-Process valheim -EA 0; if ($g) { $g.CloseMainWindow() | Out-Null; if (-not $g.WaitForExit(45000)) { Stop-Process -Id $g.Id -Force }; [Threading.Thread]::Sleep(4000) }
& "$w\tools\server.ps1" stop | Out-Null
$old = "$w\patches\archived\standalone-superseded"; New-Item -ItemType Directory -Force $old | Out-Null
foreach ($dir in 'PoVPlusPlus-Compat', 'SenealUI-UsefulTankards-Bridge-2.1.0') { $src = "$b\plugins\$dir"; if (Test-Path $src) { robocopy $src "$old\$dir" /E /NFL /NDL /NJH /NJS /NP | Out-Null; Remove-Item $src -Recurse -Force } }
Remove-Item "$b\plugins\_CallerTrace", "$b\plugins\_IdsProbe" -Recurse -Force -EA 0
New-Item -ItemType Directory -Force "$b\plugins\PoVPlus_Compat" | Out-Null; Copy-Item "$w\.sandbox\build\compat\PoVPlus.Compat.dll" "$b\plugins\PoVPlus_Compat\" -Force
if ($NoTest) { Remove-Item "$b\plugins\_ClientProbe", "$b\clientprobe" -Recurse -Force -EA 0; Remove-Item "$w\.sandbox\srv-saves\adminlist.txt" -EA 0; if ($Admin) { Set-Content "$w\.sandbox\srv-saves\adminlist.txt" '<steamid>' } }
else { New-Item -ItemType Directory -Force "$b\plugins\_ClientProbe" | Out-Null; Copy-Item "$w\.cache\clientprobe\ClientProbe.dll" "$b\plugins\_ClientProbe\" -Force; Set-Content "$w\.sandbox\srv-saves\adminlist.txt" '<steamid>' }
pwsh -NoProfile -File "$p\Update Server Mods.ps1" 2>&1 | Select-Object -Last 1 | ForEach-Object { "$_".Substring(0, [Math]::Min(60, "$_".Length)) }
if ($ServerPlugin) { New-Item -ItemType Directory -Force "$i\BepInEx\plugins\_ServerTest" | Out-Null; Copy-Item $ServerPlugin "$i\BepInEx\plugins\_ServerTest\" -Force }
if (-not $NoTest) { $c = "$i\BepInEx\config\vapok.mods.BetterSleepBruh.cfg"; (Get-Content $c -Raw).Replace('Enable Testing Mode = false', 'Enable Testing Mode = true').Replace('Fake Total Players = 10', 'Fake Total Players = 4').Replace('Simulate Players In Bed = 18', 'Simulate Players In Bed = 1') | Set-Content $c -NoNewline }
Remove-Item $sl -EA 0; $j = Start-Job -ArgumentList $w { param($w) & "$w\tools\server.ps1" start }; $t0 = Get-Date; while (((Get-Date) - $t0).TotalSeconds -lt 220) { [Threading.Thread]::Sleep(5000); if ((Test-Path $sl) -and (Select-String -Path $sl -SimpleMatch 'Opened Steam server' -Quiet)) { break } }; [Threading.Thread]::Sleep(8000); Remove-Job $j -Force -EA 0
& "$w\tools\joinserver.ps1" | Select-Object -First 1 | ForEach-Object { $_.Substring(0, [Math]::Min(90, $_.Length)) }
