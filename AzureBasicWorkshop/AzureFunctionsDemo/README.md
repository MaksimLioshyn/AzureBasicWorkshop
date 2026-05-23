# Azure Functions Demo — .NET 8 Isolated Process

Учебный проект, демонстрирующий **Serverless-вычисления** с Azure Functions на C# (.NET 8).

## Архитектура

```
[Client / Browser]
       │
       ▼ HTTP
[OrderHttpFunction]  ─── POST /orders ──► [Storage Queue "orders-processing"]
     GET/PATCH                                        │
       │                                              ▼
       │                              [OrderQueueFunction] ─► обработка заказа
       │
       ▼ Service Bus Queue
[OrderServiceBusFunction]  ──► Complete / Abandon / DeadLetter
       │
       ▼ Event Hubs (≤100 events/batch)
[TelemetryEventHubFunction] ──► агрегация IoT-телеметрии

       ⏱ Timer (каждый день в 08:00)
[DailyReportFunction] ──► сводный отчёт по заказам

       🔄 Cosmos DB Change Feed
[OrderChangeFeedFunction] ──► [Service Bus Topic "order-notifications"]
                                      │
                              ┌───────┴────────┐
                              ▼                ▼
                        email-sub        analytics-sub
```

## Что демонстрирует проект

| Функция | Триггер | Ключевая концепция |
|---|---|---|
| `OrderHttpFunction` | HTTP | REST API + Multi-Output Binding (HTTP + Queue) |
| `OrderQueueFunction` | Storage Queue | DequeueCount, Poison Messages, Idempotency |
| `OrderServiceBusFunction` | Service Bus | Complete / Abandon / DeadLetter (ручное управление) |
| `TelemetryEventHubFunction` | Event Hubs | Batch обработка, Consumer Groups, метаданные |
| `DailyReportFunction` | Timer | CRON, IsPastDue, RunOnStartup |
| `OrderChangeFeedFunction` | Cosmos DB | Change Feed + Output Binding → Service Bus Topic |

## Технологии

- **.NET 8** — Isolated Process (рекомендуемый режим)
- **Azure Functions v4**
- **Application Insights** — телеметрия и трассировка
- **Dependency Injection** — полноценный DI как в ASP.NET Core
- **Azure Storage Queues** — простые фоновые очереди
- **Azure Service Bus** — надёжный корпоративный брокер
- **Azure Event Hubs** — потоковая обработка событий
- **Azure Cosmos DB** — Change Feed триггер
- **Redis Cache** — поддержка через `IDistributedCache`

## Предварительные требования

