# CarFix

Автосервис: Avalonia + **PostgreSQL**.

Логины: `admin`/`admin` · `manager`/`manager` · `mech1`/`mech1`

---

## Mac — самый простой способ (без Docker)

Другу на Mac: **только это**.

### 1. Скачай проект
```bash
git clone https://github.com/KoLose/CarFix.git
cd CarFix
git checkout feature/role-pages-and-db
```

### 2. Один раз разреши скрипт и запусти
```bash
chmod +x scripts/start-mac.sh
./scripts/start-mac.sh
```

Скрипт сам:
- поставит PostgreSQL (через Homebrew), если его нет  
- создаст базу `carfixdb`  
- поставит .NET, если нужно  
- откроет окно программы  

Если спросит пароль Mac — это нормально (Homebrew).  
Первый запуск может занять несколько минут.

### 3. Войди
- Админ: `admin` / `admin`  
- Менеджер: `manager` / `manager`  
- Механик: `mech1` / `mech1`  

---

## Windows — тоже просто (без Docker)

1. Установи [PostgreSQL](https://www.postgresql.org/download/windows/) — пароль пользователя `postgres` сделай **`123`**  
2. Установи [.NET 9 SDK](https://dotnet.microsoft.com/download)  
3. В PowerShell из папки проекта:

```powershell
.\scripts\start-windows.ps1
```

Или вручную:
```powershell
cd Infastructure\Sql
.\setup_db.ps1
cd ..\..\AvaloniaApp
dotnet run
```

---

## Что делает программа сама

При старте CarFix:
1. Ищет PostgreSQL на компьютере (Windows `postgres/123` или Mac-пользователь Homebrew)  
2. Создаёт базу `carfixdb`, если её нет  
3. Создаёт таблицы и тестовые данные, если база пустая  

**Docker не обязателен.** PostgreSQL — обязателен (ставит скрипт на Mac).

---

## Если на Mac «не работает»

1. Установи Homebrew: https://brew.sh  
2. Открой **Terminal**, перейди в папку `CarFix` (`cd .../CarFix`)  
3. Снова:
```bash
chmod +x scripts/start-mac.sh
./scripts/start-mac.sh
```
4. Если окно не открылось — скопируй красный текст ошибки из Terminal и пришли другу/автору  

Проверка, что PostgreSQL жив:
```bash
brew services list
psql -d carfixdb -c "SELECT 1"
```

---

## Роли (кратко)

| Роль | Логин | Что делать |
|------|--------|------------|
| Админ | `admin` | заказы, выручка, склад |
| Менеджер | `manager` | клиенты, машины, заказы (двойной клик), расписание |
| Механик | `mech1` | мои заказы (двойной клик), запрос на склад |

---

## Docker (не обязательно)

Только если хочешь UI в браузере. Сначала запусти Docker Desktop:
```bash
docker compose up -d --build
```
Открой http://localhost:6080/vnc.html  

Подробнее: [DOCKER.md](DOCKER.md) · Word: `CarFix_Guide.docx`
