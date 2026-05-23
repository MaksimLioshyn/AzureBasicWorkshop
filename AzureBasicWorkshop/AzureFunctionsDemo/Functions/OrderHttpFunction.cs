using System.Net;
using System.Text.Json;
using AzureFunctionsDemo.Models;
using AzureFunctionsDemo.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsDemo.Functions;

// ══════════════════════════════════════════════════════════════════════════
//  HTTP TRIGGER — REST API для управления заказами
//
//  Маршруты:
//    GET  /api/orders              → список последних заказов
//    GET  /api/orders/{id}         → заказ по ID
//    POST /api/orders              → создать заказ  ← Multi-Output Binding
//    PATCH /api/orders/{id}/cancel → отменить заказ
//
//  Ключевые концепции:
//    ✦ AuthorizationLevel.Anonymous  — без ключа (удобно для разработки)
//    ✦ HttpRequestData / HttpResponseData — вместо HttpRequest/IActionResult
//    ✦ Конструкторная DI — точно так же как в ASP.NET Core
//    ✦ CreateOrderOutput — пример Multi-Output Binding (HTTP + Queue)
// ══════════════════════════════════════════════════════════════════════════
public sealed class OrderHttpFunction
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderHttpFunction> _logger;

    // Конструкторная инъекция — работает так же, как в ASP.NET Core контроллере
    public OrderHttpFunction(IOrderService orderService, ILogger<OrderHttpFunction> logger)
    {
        _orderService = orderService;
        _logger       = logger;
    }

    // ─── GET /api/orders ──────────────────────────────────────────────────────
    [Function(nameof(GetOrders))]
    public async Task<HttpResponseData> GetOrders(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders")]
        HttpRequestData req,
        FunctionContext context)
    {
        _logger.LogInformation("GET /api/orders");

        var orders   = await _orderService.GetRecentAsync(top: 20);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(orders);
        return response;
    }

    // ─── GET /api/orders/{id} ─────────────────────────────────────────────────
    // Параметр маршрута {id} передаётся напрямую как аргумент метода
    [Function(nameof(GetOrderById))]
    public async Task<HttpResponseData> GetOrderById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/{id}")]
        HttpRequestData req,
        string id,
        FunctionContext context)
    {
        _logger.LogInformation("GET /api/orders/{Id}", id);

        var order = await _orderService.GetByIdAsync(id);
        if (order is null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteAsJsonAsync(new { error = $"Order '{id}' not found" });
            return notFound;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(order);
        return response;
    }

    // ─── POST /api/orders ─────────────────────────────────────────────────────
    // ДЕМО: Multi-Output Binding
    //   Функция ОДНОВРЕМЕННО возвращает:
    //     1. HTTP 201 Created  ──────────── ответ клиенту
    //     2. OrderQueueMessage ──────────── в Azure Storage Queue (фоновая обработка)
    //   Runtime Azure Functions сам доставит каждое из значений куда нужно.
    [Function(nameof(CreateOrder))]
    public async Task<CreateOrderOutput> CreateOrder(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")]
        HttpRequestData req,
        FunctionContext context)
    {
        CreateOrderDto? dto;
        try
        {
            dto = await JsonSerializer.DeserializeAsync<CreateOrderDto>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Invalid JSON in request body");
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteAsJsonAsync(new { error = "Invalid JSON format" });
            return new CreateOrderOutput { HttpResponse = bad };
        }

        if (dto is null || string.IsNullOrWhiteSpace(dto.CustomerId) || dto.Items.Count == 0)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteAsJsonAsync(new { error = "customerId and at least one item are required" });
            return new CreateOrderOutput { HttpResponse = bad };
        }

        var order = await _orderService.CreateAsync(new Order
        {
            CustomerId    = dto.CustomerId,
            CustomerEmail = dto.CustomerEmail,
            Items         = dto.Items,
            Notes         = dto.Notes,
        });

        _logger.LogInformation("Order {OrderId} created", order.Id);

        var response = req.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Location", $"/api/orders/{order.Id}");
        await response.WriteAsJsonAsync(order);

        // ✅ Возвращаем HTTP-ответ И сообщение в очередь одновременно
        return new CreateOrderOutput
        {
            HttpResponse     = response,
            ProcessingMessage = new OrderQueueMessage
            {
                OrderId    = order.Id,
                CustomerId = order.CustomerId,
                Action     = "process",
            }
        };
    }

    // ─── PATCH /api/orders/{id}/cancel ────────────────────────────────────────
    [Function(nameof(CancelOrder))]
    public async Task<HttpResponseData> CancelOrder(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "orders/{id}/cancel")]
        HttpRequestData req,
        string id,
        FunctionContext context)
    {
        _logger.LogInformation("PATCH /api/orders/{Id}/cancel", id);
        try
        {
            var order    = await _orderService.UpdateStatusAsync(id, OrderStatus.Cancelled);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(order);
            return response;
        }
        catch (KeyNotFoundException)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteAsJsonAsync(new { error = $"Order '{id}' not found" });
            return notFound;
        }
    }
}

// ══════════════════════════════════════════════════════════════════════════
//  Multi-Output Binding Return Type
//
//  Когда функция должна писать сразу в несколько сервисов Azure,
//  мы создаём класс-контейнер, где каждое свойство помечено атрибутом привязки.
//  Azure Functions runtime сам маршрутизирует каждое не-null значение.
// ══════════════════════════════════════════════════════════════════════════
public sealed class CreateOrderOutput
{
    /// <summary>HTTP-ответ клиенту. [HttpResult] — маркер для runtime.</summary>
    [HttpResult]
    public required HttpResponseData HttpResponse { get; set; }

    /// <summary>
    /// Сообщение в Azure Storage Queue "orders-processing".
    /// Будет автоматически подхвачено <see cref="OrderQueueFunction"/>.
    /// Если null — сообщение не отправляется (например, при ошибке валидации).
    /// </summary>
    [QueueOutput("orders-processing", Connection = "AzureWebJobsStorage")]
    public OrderQueueMessage? ProcessingMessage { get; set; }
}
