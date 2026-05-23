using System.Text.Json.Serialization;

namespace AzureFunctionsDemo.Models;

// ══════════════════════════════════════════════════════════════════════════
//  Модели предметной области — Заказ
//  JsonPropertyName нужен для CosmosDB (lowercase имена полей в документе)
// ══════════════════════════════════════════════════════════════════════════

/// <summary>Заказ — основная сущность приложения.</summary>
public sealed class Order
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("customerEmail")]
    public string CustomerEmail { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<OrderItem> Items { get; set; } = [];

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("status")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset? UpdatedAt { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

/// <summary>Позиция в заказе.</summary>
public sealed class OrderItem
{
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonIgnore]
    public decimal SubTotal => Quantity * UnitPrice;
}

/// <summary>Статус заказа.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Pending,     // создан, ожидает обработки
    Processing,  // обрабатывается
    Shipped,     // отправлен
    Completed,   // завершён
    Cancelled,   // отменён
    Failed       // ошибка обработки
}

/// <summary>DTO для создания нового заказа (тело POST-запроса от клиента).</summary>
public sealed class CreateOrderDto
{
    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("customerEmail")]
    public string CustomerEmail { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<OrderItem> Items { get; set; } = [];

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

/// <summary>
/// Сообщение, помещаемое в Azure Storage Queue / Service Bus
/// для асинхронной фоновой обработки заказа.
/// </summary>
public sealed class OrderQueueMessage
{
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>Действие: "process", "ship", "notify".</summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = "process";

    [JsonPropertyName("enqueuedAt")]
    public DateTimeOffset EnqueuedAt { get; set; } = DateTimeOffset.UtcNow;
}
