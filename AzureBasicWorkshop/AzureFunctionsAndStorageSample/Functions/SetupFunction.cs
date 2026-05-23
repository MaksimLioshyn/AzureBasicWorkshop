using System.Net;
using AzureFunctionsAndStorageSample.Services;
using AzureFunctionsAndStorageSample.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctionsAndStorageSample.Functions;

/// <summary>
/// HTTP-триггер функция для инициализации демо-инфраструктуры.
/// НАЗНАЧЕНИЕ: создает все необходимые ресурсы Azure Storage перед началом работы.
/// </summary>
public sealed class SetupFunction
{
    private readonly IStorageDemoService _storageDemoService;

    public SetupFunction(IStorageDemoService storageDemoService)
    {
        _storageDemoService = storageDemoService;
    }

    /// <summary>
    /// HTTP POST: /api/setup
    /// Создает контейнер блобов, очередь и таблицу в Azure Storage.
    /// ДЕМОНСТРАЦИЯ: инициализация инфраструктуры через HTTP-запрос.
    /// </summary>
    [Function("SetupDemoInfrastructure")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "setup")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        // Создаем всю необходимую инфраструктуру
        await _storageDemoService.EnsureInfrastructureAsync(cancellationToken);

        // Возвращаем успешный ответ с информацией о созданных ресурсах
        return await HttpResponseFactory.JsonAsync(
            request,
            HttpStatusCode.OK,
            new
            {
                message = "Demo infrastructure created successfully.",
                container = DemoStorageNames.BlobContainerName,
                queue = DemoStorageNames.QueueName,
                table = DemoStorageNames.TableName
            },
            cancellationToken);
    }
}