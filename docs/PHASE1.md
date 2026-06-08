# Фаза 1 — Proxy API + JSON

**Цель:** поднять ASP.NET Core Web API без БД, освоить слоёную архитектуру, DI, Swagger и тесты.

**Срок:** 1.5–2 недели.

---

## Что уже есть в шаблоне (не трогать / использовать как контракт)

| Слой | Содержимое | Статус |
|------|------------|--------|
| `Program.cs` | `CreateHostBuilder` + `UseStartup<Startup>()` | ✅ готово |
| `Startup.cs` | Controllers, Swagger, pipeline — **DI закомментирован** | ✅ каркас |
| `Domain/` | `Product`, `Order`, `OrderItem`, `OrderStatus` | ✅ готово |
| `Application/DTOs/` | Request-модели с DataAnnotations | ✅ готово |
| `Application/Interfaces/` | `IProductRepository`, `IOrderRepository` | ✅ готово |
| `Infrastructure/` | Классы-заглушки с `NotImplementedException` | ☐ реализуй |
| `Controllers/` | Пустые контроллеры с TODO | ☐ реализуй |
| `Data/` | Пустые `products.json`, `orders.json` | ☐ добавь seed |

---

## Архитектура слоёв

```
Controllers/          ← HTTP, коды ответов, маппинг DTO ↔ Domain
    ↓
Application/          ← интерфейсы репозиториев, DTO (без реализации)
    ↓
Infrastructure/       ← JsonFileStore, Json*Repository (реализация)
    ↓
Data/*.json
```

**Domain** не зависит ни от чего — только сущности.

---

## Шаг 0. Запуск каркаса (30 мин)

```bash
cd src/ShopFlow.Api
dotnet restore
dotnet run
```

### Чеклист

- [ ] Проект собирается (`dotnet build`)
- [ ] Swagger открывается: http://localhost:5080/swagger
- [ ] В Swagger пока нет endpoints — это нормально
- [ ] Ученик прочитал `Program.cs` и `Startup.cs`, понимает разницу `ConfigureServices` / `Configure`

---

## Шаг 1. JsonFileStore (2–3 ч)

Файл: `Infrastructure/JsonFileStore.cs`

### Задача

Реализовать потокобезопасное чтение/запись JSON-массивов.

### Чеклист

- [ ] Конструктор принимает `IOptions<JsonStorageOptions>` и `IWebHostEnvironment`
- [ ] Путь к папке: `ContentRootPath` + `DataDirectory` из конфига
- [ ] `ReadAllAsync<T>` — если файла нет, вернуть пустой список
- [ ] `WriteAllAsync<T>` — перезаписать файл целиком
- [ ] `SemaphoreSlim` для защиты от concurrent write
- [ ] `System.Text.Json` с `PropertyNamingPolicy = JsonNamingPolicy.CamelCase`

### Подсказка (не копировать целиком — разобрать и написать самому)

```csharp
private static readonly JsonSerializerOptions SerializerOptions = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};
```

### Проверка

- [ ] Unit-тест: записал 2 объекта → прочитал → count == 2
- [ ] Unit-тест: файл не существует → пустой список

---

## Шаг 2. JsonProductRepository (2–3 ч)

Файл: `Infrastructure/JsonProductRepository.cs`

### Задача

Реализовать `IProductRepository` поверх `JsonFileStore` и `products.json`.

### Чеклист

- [ ] Primary constructor: `JsonProductRepository(JsonFileStore fileStore)`
- [ ] `GetAllAsync` — прочитать весь файл
- [ ] `GetByIdAsync` — LINQ `FirstOrDefault`
- [ ] `CreateAsync` — read all → add → write all
- [ ] `UpdateAsync` — find index → replace → write (null если не найден)
- [ ] `DeleteAsync` — remove → write (false если не найден)

### Seed-данные

Добавь в `Data/products.json` 2–3 товара:

```json
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "name": "Mechanical Keyboard",
    "category": "Peripherals",
    "price": 129.99,
    "stock": 25
  }
]
```

### Проверка

- [ ] ≥ 5 unit-тестов репозитория (temp directory, не коммитить мусор в Data/)

---

## Шаг 3. JsonOrderRepository (1–2 ч)

Файл: `Infrastructure/JsonOrderRepository.cs`

Аналогично products, файл `orders.json`, метод `UpdateStatusAsync` через record `with`:

```csharp
var updated = current with { Status = status };
```

### Чеклист

- [ ] CRUD-методы реализованы
- [ ] `UpdateStatusAsync` не создаёт новый заказ, а обновляет существующий

---

## Шаг 4. DI в Startup (30 мин)

Файл: `Startup.cs` → `ConfigureServices`

### Чеклист

- [ ] Раскомментировать и дописать регистрацию:

