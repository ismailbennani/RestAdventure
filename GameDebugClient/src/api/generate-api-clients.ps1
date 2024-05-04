$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath

Push-Location $dir

Write-Host "Generating CS client from admin.swagger.json..."
npx nswag run config.nswag /variables:"Input=admin.swagger.json,Output=admin-api-client.generated.ts"
Write-Host "Done."

Write-Host "Generating CS client from game.swagger.json..."
npx nswag run config.nswag /variables:"Input=game.swagger.json,Output=game-api-client.generated.ts"
Write-Host "Done."

Pop-Location