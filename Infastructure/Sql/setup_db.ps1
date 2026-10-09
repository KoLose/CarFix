# Recreate local carfixdb and load seed (postgres / 123)
$ErrorActionPreference = "Stop"
$psql = "C:\Program Files\PostgreSQL\18\bin\psql.exe"
if (-not (Test-Path $psql)) { $psql = (Get-Command psql -ErrorAction Stop).Source }

$env:PGPASSWORD = "123"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

& $psql -U postgres -h 127.0.0.1 -p 5432 -d postgres -f (Join-Path $root "_create_db.sql")
& $psql -U postgres -h 127.0.0.1 -p 5432 -d carfixdb -v ON_ERROR_STOP=1 -f (Join-Path $root "carfix_seed.sql")
Write-Host "carfixdb ready (postgres/123)."
