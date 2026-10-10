# CarFix — простой запуск на Windows (PostgreSQL, без Docker)
$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")

Write-Host ""
Write-Host "========================================"
Write-Host "  CarFix — запуск на Windows"
Write-Host "========================================"
Write-Host ""

$pgService = Get-Service -Name "postgresql*" -ErrorAction SilentlyContinue | Select-Object -First 1
if ($pgService) {
    if ($pgService.Status -ne "Running") {
        Write-Host "Запускаю службу PostgreSQL..."
        Start-Service $pgService.Name
    }
    Write-Host "[OK] PostgreSQL: $($pgService.Name)"
} else {
    Write-Host "[!] Служба PostgreSQL не найдена. Установи PostgreSQL и пароль пользователя postgres = 123"
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "Нужен .NET 9 SDK: https://dotnet.microsoft.com/download"
}

$env:PGPASSWORD = "123"
$psql = @(
    "C:\Program Files\PostgreSQL\18\bin\psql.exe",
    "C:\Program Files\PostgreSQL\17\bin\psql.exe",
    "C:\Program Files\PostgreSQL\16\bin\psql.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1

if ($psql) {
    Write-Host "Готовлю базу carfixdb..."
    & $psql -U postgres -h 127.0.0.1 -d postgres -c "SELECT 1" | Out-Null
    & $psql -U postgres -h 127.0.0.1 -d postgres -c "SELECT 'CREATE DATABASE carfixdb' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'carfixdb')\gexec" 2>$null
}

$env:CARFIX_CONNECTION = "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=123"

Write-Host "Запускаю приложение..."
Write-Host "Логины: admin/admin | manager/manager | mech1/mech1"
Write-Host ""
Set-Location AvaloniaApp
dotnet restore
dotnet run
