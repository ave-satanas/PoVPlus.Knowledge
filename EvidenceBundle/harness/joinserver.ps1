# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from tools/joinserver.ps1.
# Launches through Steam and joins the local server with the plain Connect button (127.0.0.1:2456, password from ServerManager's client config).
$w = 'S:\Documents\ChatGPT\PoVPlusPlus'; . "$w\tools\ui.ps1"
$p = "$env:APPDATA\com.kesomannen.gale\valheim\profiles\PoVPlus"; $log = "$p\BepInEx\LogOutput.log"
Remove-Item $log -EA 0; Start-Process 'g:/steam/steam.exe' -ArgumentList @('-applaunch', '892970', '--doorstop-enabled', 'true', '--doorstop-target-assembly', "`"$p\BepInEx\core\BepInEx.Preloader.dll`"")
Start-Sleep 15
$t0 = Get-Date
while (((Get-Date) - $t0).TotalSeconds -lt 300) { Start-Sleep 5; if ((Test-Path $log) -and (Select-String -Path $log -SimpleMatch 'Start To Lobby profile completed' -Quiet -ErrorAction SilentlyContinue)) { break } }
Start-Sleep 12
foreach ($try in 1..3) {
  FocusGame; Start-Sleep -Milliseconds 400
  # An elevated foreground window silently blocks SendInput, so stop rather than click into nothing.
  if ([Ui]::GetForegroundWindow() -ne (Game).MainWindowHandle) { 'JOIN BLOCKED: game is not foreground'; exit 3 }
  Click 1280 830; Start-Sleep 3; Click 1280 1044
  Start-Sleep 20; if (-not (Select-String -Path $log -SimpleMatch 'Connecting to server' -Quiet)) { 'JOIN BLOCKED: no connection event after clicks'; exit 4 }
  $t0 = Get-Date; while (((Get-Date) - $t0).TotalSeconds -lt 170) { Start-Sleep 5; if (Select-String -Path $log -SimpleMatch 'Lobby To World profile completed' -Quiet) { break } }
  if (Select-String -Path $log -SimpleMatch 'Lobby To World profile completed' -Quiet) { break }
  FocusGame; Click 1280 788; Start-Sleep 2
}
Start-Sleep 12; FocusGame; Shot "$w\evidence\migration\server-join.png"
Select-String -Path $log -Pattern 'Chainloader startup complete|contract not recognised|changed shape|Unsupported|Feed Barrel dispensing' | ForEach-Object { $_.Line.Substring(0, [Math]::Min(170, $_.Line.Length)) }
