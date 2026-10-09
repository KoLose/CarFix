# CarFix

Система управления автосервисом на Avalonia (.NET 9) + PostgreSQL.

Поддерживаемые роли: **Admin**, **Manager**, **Mechanic**.

---

## Быстрый старт на любом устройстве (Windows / macOS)

Нужен только [Docker Desktop](https://www.docker.com/products/docker-desktop/).

```bash
git clone <url-репозитория>
cd CarFix
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

## Локальный запуск без Docker UI (Windows)

### Требования
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL (учётка по умолчанию: `postgres` / `123`)

### База данных

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

### Приложение

```powershell
cd AvaloniaApp
dotnet run
```

---

## Локальный запуск на macOS

### Вариант A — Docker (рекомендуется)
См. раздел «Быстрый старт» выше. UI открывается в браузере.

### Вариант B — нативно
1. Установи .NET 9 SDK и PostgreSQL (`brew install postgresql@16` / Docker только для БД)
2. Подними БД и выполни `Infastructure/Sql/carfix_seed.sql`
3. В `appsettings.json` укажи свой хост/пароль
4. Запуск:

```bash
cd AvaloniaApp
dotnet run
```

Если используешь только Docker-БД с хоста:

```
Host=localhost;Port=5433;Database=carfixdb;Username=postgres;Password=123
```

---

## Структура решения

```
AvaloniaApp/     — UI (Avalonia)
Domain/          — модели
Infastructure/   — EF Core, репозитории, SQL
docker-compose.yml
Dockerfile
```

Подробнее про контейнеры: [DOCKER.md](DOCKER.md)

---

## Полезные команды Docker

```bash
# пересобрать
docker compose up -d --build

# логи приложения
docker logs -f carfix-app

# только база
docker compose up -d db
```
