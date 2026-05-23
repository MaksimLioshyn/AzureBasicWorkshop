namespace AzureFunctionsAndStorageSample.Models;

/// <summary>
/// Модель запроса для сохранения записи в Table Storage.
/// Используется при HTTP POST запросах к /api/tables/save
/// </summary>
public sealed class TableRecordRequest
{
    /// <summary>
    /// Ключ партиции - используется для логической группировки записей.
    /// Все записи с одинаковым PartitionKey хранятся вместе для быстрого доступа.
    /// Пример: "Users", "Orders", "Products"
    /// </summary>
    public string? PartitionKey { get; init; }

    /// <summary>
    /// Уникальный ключ строки внутри партиции.
    /// Комбинация PartitionKey + RowKey образует первичный ключ записи.
    /// Пример: "user123", "order456", "product789"
    /// </summary>
    public string? RowKey { get; init; }

    /// <summary>
    /// Полезная нагрузка - произвольные данные записи.
    /// В реальных проектах можно добавлять любые дополнительные свойства.
    /// </summary>
    public string? Payload { get; init; }
}