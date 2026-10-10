#!/bin/bash
# CarFix — один скрипт для Mac (PostgreSQL + запуск приложения, без Docker)
set -e
cd "$(dirname "$0")/.."
ROOT="$(pwd)"

echo ""
echo "========================================"
echo "  CarFix — запуск на Mac"
echo "========================================"
echo ""

# --- Homebrew ---
if ! command -v brew >/dev/null 2>&1; then
  echo "Нужен Homebrew. Установи одной командой:"
  echo '/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"'
  echo "Потом снова запусти: ./scripts/start-mac.sh"
  exit 1
fi

# --- PostgreSQL ---
export PATH="/opt/homebrew/opt/postgresql@16/bin:/opt/homebrew/opt/postgresql@15/bin:/opt/homebrew/bin:/usr/local/opt/postgresql@16/bin:/usr/local/bin:$PATH"

if ! command -v psql >/dev/null 2>&1; then
  echo "[1/4] Ставлю PostgreSQL (один раз, подожди)..."
  brew install postgresql@16
  brew link postgresql@16 --force 2>/dev/null || true
  export PATH="/opt/homebrew/opt/postgresql@16/bin:/usr/local/opt/postgresql@16/bin:$PATH"
else
  echo "[1/4] PostgreSQL уже есть"
fi

echo "[2/4] Запускаю PostgreSQL..."
brew services start postgresql@16 2>/dev/null \
  || brew services start postgresql@15 2>/dev/null \
  || brew services start postgresql 2>/dev/null \
  || pg_ctl -D /opt/homebrew/var/postgresql@16 start 2>/dev/null \
  || true
sleep 2

# Homebrew обычно создаёт роль = имя пользователя Mac
createdb carfixdb 2>/dev/null || true
echo "     База carfixdb готова (или уже была)"

# --- .NET ---
export PATH="$HOME/.dotnet:/usr/local/share/dotnet:/opt/homebrew/opt/dotnet@9/bin:$PATH"
if ! command -v dotnet >/dev/null 2>&1; then
  echo "[3/4] Ставлю .NET SDK..."
  brew install dotnet@9 2>/dev/null || brew install dotnet
  export PATH="/opt/homebrew/opt/dotnet@9/bin:/usr/local/share/dotnet:$PATH"
else
  echo "[3/4] .NET уже есть: $(dotnet --version)"
fi

# Подсказка приложению: пользователь Mac без пароля
export CARFIX_CONNECTION="Host=localhost;Port=5432;Database=carfixdb;Username=$(whoami);Password="

echo "[4/4] Запускаю CarFix..."
echo "     Логины: admin/admin  |  manager/manager  |  mech1/mech1"
echo ""
cd "$ROOT/AvaloniaApp"
dotnet restore
dotnet run
