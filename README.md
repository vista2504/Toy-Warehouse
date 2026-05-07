# Toy-Warehouse
Тема проэкта складский учет
Функционал:
		Приход Товара(поступление товара)
		Реализация Товара(продажа)
		Перемещение Товара
		Списание Товара
		Клиенты
			Контакты
			Компании
		Товары
		Аналитика

## Backend (WarehouseAPI) — запуск каркаса

### Требования
- Установленный **.NET SDK** (версия должна поддерживать `net10.0`).

Проверка:

```powershell
dotnet --version
```

### Запуск
В корне репозитория:

```powershell
cd WarehouseAPI
dotnet restore
dotnet run
```

После запуска API будет доступен на адресах, которые выводятся в консоль.

### Проверка
- Тестовый эндпоинт: `GET /weatherforecast`
- Файл для быстрых запросов: `WarehouseAPI/WarehouseAPI.http`

### OpenAPI
В режиме Development включена выдача OpenAPI по маршруту:
- `GET /openapi/v1.json`

Примечание: если при первом запуске будут проблемы с HTTPS dev-сертификатом, выполните:

```powershell
dotnet dev-certs https --trust
```
