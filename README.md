# CarFix

Автосервис на Avalonia.  
Для разработки — **PostgreSQL**. Для скринов другу — **портативный ZIP со встроенной БД** (ничего ставить не нужно).

Логины: `admin`/`admin` · `manager`/`manager` · `mech1`/`mech1`

---

## Другу на Mac / Windows — самый лёгкий способ (для скринов)

Не нужен Docker, не нужен PostgreSQL, не нужен Terminal.

1. Возьми ZIP из папки `dist/` (или Release на GitHub):
   - **Mac на чипе M1/M2/M3/M4** → `CarFix-Mac-AppleSilicon.zip`
   - **Mac Intel** → `CarFix-Mac-Intel.zip`
   - **Windows** → `CarFix-Windows.zip`
2. Распакуй ZIP
3. Запусти:
   - Windows: `AvaloniaApp.exe`
   - Mac: `AvaloniaApp` (если macOS ругается: правой кнопкой → **Открыть** → Открыть)
4. Войди (`admin` / `admin`) и сделай скрины

База лежит **внутри папки** в файле `carfix.db` — создаётся сама при первом запуске с тестовыми данными.

Собрать ZIP заново на своём ПК:
```powershell
.\scripts\publish-portable.ps1
```

---

## Разработка с PostgreSQL (как раньше)

### Mac
```bash
chmod +x scripts/start-mac.sh
./scripts/start-mac.sh
```

### Windows
```powershell
.\scripts\start-windows.ps1
```

Приложение само найдёт PostgreSQL или, если его нет, включит встроенный `carfix.db`.

---

## Роли

| Роль | Логин | Что смотреть на скринах |
|------|--------|-------------------------|
| Админ | `admin` | заказы, выручка, склад |
| Менеджер | `manager` | клиенты, машины, заказы, расписание |
| Механик | `mech1` | мои заказы, запрос на склад |

---

## Важно

| Режим | База |
|--------|------|
| Портативный ZIP (`portable.marker`) | файл `carfix.db` рядом с программой |
| Обычный запуск + есть PostgreSQL | PostgreSQL (`carfixdb`) |
| Обычный запуск, PostgreSQL нет | автоматически `carfix.db` |

Docker не обязателен. Подробности: [DOCKER.md](DOCKER.md)
