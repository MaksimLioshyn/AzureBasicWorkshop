using System.Net;
using System.Text.Json;
using AzureFunctionsAndStorageSample.Models;
using AzureFunctionsAndStorageSample.Serialization;
using AzureFunctionsAndStorageSample.Services;
using AzureFunctionsAndStorageSample.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctionsAndStorageSample.Functions;

/// <summary>
/// HTTP-триггер функция для работы с Azure Queue Storage.
/// ДЕМОНСТРАЦИЯ: добавление сообщений в очередь для асинхронной обработки.
/// </summary>
public sealed class QueueDemoFunction
{
    private readonly IStorageDemoService _storageDemoService;

    public QueueDemoFunction(IStorageDemoService storageDemoService)
    {
        _storageDemoService = storageDemoService;
    }

    /// <summary>
    /// HTTP POST: /api/queues/send
    /// Добавляет сообщение в Azure Queue Storage.
    /// ДЕМОНСТРАЦИЯ: паттерн "producer" - создание задач для асинхронной обработки.
    /// Сообщение будет автоматически обработано функцией QueueTriggeredLoggingFunction.
    /// 
    /// Пример запроса:
    /// POST /api/queues/send
    /// { "message": "Process this task" }
    /// </summary>
    [Function("SendQueueMessageFromHttp")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "queues/send")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        // Десериализуем JSON из тела запроса
        QueueMessageRequest? body = await JsonSerializer.DeserializeAsync<QueueMessageRequest>(
            request.Body,
            JsonDefaults.Options,
            cancellationToken);

        // Валидация: сообщение не должно быть пустым
        if (string.IsNullOrWhiteSpace(body?.Message))
        {
            return await HttpResponseFactory.JsonAsync(
                request,
                HttpStatusCode.BadRequest,
                new { error = "Request body must contain a non-empty message field." },
                cancellationToken);
        }

        // Добавляем сообщение в очередь
        // После этого оно будет автоматически обработано Queue Trigger функцией
        await _storageDemoService.EnqueueMessageAsync(body.Message.Trim(), cancellationToken);

        // Возвращаем статус 202 Accepted - стандартный код для асинхронных операций
        return await HttpResponseFactory.JsonAsync(
            request,
            HttpStatusCode.Accepted,
            new
            {
                message = "Queue message sent successfully.",
                queue = DemoStorageNames.QueueName,
                payload = body.Message.Trim()
            },
            cancellationToken);
    }
}