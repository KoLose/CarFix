# Builds portable apps with built-in DB file (carfix.db next to exe)
$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $root

$dist = Join-Path $root "dist"
New-Item -ItemType Directory -Force -Path $dist | Out-Null

function Publish-Rid([string]$rid, [string]$folderName) {
    $out = Join-Path $dist $folderName
    if (Test-Path $out) { Remove-Item $out -Recurse -Force }
    Write-Host "=== Publishing $rid -> $folderName ==="
    dotnet publish (Join-Path $root "AvaloniaApp\AvaloniaApp.csproj") `
        -c Release -r $rid --self-contained true `
        -p:PublishSingleFile=false `
        -o $out
    Set-Content -Path (Join-Path $out "portable.marker") -Value "1" -Encoding ASCII

    $howTo = @(
        "CarFix portable (DB inside, PostgreSQL not required)",
        "",
        "Windows: run AvaloniaApp.exe",
        "Mac: double-click AvaloniaApp (if blocked: right-click -> Open)",
        "",
        "Logins:",
        "  admin / admin",
        "  manager / manager",
        "  mech1 / mech1",
        "",
        "Database file carfix.db is created automatically next to the app."
    ) -join "`n"
    Set-Content -Path (Join-Path $out "HOW_TO_RUN.txt") -Value $howTo -Encoding UTF8
}

Publish-Rid "win-x64" "CarFix-Windows"
Publish-Rid "osx-arm64" "CarFix-Mac-AppleSilicon"
Publish-Rid "osx-x64" "CarFix-Mac-Intel"

Get-ChildItem $dist -Directory | ForEach-Object {
    $zip = Join-Path $dist ($_.Name + ".zip")
    if (Test-Path $zip) { Remove-Item $zip -Force }
    Compress-Archive -Path (Join-Path $_.FullName "*") -DestinationPath $zip -Force
    Write-Host "ZIP: $zip"
}

Write-Host ""
Write-Host "Done. Send ZIP from dist folder to your friend:"
Write-Host "  Windows: CarFix-Windows.zip"
Write-Host "  Mac Apple Silicon: CarFix-Mac-AppleSilicon.zip"
Write-Host "  Mac Intel: CarFix-Mac-Intel.zip"
