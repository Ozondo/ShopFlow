# ShopFlow — Фаза 1

REST API на ASP.NET Core 8 с хранением данных в JSON-файлах.

**Шаблон без реализации** — ученик пишет Infrastructure, Controllers и регистрацию DI самостоятельно.

## Быстрый старт

```bash
cd src/ShopFlow.Api
dotnet restore
dotnet run
```

Swagger UI: http://localhost:5080/swagger (пока без endpoints — их добавит ученик)

## Структура проекта

```
src/ShopFlow.Api/
├── Program.cs              # точка входа (CreateHostBuilder + Startup)
├── Startup.cs              # ConfigureServices / Configure (DI, Swagger, pipeline)
├── Domain/
│   ├── Products/Models/    # Product
│   └── Orders/Models/      # Order, OrderItem, OrderStatus
├── Application/            # TODO: DTOs, интерфейсы репозиториев
├── Infrastructure/         # TODO: JsonFileStore, Json*Repository
├── Controllers/            # TODO: ProductsController, OrdersController
└── Data/                   # products.json, orders.json
```
---

## Задание: что должно работать по итогу Задания 1

### Товары (`/api/products`)

| Операция | Поведение |
|----------|-----------|
| GET список | Все товары из JSON |
| GET по id | 200 или 404 |
| POST | Создание с новым Guid, 201 + Location |
| PUT | Обновление существующего, 404 если нет |
| DELETE | 204 или 404 |

### Заказы (`/api/orders`)

| Операция | Поведение |
|----------|-----------|
| GET список | Все заказы |
| GET по id | 200 или 404 |
| POST | Создание заказа с проверками (см. ниже) |
| PATCH status | Смена статуса с валидацией переходов |

**Бизнес-правила заказов:**

1. Каждый `ProductId` в заказе должен существовать в каталоге
2. `Quantity` не больше текущего `Stock` (на Фазе 1 stock можно не списывать — только проверять)
3. В заказе сохраняется **snapshot** — имя товара и цена на момент покупки, а не ссылка на «живую» цену
4. Статус по умолчанию — `New`
5. Допустимые переходы: `New → Processing → Shipped`, отмена из `New` или `Processing`. Из `Shipped` назад нельзя

### Swagger

- Все endpoints задокументированы: HTTP-коды, описания методов, схемы request/response
- Swagger UI доступен в Development на `/swagger`
- API можно показать и протестировать без Postman

### Обработка ошибок

- Невалидный body → 400
- Несуществующий ресурс → 404
- Нарушение бизнес-правила (нет товара, мало stock, неверный переход статуса) → 400 с понятным сообщением
- Логирование ключевых операций (создание/удаление товара, смена статуса заказа)

### Тесты

- **Unit-тесты** репозиториев и `JsonFileStore` — на временной папке, не на боевых JSON
- **Integration-тесты** API — HTTP-запросы через `WebApplicationFactory`, проверка кодов и тела ответа
- Минимум: **5 unit + 3 integration**, все зелёные локально через `dotnet test`

---

## Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
