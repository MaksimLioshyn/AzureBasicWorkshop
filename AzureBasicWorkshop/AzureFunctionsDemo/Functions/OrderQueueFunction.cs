using AzureFunctionsDemo.Models;
using AzureFunctionsDemo.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsDemo.Functions;

// ══════════════════════════════════════════════════════════════════════════
//  STORAGE QUEUE TRIGGER — асинхронная обработка заказов
//
//  Концепции:
//    ✦ QueueTrigger автоматически десериализует JSON → OrderQueueMessage
//    ✦ DequeueCount — счётчик попыток доставки (получаем из BindingContext)
//    ✦ Poison Messages — при превышении maxDequeueCount (host.json → queues →
//      maxDequeueCount = 5) Azure автоматически перемещает сообщение в очередь
//      "orders-processing-poison"
//    ✦ Idempotency — функция безопасна при дублированной доставке
//
//  Как это работает:
//    POST /api/orders
//      → [QueueOutput] кладёт сообщение в "orders-processing"
//        → [QueueTrigger] забирает сообщение и вызывает эту функцию
// ══════════════════════════════════════════════════════════════════════════
public sealed class OrderQueueFunction
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderQueueFunction> _logger;

    public OrderQueueFunction(IOrderService orderService, ILogger<OrderQueueFunction> logger)
    {
        _orderService = orderService;
        _logger       = logger;
    }

    [Function(nameof(ProcessOrderFromQueue))]
    public async Task ProcessOrderFromQueue(
        // Очередь "orders-processing" в Azure Storage (или Azurite локально)
        // POCO автоматически десериализуется из JSON
        [QueueTrigger("orders-processing", Connection = "AzureWebJobsStorage")]
        OrderQueueMessage message,
        FunctionContext context)
    {
        // ─── Получаем DequeueCount из метаданных ─────────────────────────────
        // DequeueCount = сколько раз Azure пытался доставить это сообщение.
        // Если предыдущие вызовы функции выбрасывали исключение — счётчик растёт.
        // При DequeueCount > maxDequeueCount → сообщение → "...‑poison" очередь.
        var dequeueCount = GetDequeueCount(context);

        _logger.LogInformation(
            "Queue message received: OrderId={OrderId}, Action={Action}, Attempt=#{Attempt}",
            message.OrderId, message.Action, dequeueCount);

        // ─── Idempotency check ────────────────────────────────────────────────
        // Сообщение может прийти дважды (at-least-once delivery).
        // Проверяем текущий статус заказа перед обработкой.
        var order = await _orderService.GetByIdAsync(message.OrderId);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found — skipping (idempotent)", message.OrderId);
            return; // не выбрасываем исключение — сообщение будет удалено из очереди
        }

        if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled)
        {
            _logger.LogInformation(
                "Order {OrderId} already in terminal state {Status} — idempotency skip",
                message.OrderId, order.Status);
            return;
        }

        // ─── Основная обработка ───────────────────────────────────────────────
        await _orderService.UpdateStatusAsync(message.OrderId, OrderStatus.Processing);

        // Симуляция реальной бизнес-логики:
        // - проверка оплаты
        // - резервирование товаров на складе
        // - уведомление склада
        await SimulateBusinessLogicAsync(message.OrderId);

        await _orderService.UpdateStatusAsync(message.OrderId, OrderStatus.Completed);

        _logger.LogInformation(
            "Order {OrderId} successfully processed and completed", message.OrderId);
    }

    // ─── Вспомогательные методы ───────────────────────────────────────────────

    /// <summary>
    /// Читает DequeueCount из метаданных привязки.
    /// В Isolated Process BindingContext.BindingData содержит все метаданные триггера.
    /// </summary>
    private static int GetDequeueCount(FunctionContext context)
    {
        if (context.BindingContext.BindingData.TryGetValue("dequeueCount", out var raw)
            && int.TryParse(raw?.ToString(), out var count))
        {
            return count;
        }
        return 1;
    }

    private async Task SimulateBusinessLogicAsync(string orderId)
    {
        _logger.LogDebug("Checking payment for order {OrderId}...", orderId);
        await Task.Delay(150); // имитация запроса к платёжной системе

        _logger.LogDebug("Reserving inventory for order {OrderId}...", orderId);
        await Task.Delay(100); // имитация запроса к складу

        _logger.LogDebug("Notifying warehouse for order {OrderId}...", orderId);
        await Task.Delay(50);
    }
}
