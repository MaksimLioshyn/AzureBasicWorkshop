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
/// HTTP-триггер функции для управления файлами в Azure Blob Storage.
/// ДЕМОНСТРАЦИЯ: как работать с Blob Storage через HTTP API.
/// </summary>
public sealed class BlobManagementFunction
{
    private readonly IStorageDemoService _storageDemoService;

    public BlobManagementFunction(IStorageDemoService storageDemoService)
    {
        _storageDemoService = storageDemoService;
    }

    /// <summary>
    /// HTTP POST: /api/blobs/upload
    /// Загружает текстовый файл в Azure Blob Storage.
    /// ДЕМОНСТРАЦИЯ: создание и сохранение блобов через HTTP-запрос.
    /// 
    /// Пример запроса:
    /// POST /api/blobs/upload
    /// { "fileName": "test.txt", "content": "Hello Azure!" }
    /// </summary>
    [Function("UploadBlobFromHttp")]
    public async Task<HttpResponseData> UploadAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blobs/upload")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        // Десериализуем JSON из тела запроса
        BlobUploadRequest? body = await JsonSerializer.DeserializeAsync<BlobUploadRequest>(
            request.Body,
            JsonDefaults.Options,
            cancellationToken);

        // Валидация входных данных
        if (string.IsNullOrWhiteSpace(body?.FileName) || string.IsNullOrWhiteSpace(body.Content))
        {
            return await HttpResponseFactory.JsonAsync(
                request,
                HttpStatusCode.BadRequest,
                new { error = "Request body must contain non-empty fileName and content fields." },
                cancellationToken);
        }

        // Загружаем файл в Blob Storage
        Uri blobUri = await _storageDemoService.UploadBlobAsync(body.FileName.Trim(), body.Content, cancellationToken);

        // Возвращаем информацию о загруженном файле
        return await HttpResponseFactory.JsonAsync(
            request,
            HttpStatusCode.OK,
            new
            {
                message = "Blob uploaded successfully.",
                blobUri = blobUri.ToString(),
                container = DemoStorageNames.BlobContainerName,
                fileName = body.FileName.Trim()
            },
            cancellationToken);
    }

    /// <summary>
    /// HTTP GET: /api/blobs/{fileName}
    /// Скачивает содержимое файла из Azure Blob Storage.
    /// ДЕМОНСТРАЦИЯ: чтение блобов через HTTP-запрос.
    /// 
    /// Пример: GET /api/blobs/test.txt
    /// </summary>
    [Function("DownloadBlobFromHttp")]
    public async Task<HttpResponseData> DownloadAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "blobs/{fileName}")] HttpRequestData request,
        string fileName,
        CancellationToken cancellationToken)
    {
        // Скачиваем содержимое файла
        string? content = await _storageDemoService.DownloadBlobAsync(fileName, cancellationToken);

        // Если файл не найден, возвращаем 404
        if (content is null)
        {
            return await HttpResponseFactory.JsonAsync(
                request,
                HttpStatusCode.NotFound,
                new { error = $"Blob '{fileName}' was not found." },
                cancellationToken);
        }

        // Возвращаем содержимое файла
        return await HttpResponseFactory.JsonAsync(
            request,
            HttpStatusCode.OK,
            new
            {
                fileName,
                content
            },
            cancellationToken);
    }
}