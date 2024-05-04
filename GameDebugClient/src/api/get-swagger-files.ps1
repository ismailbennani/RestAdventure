$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath

Push-Location $dir

Write-Host "Downloading game.swagger.json from https://localhost:7056/swagger/game/swagger.json..."
Invoke-WebRequest -Uri "https://localhost:7056/swagger/game/swagger.json" -OutFile "game.swagger.json"
Write-Host "Done."

Write-Host "Downloading admin.swagger.json from https://localhost:7056/swagger/admin/swagger.json..."
Invoke-WebRequest -Uri "https://localhost:7056/swagger/admin/swagger.json" -OutFile "admin.swagger.json"
Write-Host "Done."

Pop-Location
