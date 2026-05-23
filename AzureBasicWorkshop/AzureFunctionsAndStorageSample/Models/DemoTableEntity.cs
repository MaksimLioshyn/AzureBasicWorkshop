using Azure;
using Azure.Data.Tables;

namespace AzureFunctionsAndStorageSample.Models;

/// <summary>
/// Сущность для хранения в Azure Table Storage.
/// Реализует ITableEntity - обязательный интерфейс для работы с Table Storage.
/// </summary>
public sealed class DemoTableEntity : ITableEntity
{
    /// <summary>
    /// Ключ партиции - первая часть составного первичного ключа.
    /// Определяет физическое размещение данных в хранилище.
    /// </summary>
    public string PartitionKey { get; set; } = string.Empty;

    /// <summary>
    /// Ключ строки - вторая часть составного первичного ключа.
    /// Должен быть уникальным в рамках партиции.
    /// </summary>
    public string RowKey { get; set; } = string.Empty;

    /// <summary>
    /// Произвольное поле для хранения пользовательских данных.
    /// В реальных проектах здесь могут быть любые свойства: числа, даты, JSON и т.д.
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Временная метка последнего изменения записи.
    /// Автоматически управляется Azure Table Storage.
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// ETag для оптимистичной конкурентности (optimistic concurrency).
    /// Позволяет избежать конфликтов при одновременном изменении записи разными клиентами.
    /// </summary>
    public ETag ETag { get; set; }
}