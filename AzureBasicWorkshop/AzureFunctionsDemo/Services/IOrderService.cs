using AzureFunctionsDemo.Models;

namespace AzureFunctionsDemo.Services;

/// <summary>Интерфейс сервиса для работы с заказами.</summary>
public interface IOrderService
{
    /// <summary>Создаёт новый заказ и возвращает его с присвоенным ID.</summary>
    Task<Order> CreateAsync(Order order, CancellationToken ct = default);

    /// <summary>Возвращает заказ по ID, или null если не найден.</summary>
    Task<Order?> GetByIdAsync(string orderId, CancellationToken ct = default);

    /// <summary>Возвращает все заказы покупателя.</summary>
    Task<IReadOnlyList<Order>> GetByCustomerAsync(string customerId, CancellationToken ct = default);

    /// <summary>Обновляет статус заказа. Выбрасывает KeyNotFoundException если заказ не найден.</summary>
    Task<Order> UpdateStatusAsync(string orderId, OrderStatus status, CancellationToken ct = default);

    /// <summary>Возвращает последние <paramref name="top"/> заказов (для отчётов).</summary>
    Task<IReadOnlyList<Order>> GetRecentAsync(int top = 50, CancellationToken ct = default);
}
