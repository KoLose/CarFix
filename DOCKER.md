# CarFix Docker

## Запуск всего стека

```bash
docker compose up -d --build
```

| Сервис | Адрес |
|--------|--------|
| UI (noVNC) | http://localhost:6080/vnc.html |
| PostgreSQL (Docker) | `localhost:5433` |

Учётка БД: `postgres` / `123`, база `carfixdb`.

## Только БД в Docker

```bash
docker compose up -d db
```

## Локальный PostgreSQL (этот ПК)

Уже можно использовать: `postgres` / `123`, база `carfixdb` (заполнена ~20 записями на таблицу).

Пересоздать сид:

```powershell
.\Infastructure\Sql\setup_db.ps1
```

## Логины приложения

- Admin: `admin` / `admin`
- Manager: `manager` / `manager`
- Mechanic: `mech1` / `mech1` … `mech5` / `mech5`
