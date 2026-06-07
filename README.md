# WarehouseAPI

REST API бэкенд для системы складского учёта. Разработан в рамках учебной практики как серверная часть десктопного приложения на WPF (C#).

## Содержание

- [Технологии](#технологии)
- [Cистемные требования](#Системные-требования-для-«Складской-учёт»)
- [Архитектура](#архитектура)
- [Структура проекта](#структура-проекта)
- [База данных](#база-данных)
- [API — эндпоинты](#api--эндпоинты)
- [Запуск проекта](#запуск-проекта)
- [Переменные окружения](#переменные-окружения)
- [Регламент действий администратора в случае ошибок](#Регламент-действий-администратора-в-случае)

---
## 🧭 Навигация (боковое меню)
- **📦 Товары** — каталог номенклатуры
- **📊 Остатки** — актуальные складские запасы
- **🏢 Контрагенты** — справочник поставщиков/покупателей
- **📞 Контакты** — контактные лица контрагентов
- **🔄 Операции** — приходные/расходные документы
- **📋 Детали операций** — строки документов (товары + количество + цена)
- **📈 Аналитика** — отчёты и визуализация складских показателей (в разработке)

## 🔘 Основные действия
- **➕ Добавить** — открывает универсальную форму добавления записи
- **🗑 Удалить** — удаляет выбранную строку (с подтверждением)
- **🔄 Обновить** — перезагружает текущую таблицу
- **🔍 Поиск** — фильтрация по тексту (доступен для товаров и операций)
- **🎯 Фильтр** — дополнительные фильтры (для товаров: единицы измерения; для операций: тип)
  
## Технологии

| Технология | Версия | Назначение |
|---|---|---|
| .NET | 10.0 | Платформа |
| ASP.NET Core Web API | 10.0 | HTTP-сервер |
| Entity Framework Core | 10.0 | ORM |
| Npgsql EF Core PostgreSQL | 10.0 | Провайдер PostgreSQL |
| PostgreSQL | 15+ | База данных |
| Swashbuckle (Swagger) | 10.1 | Документация API |

---

## Системные требования для «Складской учёт»

### Клиентское приложение (WinForms)
- Windows 10 версии 1607 или новее
- Процессор: 1.5 ГГц, 2 ядра
- Оперативная память: 1 ГБ (рекомендуется 4 ГБ)
- Свободное место: 500 МБ
- Установленный .NET 8.0 Desktop Runtime
- Разрешение экрана: 1024×768

### Серверная часть
- ASP.NET Core 8.0
- PostgreSQL 15 или новее
- 2 ГБ ОЗУ, 2 ядра CPU (минимально)
- 10 ГБ свободного места на диске
- Стабильное сетевое соединение

### Сеть
- HTTP/HTTPS доступ к API
- Латентность не более 200 мс

---

## Архитектура


Проект построен по слоистой архитектуре с разделением ответственности:

```
HTTP-запрос
    │
    ▼
ExceptionHandlingMiddleware   ← перехватывает все необработанные исключения
    │
    ▼
ValidationFilter              ← проверяет DTO через Data Annotations
    │
    ▼
Controller                    ← принимает запрос, возвращает HTTP-ответ
    │
    ▼
Service                       ← бизнес-логика, транзакции
    │
    ▼
Repository                    ← доступ к базе данных через EF Core
    │
    ▼
AppDbContext → PostgreSQL
```

**Принципы:**
- Каждый слой взаимодействует только со слоем ниже через интерфейс
- Все зависимости регистрируются через DI (`AddScoped`)
- Контроллеры не содержат бизнес-логики и `try/catch` — исключения обрабатываются глобально

---

## Структура проекта

```
WarehouseAPI/
├── Controllers/                  # HTTP-эндпоинты
│   ├── ProductsController.cs
│   ├── StockController.cs
│   ├── OperationsController.cs
│   ├── CounterpartiesController.cs
│   └── AnalyticsController.cs
│
├── Services/                     # Бизнес-логика
│   ├── Interfaces/
│   │   ├── IProductService.cs
│   │   ├── ICounterpartyService.cs
│   │   ├── IOperationService.cs
│   │   ├── IStockService.cs
│   │   └── IAnalyticsService.cs
│   ├── ProductService.cs
│   ├── CounterpartyService.cs
│   ├── OperationService.cs
│   ├── StockService.cs
│   └── AnalyticsService.cs
│
├── Repositories/                 # Доступ к данным
│   ├── Interfaces/
│   │   ├── IProductRepository.cs
│   │   ├── ICounterpartyRepository.cs
│   │   ├── IOperationRepository.cs
│   │   ├── IStockRepository.cs
│   │   └── IAnalyticsRepository.cs
│   ├── ProductRepository.cs
│   ├── CounterpartyRepository.cs
│   ├── OperationRepository.cs
│   ├── StockRepository.cs
│   └── AnalyticsRepository.cs
│
├── Models/                       # Сущности EF Core
│   ├── Product.cs
│   ├── Stock.cs
│   ├── Operation.cs
│   ├── OperationItem.cs
│   ├── Counterparty.cs
│   └── Contact.cs
│
├── DTOs/                         # Объекты передачи данных
│   ├── Products/
│   │   ├── ProductCreateDto.cs
│   │   ├── ProductUpdateDto.cs
│   │   └── ProductResponseDto.cs
│   ├── Counterparties/
│   │   ├── CounterpartyCreateDto.cs
│   │   ├── CounterpartyUpdateDto.cs
│   │   ├── CounterpartyResponseDto.cs
│   │   └── ContactDto.cs
│   ├── Operations/
│   │   ├── OperationCreateDto.cs   # IncomeCreateDto / SaleCreateDto / ...
│   │   ├── OperationResponseDto.cs
│   │   ├── OperationItemDto.cs
│   │   └── OperationFilterDto.cs
│   ├── Stock/
│   │   └── StockResponseDto.cs
│   └── Analytics/
│       ├── TopProductDto.cs
│       ├── TurnoverDto.cs
│       └── LowStockDto.cs
│
├── Data/
│   └── AppDbContext.cs            # DbContext, конфигурация связей
│
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs  # Глобальная обработка ошибок
│
├── Filters/
│   └── ValidationFilter.cs        # Глобальная валидация DTO
│
├── Migrations/                    # EF Core миграции
├── appsettings.json
└── Program.cs
```

---

## База данных

### Схема

```
Products ──────────── Stock
   │                (1 : 1)
   │
   └──── OperationItems ──── Operations ──── Counterparties
              (N)               (1)               (1)
                                                   │
                                                Contacts
                                                  (N)
```

### Таблицы

#### `Products` — товары
| Поле | Тип | Описание |
|---|---|---|
| Id | int | Первичный ключ |
| Name | varchar(200) | Наименование |
| Article | varchar(50) | Артикул (уникальный) |
| Unit | varchar(20) | Единица измерения (шт, кг, л) |
| Price | decimal | Цена |
| CreatedAt | timestamp | Дата создания |

#### `Stocks` — остатки на складе
| Поле | Тип | Описание |
|---|---|---|
| Id | int | Первичный ключ |
| ProductId | int | FK → Products |
| Quantity | decimal | Текущий остаток |
| UpdatedAt | timestamp | Время последнего изменения |

#### `Operations` — складские операции
| Поле | Тип | Описание |
|---|---|---|
| Id | int | Первичный ключ |
| Type | enum | `Income` / `Sale` / `Transfer` / `WriteOff` |
| Date | timestamp | Дата и время операции |
| Comment | text | Комментарий / причина |
| CounterpartyId | int? | FK → Counterparties (опционально) |

#### `OperationItems` — строки операции
| Поле | Тип | Описание |
|---|---|---|
| Id | int | Первичный ключ |
| OperationId | int | FK → Operations |
| ProductId | int | FK → Products |
| Quantity | decimal | Количество |
| Price | decimal | Цена на момент операции |

#### `Counterparties` — контрагенты
| Поле | Тип | Описание |
|---|---|---|
| Id | int | Первичный ключ |
| Name | varchar(200) | Наименование |
| Type | enum | `Client` / `Supplier` / `Company` |
| Inn | varchar(12) | ИНН (опционально) |
| Address | varchar(300) | Адрес (опционально) |

#### `Contacts` — контакты контрагентов
| Поле | Тип | Описание |
|---|---|---|
| Id | int | Первичный ключ |
| CounterpartyId | int | FK → Counterparties |
| Name | varchar(100) | Имя контакта |
| Phone | varchar | Телефон |
| Email | varchar | Email |

---

## API — эндпоинты

### Товары `/api/products`

| Метод | Путь | Описание |
|---|---|---|
| `GET` | `/api/products` | Список всех товаров с текущими остатками |
| `GET` | `/api/products/{id}` | Товар по ID |
| `POST` | `/api/products` | Создать товар |
| `PUT` | `/api/products/{id}` | Обновить товар |
| `DELETE` | `/api/products/{id}` | Удалить товар (нельзя если есть остаток) |

<details>
<summary>Пример запроса POST /api/products</summary>

```json
{
  "name": "Молоко 1л",
  "article": "MLK-001",
  "unit": "шт",
  "price": 89.90
}
```

</details>

---

### Остатки `/api/stock`

| Метод | Путь | Описание |
|---|---|---|
| `GET` | `/api/stock` | Остатки по всем товарам |
| `GET` | `/api/stock/{productId}` | Остаток по конкретному товару |

---

### Складские операции `/api/operations`

| Метод | Путь | Описание |
|---|---|---|
| `GET` | `/api/operations/history` | История операций с фильтрами |
| `GET` | `/api/operations/{id}` | Операция по ID |
| `POST` | `/api/operations/income` | Приход товара |
| `POST` | `/api/operations/sale` | Реализация (продажа) |
| `POST` | `/api/operations/transfer` | Перемещение |
| `POST` | `/api/operations/writeoff` | Списание |

**Фильтры для GET /api/operations/history:**

| Параметр | Тип | Описание |
|---|---|---|
| `from` | DateTime | Начало периода |
| `to` | DateTime | Конец периода |
| `type` | enum | `Income` / `Sale` / `Transfer` / `WriteOff` |
| `counterpartyId` | int | Фильтр по контрагенту |
| `productId` | int | Фильтр по товару |
| `page` | int | Страница (по умолчанию 1) |
| `pageSize` | int | Размер страницы (по умолчанию 20) |

<details>
<summary>Пример запроса POST /api/operations/income</summary>

```json
{
  "counterpartyId": 1,
  "comment": "Плановая поставка",
  "items": [
    { "productId": 3, "quantity": 100, "price": 45.00 },
    { "productId": 7, "quantity": 50,  "price": 120.00 }
  ]
}
```

</details>

> ⚠️ Все операции выполняются в транзакции. При недостаточном остатке для `Sale`, `Transfer`, `WriteOff` — возвращается `400 Bad Request` с описанием ошибки, изменений в БД не происходит.

---

### Контрагенты `/api/counterparties`

| Метод | Путь | Описание |
|---|---|---|
| `GET` | `/api/counterparties` | Все контрагенты |
| `GET` | `/api/counterparties?type=Client` | Фильтрация по типу |
| `GET` | `/api/counterparties/clients` | Только клиенты |
| `GET` | `/api/counterparties/suppliers` | Только поставщики |
| `GET` | `/api/counterparties/companies` | Только компании |
| `GET` | `/api/counterparties/{id}` | Контрагент по ID |
| `POST` | `/api/counterparties` | Создать контрагента с контактами |
| `PUT` | `/api/counterparties/{id}` | Обновить контрагента и контакты |
| `DELETE` | `/api/counterparties/{id}` | Удалить контрагента |

---

### Аналитика `/api/analytics`

| Метод | Путь | Описание |
|---|---|---|
| `GET` | `/api/analytics/top-products` | Топ продаваемых товаров за период |
| `GET` | `/api/analytics/turnover` | Обороты (приход / продажи / списания) за период |
| `GET` | `/api/analytics/low-stock` | Товары с остатком ниже минимума |

**Параметры:**

| Эндпоинт | Параметры |
|---|---|
| `top-products` | `from`, `to`, `limit` (1–100, по умолч. 10) |
| `turnover` | `from`, `to` — возвращает разбивку по дням |
| `low-stock` | `minQuantity` (по умолч. 5) |

---

### Служебные эндпоинты

| Метод | Путь | Описание |
|---|---|---|
| `GET` | `/` | Информация о сервисе |
| `GET` | `/health` | Проверка доступности |
| `GET` | `/swagger` | Swagger UI (только в Development) |

---

## Запуск проекта

### Требования

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 15+](https://www.postgresql.org/download/)

### 1. Клонировать репозиторий

```bash
git clone <url-репозитория>
cd WarehouseAPI
```

### 2. Настроить строку подключения

Открой `appsettings.json` и заполни `DefaultConnection`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=warehouse_db;Username=postgres;Password=твой_пароль"
  }
}
```

> Базу данных `warehouse_db` нужно создать заранее в pgAdmin или через `psql`:
> ```sql
> CREATE DATABASE warehouse_db;
> ```

### 3. Применить миграции

```bash
dotnet ef database update
```

После выполнения все таблицы будут созданы автоматически.

### 4. Запустить API

```bash
dotnet run
```

API будет доступен по адресам:
- HTTP: `http://localhost:5023`
- HTTPS: `https://localhost:7150`
- Swagger UI: `http://localhost:5023/swagger`

---

## Запуск проекта с помощью Docker
Чтобы быстро запустить проект в контейнерах, выполните следующие шаги:

### 1. Убедитесь, что у вас установлены Docker и Docker Compose
Инструкция по установке Docker
Инструкция по установке Docker Compose
### 2. Подготовьте docker-compose.yml и Dockerfile
В корне проекта должны находиться файлы:
docker-compose.yml (описание нескольких сервисов, например, базы данных и API)
Dockerfile (описание сборки вашего приложения)
Если эти файлы еще не созданы, ниже приведу их примеры.

### 3. Пример docker-compose.yml

```bash
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: warehouse_db
      POSTGRES_USER: your_username
      POSTGRES_PASSWORD: your_password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build: .
    depends_on:
      - postgres
    environment:
      - "ConnectionStrings:DefaultConnection=Host=postgres;Database=warehouse_db;Username=your_username;Password=your_password"
    ports:
      - "5000:80"

volumes:
  postgres_data:
```
### 4. Пример Dockerfile для ASP.NET Core проекта
```dockerfile
dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WarehouseAPI/WarehouseAPI.csproj", "WarehouseAPI/"]
RUN dotnet restore "WarehouseAPI/WarehouseAPI.csproj"
COPY . .
WORKDIR "/src/WarehouseAPI"
RUN dotnet build "WarehouseAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WarehouseAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WarehouseAPI.dll"]
```
Обязательно замените your_username и your_password на ваши данные.

### 5. Запуск контейнеров
В терминале, в папке с файлами, выполните команду:

```bash
docker-compose up -d
```
Это запустит ваше приложение и базу данных в фоновом режиме.

### 6. Доступ к приложению
После запуска API будет доступен по адресу:
http://localhost:5000

(или по тому порту, который вы указали).

### 7. Остановка и удаление контейнеров
Чтобы остановить:

```bash
docker-compose down
```

---

## Переменные окружения

| Переменная | Описание |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Development` — включает Swagger и расширенные логи |
| `ConnectionStrings__DefaultConnection` | Строка подключения к PostgreSQL |
| `secret.env` | Файл, содержащий скрытые параметры входа к вашей бд. |

---

## Формат ответа при ошибке

Все ошибки возвращаются в едином формате:

```json
{
  "status": 400,
  "message": "Недостаточно товара ID=3: на складе 5, запрошено 10",
  "path": "/api/operations/sale",
  "traceId": "0HN7K2J1L4Q3A:00000001"
}
```

| HTTP-код | Причина |
|---|---|
| `400` | Ошибка валидации DTO или бизнес-правил (нехватка остатка и т.д.) |
| `404` | Запрошенный ресурс не найден |
| `500` | Внутренняя ошибка сервера |

## Регламент действий администратора в случае

### 🔴 Сценарий А: Упала база данных (PostgreSQL)
#### Признаки:
- API возвращает ошибки 500 Internal Server Error или Connection refused

- WinForms клиент пишет: Не удалось подключиться к серверу

- В логах API: Npgsql.NpgsqlException: Exception while connecting

---
#### Последовательность действий администратора:
| Шаг |	Действие |	Команда / Проверка| Ожидаемый результат|
|---|
|1|	Проверить, работает ли служба PostgreSQL|	`sudo systemctl status postgresql` (Linux) `Get-Service postgresql*` (PowerShell)|	Должно быть running|
|2|	Если служба остановлена — запустить	|`sudo systemctl start postgresql` `Start-Service postgresql`|	Служба запустилась|
|3|	Проверить подключение к БД|	psql -h localhost -U postgres -d warehouse_db -c "SELECT 1"|	Должен вернуть ?column? → 1|
|4|	Проверить, не блокирует ли фаервол порт 5432|	`telnet localhost 5432` Или `Test-NetConnection -Port 5432 localhost`|	Соединение устанавливается|
|5|	Если не подключается — посмотреть логи|	sudo tail -100 /var/log/postgresql/postgresql-*.log	|Искать FATAL или ERROR|
|6|	После восстановления — перезапустить API|	`docker compose restart api` (если в Docker) `sudo systemctl restart warehouse-api` (если как служба)|	API снова отвечает

### Если PostgreSQL не запускается
```bash
# Шаг А1: Посмотреть подробную ошибку
sudo journalctl -u postgresql -n 50 --no-pager


# Шаг А2: Проверить целостность данных
sudo -u postgres pg_checksums -c /var/lib/postgresql/data

# Шаг А3: Попробовать запустить в одном пользовательском режиме (только для экспертов)
sudo -u postgres postgres --single -D /var/lib/postgresql/data
Крайний случай: восстановление из резервной копии
Остановить API (чтобы не писались новые данные): docker compose stop api
```
### Восстановить БД из бэкапа:

```bash
gunzip -c backup.sql.gz | psql -U postgres -d warehouse_db
```
Запустить API: `docker compose start api`

⚠️ Регулярно делайте бэкапы! Минимум — раз в день через pg_dump.

### 💾 Сценарий Б: Переполнился диск
#### Признаки
- API падает с ошибкой No space left on device

- WinForms клиент зависает при попытке сохранить операцию

- PostgreSQL не может записывать данные и аварийно завершается

- df -h показывает 100% использования на каком-то разделе

### Последовательность действий администратора
|Шаг	|Действие	|Команда	|Что ищем|
|---|
|1|	Определить, какой диск переполнен|	`df -h`|	Раздел с Use% = 100%|
|2|	Найти самые большие папки|	`sudo du -sh /* 2>/dev/null -> sort -hr -> head -20`|	Логи, временные файлы, дампы|
|3|	Очистить системные логи	sudo| `journalctl --vacuum-size=500M` (Linux systemd)| Или удалить старые .log в /var/log/	Освободить 500 МБ - 2 ГБ|
|4|	Очистить логи PostgreSQL|	`sudo find /var/lib/postgresql/data/log -name "*.log" -mtime +7 -delete`|	Удалить логи старше 7 дней|
|5|	Очистить логи вашего API|	`find /путь/к/логам/api -name "*.log" -mtime +7 -delete`|	Если вы пишете логи в файлы|
|6|	Проверить Docker (если используете)|	`docker system prune -a -f`|	Удалить неиспользуемые образы, контейнеры, тома|
|7|	Перезапустить PostgreSQL|	`sudo systemctl restart postgresql`|	Должен запуститься без ошибок|
|8|	Перезапустить API|	`docker compose restart api`	|API отвечает на запросы|

### Что делать, если очистка не помогла
Вариант 1: Перенести данные PostgreSQL на другой диск:
```
# Остановить PostgreSQL
sudo systemctl stop postgresql

# Скопировать данные на новый диск
sudo rsync -av /var/lib/postgresql/ /mnt/newdisk/postgresql/

# Изменить конфиг на новый путь
sudo nano /etc/postgresql/15/main/postgresql.conf
# Изменить data_directory = '/mnt/newdisk/postgresql/15/main

# Запустить PostgreSQL
sudo systemctl start postgresql
```
### Вариант 2: Расширить диск (если виртуальная машина/облако) — через панель управления хостингом.

### Профилактика переполнения диска
|Что настроить|	Как|	Почему важно
|---|
|Ротация логов PostgreSQL|	В postgresql.conf: log_rotation_age = 1d, log_rotation_size = 100MB|Логи не разрастаются бесконтрольно|
|Ротация логов вашего API|	Использовать Serilog с rolling file sink или настройки логирования в .NET|	Логи делятся по дням/размерам
|Мониторинг свободного места|	df -h раз в день (через cron/Task Scheduler)|	Узнаете о проблеме до критического порога
|Автоочистка Docker|	docker system prune -f раз в неделю в cron|	Docker volume — частая причина неожиданных "съеденных" гигабайт|