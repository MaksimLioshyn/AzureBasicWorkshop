using AzureFunctionsDemo.Models;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsDemo.Services;

/// <summary>
/// Реализация сервиса заказов с потокобезопасным in-memory хранилищем.
///
/// В продакшне здесь был бы CosmosClient (или DbContext для SQL/PostgreSQL):
/// <code>
///   private readonly CosmosContainer _container;
///   public OrderService(CosmosClient cosmos)
///       => _container = cosmos.GetContainer("ShopDb", "orders");
/// </code>
/// </summary>
public sealed class OrderService : IOrderService
{
    // Статический словарь — живёт всё время работы приложения
    // В реальном приложении — это CosmosDB / SQL / PostgreSQL
    private static readonly Dictionary<string, Order> _store = new();
    private static readonly object _syncRoot = new();

    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
        SeedDemoData();
    }

    public Task<Order> CreateAsync(Order order, CancellationToken ct = default)
    {
        order.Id        = Guid.NewGuid().ToString();
        order.CreatedAt = DateTimeOffset.UtcNow;
        order.Status    = OrderStatus.Pending;
        order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        lock (_syncRoot)
            _store[order.Id] = order;

        _logger.LogInformation(
            "Order {OrderId} created for customer {CustomerId} | Total: {Total:C}",
            order.Id, order.CustomerId, order.TotalAmount);

        return Task.FromResult(order);
    }

    public Task<Order?> GetByIdAsync(string orderId, CancellationToken ct = default)
    {
        Order? order;
        lock (_syncRoot)
            _store.TryGetValue(orderId, out order);

        return Task.FromResult(order);
    }

    public Task<IReadOnlyList<Order>> GetByCustomerAsync(string customerId, CancellationToken ct = default)
    {
        List<Order> result;
        lock (_syncRoot)
        {
            result = _store.Values
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }
        return Task.FromResult<IReadOnlyList<Order>>(result);
    }

    public Task<Order> UpdateStatusAsync(string orderId, OrderStatus status, CancellationToken ct = default)
    {
        lock (_syncRoot)
        {
            if (!_store.TryGetValue(orderId, out var order))
                throw new KeyNotFoundException($"Order '{orderId}' not found");

            order.Status    = status;
            order.UpdatedAt = DateTimeOffset.UtcNow;

            _logger.LogInformation(
                "Order {OrderId} status changed to {Status}", orderId, status);

            return Task.FromResult(order);
        }
    }

    public Task<IReadOnlyList<Order>> GetRecentAsync(int top = 50, CancellationToken ct = default)
    {
        List<Order> result;
        lock (_syncRoot)
        {
            result = _store.Values
                .OrderByDescending(o => o.CreatedAt)
                .Take(top)
                .ToList();
        }
        return Task.FromResult<IReadOnlyList<Order>>(result);
    }

    // ─── Демо-данные для быстрого старта ─────────────────────────────────────
    private static void SeedDemoData()
    {
        if (_store.Count > 0) return;

        var orders = new[]
        {
            new Order
            {
                Id            = "order-001",
                CustomerId    = "customer-alice",
                CustomerEmail = "alice@example.com",
                Status        = OrderStatus.Completed,
                Items         =
                [
                    new OrderItem { ProductId = "prod-1", Name = "Azure T-Shirt",     Quantity = 2, UnitPrice = 29.99m },
                    new OrderItem { ProductId = "prod-2", Name = "Cosmos DB Mug",     Quantity = 1, UnitPrice = 14.99m },
                ],
                TotalAmount   = 74.97m,
                CreatedAt     = DateTimeOffset.UtcNow.AddDays(-3),
                UpdatedAt     = DateTimeOffset.UtcNow.AddDays(-2),
            },
            new Order
            {
                Id            = "order-002",
                CustomerId    = "customer-bob",
                CustomerEmail = "bob@example.com",
                Status        = OrderStatus.Processing,
                Items         =
                [
                    new OrderItem { ProductId = "prod-3", Name = "Functions Sticker Pack", Quantity = 3, UnitPrice = 4.99m },
                ],
                TotalAmount   = 14.97m,
                CreatedAt     = DateTimeOffset.UtcNow.AddHours(-5),
            },
            new Order
            {
                Id            = "order-003",
                CustomerId    = "customer-alice",
                CustomerEmail = "alice@example.com",
                Status        = OrderStatus.Pending,
                Items         =
                [
                    new OrderItem { ProductId = "prod-4", Name = "Redis Cache Hoodie", Quantity = 1, UnitPrice = 59.99m },
                ],
                TotalAmount   = 59.99m,
                CreatedAt     = DateTimeOffset.UtcNow.AddMinutes(-30),
            },
        };

        foreach (var o in orders)
            _store[o.Id] = o;
    }
}
