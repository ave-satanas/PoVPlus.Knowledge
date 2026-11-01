# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from tools/missingprefab.ps1.
# Observes what happens to objects with missing prefabs on the COPY of Wraeclast in .sandbox\snap-saves. Disposable character, no test plugins.
param([ValidateSet('join','visit')][string]$Stage)
$w = 'S:\Documents\ChatGPT\PoVPlusPlus'; $i = '<steam-library>\.bakaloader-instances\Path Of Valheiman Plus'; $s = "$w\.sandbox\snap-saves"; $e = "$w\evidence\ops\missingprefab"; $log = "$env:APPDATA\com.kesomannen.gale\valheim\profiles\PoVPlus\BepInEx\LogOutput.log"
New-Item -ItemType Directory -Force $e | Out-Null
if ($Stage -eq 'join') {
  robocopy "$w\.sandbox\srv-saves\ServerManager" "$s\ServerManager" /E /NFL /NDL /NJH /NJS /NP /XD required | Out-Null
  Set-Content "$s\adminlist.txt" '<steamid>'
  $env:SteamAppId = '892970'; Remove-Item "$s\server.log" -EA 0
  Start-Process "$i\valheim_server.exe" -WorkingDirectory $i -WindowStyle Minimized -ArgumentList @('-nographics', '-batchmode', '-name', 'PovTestServer', '-port', '2456', '-world', 'Wraeclast', '-password', '<disposable-password>', '-public', '0', '-savedir', "`"$s`"", '-logFile', "`"$s\server.log`"") | Out-Null
  $t0 = Get-Date; while (((Get-Date) - $t0).TotalSeconds -lt 300) { Start-Sleep 5; if ((Test-Path "$s\server.log") -and (Select-String -Path "$s\server.log" -SimpleMatch 'Opened Steam server' -Quiet)) { break } }; Start-Sleep 8
  & "$w\tools\joinserver.ps1" | Select-Object -First 1
  return
}
. "$w\tools\ui.ps1"
function Con($c) { FocusGame; Key f5; Start-Sleep 1; [Ui]::Press(0x0E, 60); Start-Sleep .2; TypeText $c; Start-Sleep .4; Key enter 240; Start-Sleep 1; Key f5; Start-Sleep 1 }
FocusGame; Key enter 200; Start-Sleep 20
$n = 0; foreach ($v in @(@(-940.6, 1177.2, 'planhammer'), @(36.9, 46.8, 'creature-a'), @(109.2, 34.6, 'creature-b'))) { Con "goto $($v[0]) $($v[1])"; Start-Sleep 22; Shot "$e\$($v[2]).png"; $n++ }
$c0 = (Get-Content $log).Count; foreach ($y in 984, 978, 992) { Key esc; Start-Sleep 1.5; Click 220 $y; Start-Sleep 2; Click 612 772; Start-Sleep 6; if (Get-Content $log | Select-Object -Skip $c0 | Select-String 'Local player destroyed') { break }; Key esc; Start-Sleep 1; Key esc; Start-Sleep 1 }; Start-Sleep 12
$g = Get-Process valheim -EA 0; if ($g) { $g.CloseMainWindow() | Out-Null; if (-not $g.WaitForExit(45000)) { Stop-Process -Id $g.Id -Force }; Start-Sleep 4 }
$p = Get-Process valheim_server -EA 0; if ($p) { $p.CloseMainWindow() | Out-Null; if (-not $p.WaitForExit(70000)) { Stop-Process -Id $p.Id -Force } }; Start-Sleep 3
$o = @("visited $n"); foreach ($q in 'Missing prefab hash', 'Destroyed invalid prefab', 'World saved', 'Failed to find item prefab') { $o += "$q : client $(@(Select-String -Path $log -SimpleMatch $q).Count), server $(@(Select-String -Path "$s\server.log" -SimpleMatch $q).Count)" }
$o += Select-String -Path $log -SimpleMatch 'Missing prefab hash' | Select-Object -First 8 | ForEach-Object { $_.Line.Substring(0, [Math]::Min(110, $_.Line.Length)) }
$o += Select-String -Path "$s\server.log" -SimpleMatch 'Destroyed invalid prefab' | Select-Object -First 8 | ForEach-Object { $_.Line.Substring(0, [Math]::Min(110, $_.Line.Length)) }
$o | Set-Content "$e\result.txt"; $o