```csharp
services.Configure<JsonStorageOptions>(
    Configuration.GetSection(JsonStorageOptions.SectionName));
services.AddSingleton<JsonFileStore>();
services.AddScoped<IProductRepository, JsonProductRepository>();
services.AddScoped<IOrderRepository, JsonOrderRepository>();
```

- [ ] Добавить `using` для Application и Infrastructure
- [ ] Проект собирается, DI резолвит зависимости

---

## Шаг 5. ProductsController (3–4 ч)

Файл: `Controllers/ProductsController.cs`

### Endpoints

| Метод | Route | Код |
|-------|-------|-----|
| GET | `/api/products` | 200 |
| GET | `/api/products/{id}` | 200 / 404 |
| POST | `/api/products` | 201 / 400 |
| PUT | `/api/products/{id}` | 200 / 404 |
| DELETE | `/api/products/{id}` | 204 / 404 |

### Чеклист

- [ ] DI: `ProductsController(IProductRepository repository)`
- [ ] POST маппит `CreateProductRequest` → `Product` с `Guid.NewGuid()`
- [ ] POST возвращает `CreatedAtAction` с Location header
- [ ] `[ProducesResponseType(...)]` на каждом методе — для Swagger
- [ ] XML-комментарии `///` на public methods

### Первый endpoint (начни с него)

```csharp
[HttpGet]
[ProducesResponseType(typeof(IReadOnlyList<Product>), StatusCodes.Status200OK)]
public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(CancellationToken cancellationToken)
{
    var products = await _repository.GetAllAsync(cancellationToken);
    return Ok(products);
}
```

### Проверка через Swagger

- [ ] Все 5 endpoints видны в UI
- [ ] POST → GET by id возвращает созданный товар
- [ ] Невалидный body → 400

---

## Шаг 6. OrdersController (4–5 ч)

Файл: `Controllers/OrdersController.cs`

### Бизнес-правила

1. Товар в заказе должен существовать
2. `Quantity <= Stock` (stock не списываем в Фазе 1)
3. `UnitPrice` — snapshot цены на момент заказа
4. Статус по умолчанию: `New`
5. Переходы: `New→Processing→Shipped`, `New|Processing→Cancelled`

### Endpoints

| Метод | Route |
|-------|-------|
| GET | `/api/orders` |
| GET | `/api/orders/{id}` |
| POST | `/api/orders` |
| PATCH | `/api/orders/{id}/status` |

### Чеклист

- [ ] DI: `IOrderRepository` + `IProductRepository`
- [ ] POST собирает `OrderItem` с именем и ценой из Product
- [ ] PATCH валидирует переходы статусов → 400 при невалидном
- [ ] Все endpoints в Swagger

---

## Шаг 7. Swagger — полировка (1 ч)

В `Startup.ConfigureServices` → `AddSwaggerGen`:

- [ ] `GenerateDocumentationFile` в `.csproj`
- [ ] `IncludeXmlComments` для описаний в UI
- [ ] `UseInlineDefinitionsForEnums()` — OrderStatus в PATCH

---

## Шаг 8. Тесты (4–5 ч)

```bash
dotnet new xunit -n ShopFlow.UnitTests -o tests/ShopFlow.UnitTests
dotnet sln add tests/ShopFlow.UnitTests/ShopFlow.UnitTests.csproj
dotnet add tests/ShopFlow.UnitTests reference src/ShopFlow.Api/ShopFlow.Api.csproj
dotnet add tests/ShopFlow.UnitTests package FluentAssertions
dotnet add tests/ShopFlow.UnitTests package Microsoft.AspNetCore.Mvc.Testing
```

### Чеклист

- [ ] ≥ 5 unit-тестов (JsonFileStore + репозитории)
- [ ] ≥ 3 integration-тестов (`WebApplicationFactory<Program>`)
- [ ] Для integration: добавить в конец `Program.cs` → `public partial class Program { }`

---

## Definition of Done

| # | Критерий |
|---|----------|
| 1 | Слои Domain / Application / Infrastructure / Controllers соблюдены |
| 2 | DI зарегистрирован в `Startup.ConfigureServices` |
| 3 | CRUD products + orders работает |
| 4 | Swagger документирует все endpoints |
| 5 | ≥ 5 unit + ≥ 3 integration тестов |
| 6 | Ученик объясняет поток запроса от Controller до JSON-файла |

---

## Порядок домашек

| # | Задача | Часы |
|---|--------|------|
| 1 | JsonFileStore + тесты | 3–4 |
| 2 | JsonProductRepository + seed + GET endpoint | 3–4 |
| 3 | ProductsController CRUD полностью | 3–4 |
| 4 | JsonOrderRepository + OrdersController | 4–5 |
| 5 | Swagger XML + integration tests | 4–5 |
