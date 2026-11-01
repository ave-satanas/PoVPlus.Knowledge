# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from tools/snapshot.ps1.
# Loads a COPY of the real world on a separate port and save root, with a server-only census. Never touches the real save root.
param([string]$Tag = 'census', [switch]$KeepCopy)
$w = 'S:\Documents\ChatGPT\PoVPlusPlus'; $i = '<steam-library>\.bakaloader-instances\Path Of Valheiman Plus'; $real = "$env:USERPROFILE\AppData\LocalLow\IronGate\Valheim\servers\Path Of Valheiman Plus"; $s = "$w\.sandbox\snap-saves"
if (-not $KeepCopy) { Remove-Item $s -Recurse -Force -EA 0; New-Item -ItemType Directory -Force "$s\worlds_local", "$s\ServerManager" | Out-Null
robocopy "$real\worlds_local\Wraeclast" "$s\worlds_local\Wraeclast" /E /NFL /NDL /NJH /NJS /NP | Out-Null }
robocopy "$i\BepInEx\plugins" "$s\ServerManager\required" /MIR /NFL /NDL /NJH /NJS /NP /XD BakaLoaderSpawnHelper BakaLoaderCommander BakaLoaderItemIndexer BakaLoaderKillAll _WorldCensus | Out-Null
$pd = "$i\BepInEx\plugins\_WorldCensus"; New-Item -ItemType Directory -Force $pd | Out-Null; Copy-Item "$w\.cache\census\WorldCensus.dll" $pd -Force; Remove-Item "$i\BepInEx\census.txt", "$i\BepInEx\LogOutput.log" -EA 0
$env:SteamAppId = '892970'
$proc = Start-Process "$i\valheim_server.exe" -WorkingDirectory $i -WindowStyle Hidden -PassThru -ArgumentList @('-nographics', '-batchmode', '-name', 'PovSnap', '-port', '2466', '-world', 'Wraeclast', '-password', '<disposable-password>', '-public', '0', '-savedir', "`"$s`"", '-logFile', "`"$s\server.log`"")
$t0 = Get-Date; while (((Get-Date) - $t0).TotalSeconds -lt 400 -and -not $proc.HasExited) { [Threading.Thread]::Sleep(5000); if ((Test-Path "$i\BepInEx\census.txt") -and (Select-String -Path "$i\BepInEx\census.txt" -SimpleMatch 'DONE' -Quiet)) { break } }
if (-not $proc.HasExited) { Stop-Process -Id $proc.Id -Force }; [Threading.Thread]::Sleep(2500); Remove-Item $pd -Recurse -Force -EA 0
New-Item -ItemType Directory -Force "$w\evidence\ops" | Out-Null; Copy-Item "$i\BepInEx\census.txt" "$w\evidence\ops\census-$Tag.txt" -Force -EA 0
"time $([int]((Get-Date)-$t0).TotalSeconds)s"; Get-Content "$w\evidence\ops\census-$Tag.txt" -EA 0 | ForEach-Object { $_.Substring(0, [Math]::Min(130, $_.Length)) } | Select-Object -First 30
Select-String -Path "$i\BepInEx\LogOutput.log" -Pattern 'Chainloader startup complete' | ForEach-Object { $_.Line.Substring(0, 90) }
