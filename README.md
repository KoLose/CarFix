# CarFix

Система управления автосервисом на Avalonia (.NET 9) + PostgreSQL.

Поддерживаемые роли: **Admin**, **Manager**, **Mechanic**.

---

## Быстрый старт на любом устройстве (Windows / macOS)

Нужен [Docker Desktop](https://www.docker.com/products/docker-desktop/) — **сначала запусти Docker Desktop**, дождись статуса Running, затем:

```bash
git clone https://github.com/KoLose/CarFix.git
cd CarFix
git checkout feature/role-pages-and-db
docker compose up -d --build
```

Открой в браузере:

**http://localhost:6080/vnc.html** → кнопка **Connect**

| Сервис | Адрес |
|--------|--------|
| UI (через браузер) | http://localhost:6080/vnc.html |
| PostgreSQL | `localhost:5433` |

БД внутри Docker: пользователь `postgres`, пароль `123`, база `carfixdb` (данные уже загружены).

Остановка:

```bash
docker compose down
```

Если Docker не установлен или не запускается — используй локальный запуск ниже.

---

## Локальный запуск (Windows) — рекомендуемый вариант для разработки

### Требования
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL (служба должна быть **Running**)
- Учётка БД по умолчанию: `postgres` / `123`

### 1. База данных

```powershell
cd Infastructure\Sql
.\setup_db.ps1
```

Или вручную:

```powershell
$env:PGPASSWORD='123'
psql -U postgres -h 127.0.0.1 -c "DROP DATABASE IF EXISTS carfixdb;"
psql -U postgres -h 127.0.0.1 -c "CREATE DATABASE carfixdb;"
psql -U postgres -h 127.0.0.1 -d carfixdb -f carfix_seed.sql
```

Строка подключения: `AvaloniaApp/appsettings.json`

```
Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=123
```

### 2. Приложение

```powershell
cd AvaloniaApp
dotnet restore
dotnet run
```

Должно открыться окно **AvaloniaApp** с формой входа.

Подробная инструкция также в файле `CarFix_Guide.docx`.

---

## Вход по ролям

На экране логина введи логин и пароль:

| Роль | Логин | Пароль | Что открывается |
|------|--------|--------|-----------------|
| Администратор (руководитель) | `admin` | `admin` | Заказы, выручка, склад, профиль |
| Менеджер | `manager` | `manager` | Клиенты, машины, заказы, расписание, механики, профиль |
| Механик | `mech1` | `mech1` | Мои заказы, запрос на склад, профиль |

Дополнительные механики: `mech2`…`mech5` (пароль = логин).  
Ещё менеджер: `manager2` / `manager2`.

### Как работать за Админа
1. Войти как `admin` / `admin`
2. Слева: иконки заказов, выручки, склада, профиля
3. Выручка = сумма платежей − сумма закупок запчастей

### Как работать за Менеджера
1. Войти как `manager` / `manager`
2. **Клиенты** → «Регистрация клиента»
3. **Машины** → «Добавить машину клиенту»
4. **Заказы** → двойной клик по строке → назначить механика (если занят в это время — отказ)
5. **Расписание** → выбрать слот и заменить механика (нельзя, если у текущего есть активные заказы в день)
6. **Механики** → увольнение только без активных заказов

### Как работать за Механика
1. Войти как `mech1` / `mech1`
2. **Мои заказы** → двойной клик
3. **Забрать себе** — нужны навыки (`UserService`) и смена в расписании
4. **Передать другому** — список механиков со навыками и свободным временем
5. Завершение услуги блокируется, если на складе мало запчастей
6. Можно добавить комментарий на любом этапе
7. **Запрос на склад** — форма недостающих позиций

---

## Локальный запуск на macOS

### Вариант A — Docker
См. раздел «Быстрый старт». UI открывается в браузере. Docker Desktop должен быть запущен.

### Вариант B — нативно
1. Установи .NET 9 SDK и PostgreSQL (`brew install postgresql@16` или Docker только для БД)
2. Создай БД `carfixdb` и выполни `Infastructure/Sql/carfix_seed.sql`
3. В `AvaloniaApp/appsettings.json` укажи хост/пароль
4. Запуск:

```bash
cd AvaloniaApp
dotnet run
```

Если БД только в Docker:

```
Host=localhost;Port=5433;Database=carfixdb;Username=postgres;Password=123
```

---

## Если окно не появляется / «проект не запускается»

1. **PostgreSQL не запущен** — в Windows: службы → `postgresql-x64-…` → Запустить.
2. **БД пустая** — выполни `Infastructure\Sql\setup_db.ps1`.
3. **Неверный пароль в appsettings.json** — должно быть `postgres` / `123`, база `carfixdb`.
4. **Docker** — без запущенного Docker Desktop `docker compose` не работает; используй локальный `dotnet run`.
5. Нужен **.NET 9 SDK** (`dotnet --list-sdks`).

При ошибке БД приложение показывает диалог с текстом ошибки (не зависает без окна).

---

## Структура решения

```
AvaloniaApp/       — UI (Avalonia)
Domain/            — модели
Infastructure/     — EF Core, репозитории, SQL
docker-compose.yml
Dockerfile
CarFix_Guide.docx  — инструкция Word
README.md          — этот файл
DOCKER.md          — детали Docker
```

---

## Полезные команды Docker

```bash
# пересобрать (Docker Desktop уже запущен)
docker compose up -d --build

# логи приложения
docker logs -f carfix-app

# только база
docker compose up -d db

# остановка
docker compose down
```
