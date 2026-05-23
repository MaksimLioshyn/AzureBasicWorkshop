using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsAndStorageSample.Functions;

/// <summary>
/// Queue Trigger функция - автоматически запускается при появлении сообщения в очереди.
/// ДЕМОНСТРАЦИЯ: асинхронная обработка задач через очереди (Queue-based Architecture).
/// </summary>
public sealed class QueueTriggeredLoggingFunction
{
    private readonly ILogger<QueueTriggeredLoggingFunction> _logger;

    public QueueTriggeredLoggingFunction(ILogger<QueueTriggeredLoggingFunction> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Эта функция автоматически вызывается каждый раз, когда новое сообщение появляется в очереди "demo-messages".
    /// ДЕМОНСТРАЦИЯ: QueueTrigger - паттерн "consumer" для асинхронной обработки задач.
    /// 
    /// Преимущества использования очередей:
    /// - Развязывание компонентов (decoupling) - отправитель не зависит от получателя
    /// - Надежность - сообщения не теряются при падении обработчика
    /// - Масштабируемость - Azure автоматически создает больше инстансов функции при росте очереди
    /// - Retry логика - автоматические повторные попытки при ошибках
    /// 
    /// Применение:
    /// - Отправка email/SMS уведомлений
    /// - Обработка платежей
    /// - Генерация отчетов
    /// - Импорт/экспорт данных
    /// - Любые длительные фоновые операции
    /// </summary>
    /// <param name="message">Текст сообщения из очереди (автоматически извлекается триггером)</param>
    [Function("ProcessQueueMessage")]
    public void Run(
        [QueueTrigger(DemoStorageNames.QueueName, Connection = "AzureWebJobsStorage")] string message)
    {
        // В учебном примере мы просто логируем сообщение
        // В реальном приложении здесь может быть любая бизнес-логика
        _logger.LogInformation("📬 QueueTrigger activated! Processing message: {Message}", message);

        // Примеры реальной обработки:
        // - Отправка email через SendGrid
        // - Вызов внешнего API
        // - Сохранение данных в БД
        // - Генерация PDF отчета

        // Важно: если функция выбросит исключение, Azure автоматически вернет сообщение в очередь
        // для повторной обработки (по умолчанию до 5 попыток)
    }
}