1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
2. **[Azure Functions Core Tools v4](https://learn.microsoft.com/azure/azure-functions/functions-run-local)**
   ```powershell
   npm install -g azure-functions-core-tools@4 --unsafe-perm true
   ```
3. **[Azurite](https://learn.microsoft.com/azure/storage/common/storage-use-azurite)** — эмулятор Azure Storage
   ```powershell
   npm install -g azurite
   # или через Docker:
   docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
   ```

## Локальный запуск

### 1. Запустить Azurite (в отдельном терминале)
```powershell
azurite --silent --location .azurite --debug .azurite/debug.log
```

### 2. Восстановить зависимости и запустить
```powershell
cd AzureFunctionsDemo
dotnet restore
func start
```

После запуска вы увидите список зарегистрированных функций:
```
Functions:
    GetOrders:                  [GET] http://localhost:7071/api/orders
    GetOrderById:               [GET] http://localhost:7071/api/orders/{id}
    CreateOrder:                [POST] http://localhost:7071/api/orders
    CancelOrder:                [PATCH] http://localhost:7071/api/orders/{id}/cancel
    ProcessOrderFromQueue:      queueTrigger
    GenerateDailyReport:        timerTrigger (RunOnStartup=true в DEBUG)
```

## Тестирование функций

### HTTP Trigger — REST API

```powershell
# GET все заказы
curl http://localhost:7071/api/orders

# GET заказ по ID
curl http://localhost:7071/api/orders/order-001

# POST создать заказ (автоматически кладёт сообщение в очередь)
curl -X POST http://localhost:7071/api/orders `
  -H "Content-Type: application/json" `
  -d '{
    "customerId": "customer-test",
    "customerEmail": "test@example.com",
    "items": [
      { "productId": "prod-1", "name": "Azure T-Shirt", "quantity": 2, "unitPrice": 29.99 }
    ],
    "notes": "Подарочная упаковка"
  }'

# PATCH отменить заказ
curl -X PATCH http://localhost:7071/api/orders/order-002/cancel
```

### Или используйте файл `http-requests.http` в VS Code (расширение REST Client)

### Queue Trigger — автоматически после POST

После успешного `POST /api/orders` сообщение попадает в очередь `orders-processing`,
и `OrderQueueFunction` начнёт обработку автоматически через ~2 секунды.

В логах вы увидите:
```
[Information] Queue message received: OrderId=..., Action=process, Attempt=#1
[Information] Checking payment...
[Information] Reserving inventory...
[Information] Order ... completed successfully
```

### Timer Trigger — запускается при старте (в DEBUG режиме)

`GenerateDailyReport` запускается немедленно при `func start` благодаря `RunOnStartup = true`.

## Понимание ключевых концепций

### Multi-Output Binding (OrderHttpFunction.CreateOrder)
```
POST /api/orders
     │
     ├─► HTTP 201 Created ───────────────────────── (HttpResponse)
     └─► Queue Message ──────────────────────────── (ProcessingMessage)
                │
                ▼
         [OrderQueueFunction]
```
В C# это реализуется через класс-контейнер `CreateOrderOutput` с атрибутами:
- `[HttpResult]` — HTTP-ответ
- `[QueueOutput("orders-processing")]` — сообщение в очередь

### Storage Queue vs Service Bus

| | Storage Queue | Service Bus Queue |
|---|---|---|
| Установка | Ничего (часть Storage) | Отдельный ресурс |
| Max сообщение | 64 KB | 256 KB – 100 MB |
| DLQ | "‑poison" очередь | Встроен |
| Ручное управление | Нет | `ServiceBusMessageActions` |
| Подходит для | Простые фоновые задачи | Надёжные бизнес-операции |

### Poison Messages (Queue)
1. Функция выбрасывает исключение → сообщение возвращается в очередь
2. `dequeueCount` растёт (виден в BindingContext)
3. При `dequeueCount > maxDequeueCount` (5 по умолчанию) → уходит в `orders-processing-poison`
4. Из "poison" очереди можно читать отдельной функцией или вручную анализировать

## Структура проекта

```
AzureFunctionsDemo/
├── AzureFunctionsDemo.csproj   # зависимости NuGet
├── Program.cs                  # точка входа, регистрация DI
├── host.json                   # конфигурация хоста Functions
├── local.settings.json         # строки подключения (НЕ в git!)
├── .gitignore
│
├── Models/
│   ├── Order.cs                # Order, OrderItem, OrderStatus, CreateOrderDto, OrderQueueMessage
│   └── TelemetryEvent.cs       # IoT-событие для Event Hubs
│
├── Services/
│   ├── IOrderService.cs        # интерфейс
│   └── OrderService.cs         # in-memory реализация (в prod → CosmosDB/SQL)
│
└── Functions/
    ├── OrderHttpFunction.cs         # HTTP Trigger: CRUD + Multi-Output Binding
    ├── OrderQueueFunction.cs        # Storage Queue Trigger: DequeueCount, Idempotency
    ├── OrderServiceBusFunction.cs   # Service Bus: Complete/Abandon/DeadLetter
    ├── TelemetryEventHubFunction.cs # Event Hubs: batch, аномалии, агрегация
    ├── DailyReportFunction.cs       # Timer: CRON, IsPastDue, RunOnStartup
    └── OrderChangeFeedFunction.cs   # Cosmos DB Change Feed + SB Topic output
```

## Деплой в Azure

```powershell
# Логин
az login

# Создание Function App (Consumption Plan — бесплатно для первых 1M вызовов)
az group create --name rg-functions-demo --location eastus

az storage account create --name safunctionsdemo --resource-group rg-functions-demo `
  --location eastus --sku Standard_LRS

az functionapp create --resource-group rg-functions-demo --consumption-plan-location eastus `
  --runtime dotnet-isolated --runtime-version 8 --functions-version 4 `
  --name func-orders-demo --storage-account safunctionsdemo

# Публикация
func azure functionapp publish func-orders-demo
```

> **Совет**: Для prod используйте Managed Identity вместо строк подключения:
> `az functionapp identity assign --name func-orders-demo --resource-group rg-functions-demo`
