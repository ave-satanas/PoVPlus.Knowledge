# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from tools/server.ps1.
# Starts or stops the Bakaloader instance on a disposable world. Never touches the real world's save directory.
param([ValidateSet('start', 'stop', 'status')][string]$Do = 'status')
$i = '<steam-library>\.bakaloader-instances\Path Of Valheiman Plus'
$saves = 'S:\Documents\ChatGPT\PoVPlusPlus\.sandbox\srv-saves'
switch ($Do) {
  'start' {
    New-Item -ItemType Directory -Force $saves | Out-Null
    # ServerManager reads its allow-list from the save root, so the disposable world needs its own copy.
    robocopy "$i\BepInEx\plugins" "$saves\ServerManager\required" /MIR /NFL /NDL /NJH /NJS /NP /XD BakaLoaderSpawnHelper BakaLoaderCommander BakaLoaderItemIndexer BakaLoaderKillAll _ServerTest | Out-Null
    Remove-Item "$i\BepInEx\LogOutput.log" -ErrorAction SilentlyContinue
    $env:SteamAppId = '892970'
    Start-Process "$i\valheim_server.exe" -WorkingDirectory $i -WindowStyle Minimized -ArgumentList @('-nographics', '-batchmode', '-name', 'PovTestServer', '-port', '2456', '-world', 'PovSrvTest', '-password', '<disposable-password>', '-public', '0', '-savedir', "`"$saves`"", '-logFile', "`"$saves\server.log`"")
    $t0 = Get-Date
    while (((Get-Date) - $t0).TotalSeconds -lt 280) { Start-Sleep 5; if (Select-String -Path "$saves\server.log" -Pattern 'Game server connected$|Game server connected\r?$' -Quiet -ErrorAction SilentlyContinue) { break } }
    "up after $([int]((Get-Date) - $t0).TotalSeconds)s"
    Select-String -Path "$saves\server.log" -Pattern 'Game server connected|Valheim version|Load world|World loaded' | Select-Object -Last 4 | ForEach-Object { $_.Line.Substring(0, [Math]::Min(130, $_.Line.Length)) }
    Select-String -Path "$i\BepInEx\LogOutput.log" -Pattern 'Chainloader startup complete|Feed Barrel dispensing|Stack Compat|Bridge' -ErrorAction SilentlyContinue | ForEach-Object { $_.Line.Substring(0, [Math]::Min(150, $_.Line.Length)) }
  }
  'stop' { Get-Process valheim_server -ErrorAction SilentlyContinue | Stop-Process -Force; Start-Sleep 2; 'stopped' }
  'status' { Get-Process valheim_server -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
}
