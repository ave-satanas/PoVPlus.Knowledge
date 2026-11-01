# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from tools/fdcheck.ps1.
# Instrumented, disposable server. Fixtures use the console spawn and the game's auto-pickup; fdclock and fdrule only read. Run after compatsetup.ps1 -ServerPlugin.
$w = 'S:\Documents\ChatGPT\PoVPlusPlus'; $e = "$w\evidence\fdcheck"; New-Item -ItemType Directory -Force $e | Out-Null; $log = "$env:APPDATA\com.kesomannen.gale\valheim\profiles\PoVPlus\BepInEx\LogOutput.log"; $i = '<steam-library>\.bakaloader-instances\Path Of Valheiman Plus'; $aud = "$w\.sandbox\srv-saves\ServerManager\logs\events-audit.log"
. "$w\tools\ui.ps1"; . "$w\tools\pc.ps1"
$foods = 'CookedBearSteak_TW', 'CookedFoxMeat_TW', 'CookedSharkMeat_TW', 'BerryCakePlains_TW', 'BearSteak_TW', 'FoxMeat_TW', 'SharkMeat_TW', 'UncookedBerryCakePlains_TW', 'CookedMeat', 'NeckTailGrilled', 'Raspberry', 'RawMeat'
function Step($s) { "$(Get-Date -Format HH:mm:ss) $s" | Add-Content "$e\steps.txt"; Pc @("rem $s") 200 | Out-Null }
function Fg { FocusGame; Start-Sleep -Milliseconds 400; if ([Ui]::GetForegroundWindow() -ne (Game).MainWindowHandle) { Step 'ABORT game is not foreground'; Shot "$e\focus-blocked.png"; exit 3 } }
function Con($c) { Fg; Key f5; Start-Sleep 1; [Ui]::Press(0x0E, 60); Start-Sleep .2; TypeText $c; Start-Sleep .4; Key enter 240; Start-Sleep 1; Key f5; Start-Sleep 1 }
function Out1 { $n0 = (Get-Content $log).Count; foreach ($y in 984, 978, 992) { Fg; Key esc; Start-Sleep 1.5; Click 220 $y; Start-Sleep 2; Click 612 772; Start-Sleep 8; if (Get-Content $log | Select-Object -Skip $n0 | Select-String 'Net scene destroyed') { return $true }; Key esc; Start-Sleep 1; Key esc; Start-Sleep 1 }; Shot "$e\logout-failed.png"; $false }
function Saves { @(Select-String -Path $aud -SimpleMatch 'server.saved').Count }
Remove-Item "$e\steps.txt" -EA 0
Fg; Key enter 200; Start-Sleep 25; Pc @('god', 'clearinv') 800 | Out-Null
Step 'fixture: console spawn near player, auto-pickup'
Fg; Key f5; Start-Sleep 1; foreach ($f in $foods) { [Ui]::Press(0x0E, 60); Start-Sleep .2; TypeText "spawn $f 2"; Start-Sleep .4; Key enter 240; Start-Sleep 1.2 }; Key f5; Start-Sleep 2
Start-Sleep 6; Pc @('pickall') 1500 | Out-Null; Start-Sleep 4; Pc @('inv') 800 | Out-Null; Step 'station check'; Pc @(('fdstation BearSteak_TW FoxMeat_TW SharkMeat_TW RawMeat')) 2000 | Out-Null
if (@(Select-String -Path "$script:ProbeDir\log.txt" -Pattern 'item .*(CookedBearSteak_TW)').Count -eq 0) { Step 'no pickup yet, toggling auto-pickup'; Fg; [Ui]::Press(0x2F, 80); Start-Sleep 10; Pc @('inv') 800 | Out-Null }
Step 'observe A'; Pc @('fdcounts', 'fdclock', ('fdconv ' + ($foods -join ' ')), ('fdrule ' + ($foods -join ' '))) 1500 | Out-Null
Fg; Key tab; Start-Sleep 2; Shot "$e\inventory-a.png"; Key tab; Start-Sleep 1
Start-Sleep 60; Step 'observe B, 60 s later'; Pc @('fdclock') 1200 | Out-Null
$s0 = Saves; Con 'save'; Start-Sleep 15; Step "save: server.saved $s0 -> $(Saves)"
Step "logout $(Out1)"; Start-Sleep 18
$mark = (Get-Content $log).Count; Fg; Click 1280 830; Start-Sleep 3; Click 1280 1044; $t0 = Get-Date; while (((Get-Date) - $t0).TotalSeconds -lt 170) { Start-Sleep 4; if (Get-Content $log | Select-Object -Skip $mark | Select-String -SimpleMatch 'Lobby To World profile completed') { break } }
$tail = Get-Content $log | Select-Object -Skip $mark; Step "rejoin connect=$([bool]($tail | Select-String -SimpleMatch 'Connecting to server')) world=$([bool]($tail | Select-String -SimpleMatch 'Lobby To World profile completed'))"
Start-Sleep 14; Fg; Key enter 200; Start-Sleep 12; Pc @('god') 500 | Out-Null
Step 'observe C, after rejoin'; Pc @('fdcounts', 'inv', 'fdclock', ('fdrule ' + ($foods -join ' '))) 1500 | Out-Null
Con 'skiptime 270000'; Start-Sleep 30; Step 'observe D, after 75 h skip'; Pc @('inv', 'fdclock') 1500 | Out-Null
Fg; Key tab; Start-Sleep 2; Shot "$e\inventory-d.png"; Key tab; Start-Sleep 1
Step "logout2 $(Out1)"; Start-Sleep 14
Get-Process valheim -EA 0 | ForEach-Object { $_.CloseMainWindow() | Out-Null; if (-not $_.WaitForExit(45000)) { Stop-Process -Id $_.Id -Force } }; Start-Sleep 4
Get-Process valheim_server -EA 0 | ForEach-Object { $_.CloseMainWindow() | Out-Null; if (-not $_.WaitForExit(70000)) { Stop-Process -Id $_.Id -Force } }
Copy-Item "$script:ProbeDir\log.txt" "$e\probe-log.txt" -Force; Copy-Item "$i\BepInEx\fdserver.txt" "$e\server-probe.txt" -Force -EA 0; ([IO.File]::ReadAllText($log) -replace '<user>', '<user>' -replace '7656\d{13}', '<steamid>') | Set-Content "$e\client-LogOutput.log" -Encoding utf8NoBOM
