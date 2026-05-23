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
/// HTTP-триггер функции для работы с Azure Table Storage.
/// ДЕМОНСТРАЦИЯ: CRUD операции с NoSQL хранилищем.
/// </summary>
public sealed class TableDemoFunction
{
    private readonly IStorageDemoService _storageDemoService;

    public TableDemoFunction(IStorageDemoService storageDemoService)
    {
        _storageDemoService = storageDemoService;
    }

    /// <summary>
    /// HTTP POST: /api/tables/save
    /// Сохраняет запись в Azure Table Storage.
    /// ДЕМОНСТРАЦИЯ: создание/обновление записей в NoSQL таблице.
    /// 
    /// Azure Table Storage использует двухуровневую схему ключей:
    /// - PartitionKey: для логической группировки и распределения нагрузки
    /// - RowKey: уникальный идентификатор внутри партиции
    /// 
    /// Пример запроса:
    /// POST /api/tables/save
    /// { "partitionKey": "Users", "rowKey": "user123", "payload": "User data" }
    /// </summary>
    [Function("SaveTableRecordFromHttp")]
    public async Task<HttpResponseData> SaveAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tables/save")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        // Десериализуем JSON из тела запроса
        TableRecordRequest? body = await JsonSerializer.DeserializeAsync<TableRecordRequest>(
            request.Body,
            JsonDefaults.Options,
            cancellationToken);

        // Валидация: оба ключа обязательны
        if (string.IsNullOrWhiteSpace(body?.PartitionKey) || string.IsNullOrWhiteSpace(body.RowKey))
        {
            return await HttpResponseFactory.JsonAsync(
                request,
                HttpStatusCode.BadRequest,
                new { error = "Request body must contain non-empty partitionKey and rowKey fields." },
                cancellationToken);
        }

        // Сохраняем запись в Table Storage (upsert - создаем или обновляем)
        await _storageDemoService.SaveTableRecordAsync(body, cancellationToken);

        // Возвращаем успешный результат
        return await HttpResponseFactory.JsonAsync(
            request,
            HttpStatusCode.OK,
            new
            {
                message = "Table record saved successfully.",
                table = DemoStorageNames.TableName,
                partitionKey = body.PartitionKey,
                rowKey = body.RowKey
            },
            cancellationToken);
    }

    /// <summary>
    /// HTTP GET: /api/tables/{partitionKey}/{rowKey}
    /// Получает запись из Azure Table Storage по ключам.
    /// ДЕМОНСТРАЦИЯ: быстрое чтение данных по первичному ключу O(1).
    /// 
    /// Пример: GET /api/tables/Users/user123
    /// </summary>
    [Function("GetTableRecordFromHttp")]
    public async Task<HttpResponseData> GetAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tables/{partitionKey}/{rowKey}")] HttpRequestData request,
        string partitionKey,
        string rowKey,
        CancellationToken cancellationToken)
    {
        // Получаем запись по первичному ключу
        DemoTableEntity? entity = await _storageDemoService.GetTableRecordAsync(partitionKey, rowKey, cancellationToken);

        // Если запись не найдена, возвращаем 404
        if (entity is null)
        {
            return await HttpResponseFactory.JsonAsync(
                request,
                HttpStatusCode.NotFound,
                new { error = "Table entity was not found." },
                cancellationToken);
        }

        // Возвращаем найденную запись
        return await HttpResponseFactory.JsonAsync(request, HttpStatusCode.OK, entity, cancellationToken);
    }
}