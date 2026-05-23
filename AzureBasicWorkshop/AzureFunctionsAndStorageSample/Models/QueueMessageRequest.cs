namespace AzureFunctionsAndStorageSample.Models;

/// <summary>
/// Модель запроса для добавления сообщения в Queue Storage.
/// Используется при HTTP POST запросах к /api/queues/send
/// </summary>
public sealed class QueueMessageRequest
{
    /// <summary>
    /// Текст сообщения, которое будет добавлено в очередь для асинхронной обработки.
    /// Это сообщение будет автоматически обработано Queue Trigger функцией.
    /// Пример: "Send email to user@example.com", "Generate report for order #123"
    /// </summary>
    public string? Message { get; init; }
}