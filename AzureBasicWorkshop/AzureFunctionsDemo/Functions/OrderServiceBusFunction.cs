using System.Text.Json;
using AzureFunctionsDemo.Models;
using AzureFunctionsDemo.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsDemo.Functions;

// ══════════════════════════════════════════════════════════════════════════
//  SERVICE BUS TRIGGER — надёжная обработка с полным контролем над сообщением
//
//  В отличие от Storage Queue, Service Bus даёт:
//    ✦ Ручное подтверждение (Complete / Abandon / DeadLetter)
//    ✦ Встроенный Dead Letter Queue (DLQ) — не нужна отдельная "‑poison" очередь
//    ✦ DeliveryCount — аналог DequeueCount, но встроен в сообщение
//    ✦ ApplicationProperties — пользовательские метаданные сообщения
//    ✦ Sessions — гарантированный порядок обработки по группам
//
//  AutoCompleteMessages = false → управляем сообщением вручную через messageActions
// ══════════════════════════════════════════════════════════════════════════
public sealed class OrderServiceBusFunction
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderServiceBusFunction> _logger;

    public OrderServiceBusFunction(IOrderService orderService, ILogger<OrderServiceBusFunction> logger)
    {
        _orderService = orderService;
        _logger       = logger;
    }

    [Function(nameof(ProcessOrderFromServiceBus))]
    public async Task ProcessOrderFromServiceBus(
        // AutoCompleteMessages = false — отключаем автоподтверждение
        // чтобы вручную вызывать Complete / Abandon / DeadLetter
        [ServiceBusTrigger(
            queueName: "orders-queue",
            Connection = "ServiceBusConnection",
            AutoCompleteMessages = false)]
        ServiceBusReceivedMessage message,

        // ServiceBusMessageActions — API для управления судьбой сообщения
        ServiceBusMessageActions messageActions,
        FunctionContext context)
    {
        _logger.LogInformation(
            "Service Bus message: Id={MessageId}, DeliveryCount={Count}, Subject={Subject}",
            message.MessageId, message.DeliveryCount, message.Subject);

        // ─── Чтение пользовательских свойств сообщения ────────────────────────
        var priority = message.ApplicationProperties.TryGetValue("priority", out var p)
            ? p?.ToString() ?? "normal"
            : "normal";

        // ─── Десериализация payload ────────────────────────────────────────────
        OrderQueueMessage? payload;
        try
        {
            payload = message.Body.ToObjectFromJson<OrderQueueMessage>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Cannot deserialize message {MessageId} — sending to DLQ", message.MessageId);

            // ❌ DEAD LETTER — неправильный формат, повторная доставка бессмысленна.
            // Сообщение перемещается в очередь "$DeadLetterQueue", откуда его
            // можно прочитать и проанализировать позже.
            await messageActions.DeadLetterMessageAsync(
                message,
                propertiesToModify: null,
                deadLetterReason: "InvalidPayload",
                deadLetterErrorDescription: ex.Message);
            return;
        }

        if (payload is null)
        {
            await messageActions.DeadLetterMessageAsync(
                message, propertiesToModify: null,
                deadLetterReason: "NullPayload",
                deadLetterErrorDescription: "Body was null");
            return;
        }

        try
        {
            var order = await _orderService.GetByIdAsync(payload.OrderId);

            if (order is null)
            {
                // Бизнес-ошибка: заказ не существует — DLQ, не повторяем
                _logger.LogWarning("Order {OrderId} not found → DLQ", payload.OrderId);
                await messageActions.DeadLetterMessageAsync(
                    message,
                    propertiesToModify: null,
                    deadLetterReason: "OrderNotFound",
                    deadLetterErrorDescription: $"Order {payload.OrderId} does not exist");
                return;
            }

            // ─── Обработка ────────────────────────────────────────────────────
            await _orderService.UpdateStatusAsync(payload.OrderId, OrderStatus.Processing);
            await SimulateReliableProcessingAsync(payload.OrderId, priority);
            await _orderService.UpdateStatusAsync(payload.OrderId, OrderStatus.Completed);

            // ✅ COMPLETE — работа успешна, сообщение удаляется из очереди навсегда
            await messageActions.CompleteMessageAsync(message);

            _logger.LogInformation(
                "Order {OrderId} completed via Service Bus (priority={Priority})",
                payload.OrderId, priority);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // ⚠ ABANDON — временная ошибка (БД недоступна, таймаут и т.д.)
            // Сообщение становится видимым снова через lockDuration и будет доставлено повторно.
            // При DeliveryCount > MaxDeliveryCount → автоматически уходит в DLQ.
            _logger.LogWarning(ex,
                "Transient error processing order {OrderId} — abandoning (will retry)",
                payload?.OrderId);
            await messageActions.AbandonMessageAsync(message);
        }
    }

    private async Task SimulateReliableProcessingAsync(string orderId, string priority)
    {
        var delay = priority == "high"
            ? TimeSpan.FromMilliseconds(50)
            : TimeSpan.FromMilliseconds(200);

        _logger.LogDebug(
            "Processing order {OrderId} with priority={Priority} (delay={Delay}ms)",
            orderId, priority, delay.TotalMilliseconds);

        await Task.Delay(delay);
    }
}
