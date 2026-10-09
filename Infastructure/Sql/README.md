# CarFix database

- `carfix_seed.sql` — схема + ~20 записей в основные таблицы
- `setup_db.ps1` — пересоздание локальной БД (`postgres` / `123`)
- `db_source.sql` — исходный дамп

Локально: `Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=123`  
Docker: порт `5433`, та же учётка (см. `DOCKER.md` в корне).
