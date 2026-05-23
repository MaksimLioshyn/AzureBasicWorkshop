using System.Text.Json;
using AzureFunctionsDemo.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsDemo.Functions;

// ══════════════════════════════════════════════════════════════════════════
//  COSMOS DB CHANGE FEED TRIGGER — реакция на изменения в базе данных
//
//  Change Feed — это поток всех INSERT и UPDATE операций в контейнере.
//  Функция вызывается каждый раз, когда в Cosmos DB появляются новые
//  или изменённые документы.
//
//  Концепции:
//    ✦ Lease Container — хранит позицию чтения (checkpoint) Change Feed
//    ✦ CreateLeaseContainerIfNotExists — авто-создание "leases" контейнера
//    ✦ IReadOnlyList<Order> — пакет изменённых документов (POCO из JSON)
//    ✦ Output Binding → Service Bus Topic — уведомляем подписчиков
//
//  Сценарий:
//    Заказ обновляется в Cosmos DB
//      → Change Feed вызывает эту функцию
//        → При статусе Completed отправляем уведомление в Service Bus Topic
//          → Подписчики (email, push, analytics) получают событие
// ══════════════════════════════════════════════════════════════════════════
public sealed class OrderChangeFeedFunction
{
    private readonly ILogger<OrderChangeFeedFunction> _logger;

    public OrderChangeFeedFunction(ILogger<OrderChangeFeedFunction> logger)
        => _logger = logger;

    // [ServiceBusOutput] — Output Binding: return string[] отправит каждый элемент
    // как отдельное сообщение в Service Bus Topic "order-notifications"
    [Function(nameof(ProcessOrderChanges))]
    [ServiceBusOutput("order-notifications", Connection = "ServiceBusConnection")]
    public Task<string[]> ProcessOrderChanges(
        [CosmosDBTrigger(
            databaseName: "ShopDb",
            containerName: "orders",
            Connection = "CosmosDBConnection",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true,
            FeedPollDelay = 1000)]          // проверяем изменения каждую секунду
        IReadOnlyList<Order> changedOrders,
        FunctionContext context)
    {
        _logger.LogInformation(
            "Change Feed: {Count} order document(s) changed", changedOrders.Count);

        // ─── Формируем уведомления для завершённых заказов ───────────────────
        var notifications = new List<string>();

        foreach (var order in changedOrders)
        {
            _logger.LogInformation(
                "Changed document: OrderId={Id}, Status={Status}, CustomerId={Customer}",
                order.Id, order.Status, order.CustomerId);

            // Реагируем только на завершённые заказы
            if (order.Status == OrderStatus.Completed)
            {
                // Каждый элемент массива → отдельное сообщение в Service Bus Topic
                // Все подписчики топика (email-sub, push-sub, analytics-sub) получат его
                var notification = JsonSerializer.Serialize(new
                {
                    eventType     = "OrderCompleted",
                    orderId       = order.Id,
                    customerId    = order.CustomerId,
                    customerEmail = order.CustomerEmail,
                    totalAmount   = order.TotalAmount,
                    itemCount     = order.Items.Count,
                    completedAt   = order.UpdatedAt ?? DateTimeOffset.UtcNow,
                });

                notifications.Add(notification);

                _logger.LogInformation(
                    "Notification queued for completed order {OrderId}", order.Id);
            }
        }

        // Возвращаем массив → каждый элемент уйдёт в Service Bus Topic через Output Binding
        // Если список пуст — ни одно сообщение не отправляется
        return Task.FromResult(notifications.ToArray());
    }
}
