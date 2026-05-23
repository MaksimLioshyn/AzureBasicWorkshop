using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using AzureFunctionsAndStorageSample.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsAndStorageSample.Services;

/// <summary>
/// Основной сервис для работы с Azure Storage.
/// Демонстрирует использование трех типов хранилищ: Blob, Queue и Table.
/// </summary>
public sealed class StorageDemoService : IStorageDemoService
{
    // Клиенты для работы с различными типами Azure Storage
    private readonly BlobContainerClient _blobContainerClient; // Для работы с файлами (блобами)
    private readonly QueueClient _queueClient;                 // Для работы с очередями сообщений
    private readonly TableClient _tableClient;                 // Для работы с NoSQL таблицами
    private readonly ILogger<StorageDemoService> _logger;

    public StorageDemoService(IConfiguration configuration, ILogger<StorageDemoService> logger)
    {
        // Получаем строку подключения к Azure Storage из конфигурации
        // В локальной разработке это может быть "UseDevelopmentStorage=true" для эмулятора
        var connectionString = configuration["AzureWebJobsStorage"]
            ?? throw new InvalidOperationException("Configuration key 'AzureWebJobsStorage' is missing.");

        // Инициализируем клиент для работы с контейнером блобов
        _blobContainerClient = new BlobContainerClient(connectionString, DemoStorageNames.BlobContainerName);

        // Инициализируем клиент для работы с очередью
        // Base64 кодирование используется для корректной передачи любых символов в сообщениях
        _queueClient = new QueueClient(
            connectionString,
            DemoStorageNames.QueueName,
            new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

        // Инициализируем клиент для работы с таблицей
        _tableClient = new TableClient(connectionString, DemoStorageNames.TableName);
        _logger = logger;
    }

    /// <summary>
    /// Создает всю необходимую инфраструктуру в Azure Storage.
    /// Идемпотентная операция - можно вызывать многократно без последствий.
    /// </summary>
    public async Task EnsureInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        // Создаем контейнер для блобов, если он еще не существует
        // PublicAccessType.None означает, что доступ к файлам требует аутентификации
        await _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        // Создаем очередь, если она еще не существует
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        // Создаем таблицу, если она еще не существует
        await _tableClient.CreateIfNotExistsAsync(cancellationToken);

        _logger.LogInformation(
            "Storage demo infrastructure is ready. Container: {Container}, Queue: {Queue}, Table: {Table}",
            DemoStorageNames.BlobContainerName,
            DemoStorageNames.QueueName,
            DemoStorageNames.TableName);
    }

    /// <summary>
    /// Загружает текстовый файл в Azure Blob Storage.
    /// ДЕМОНСТРАЦИЯ: операции с Blob Storage для хранения неструктурированных данных (файлы, изображения, видео).
    /// </summary>
    public async Task<Uri> UploadBlobAsync(string fileName, string content, CancellationToken cancellationToken = default)
    {
        // Убеждаемся, что инфраструктура создана
        await EnsureInfrastructureAsync(cancellationToken);

        // Получаем клиент для конкретного блоба (файла)
        var blobClient = _blobContainerClient.GetBlobClient(fileName);

        // Преобразуем текст в двоичные данные
        var data = BinaryData.FromString(content);

        // Загружаем данные в Blob Storage
        // Устанавливаем Content-Type для правильного отображения в браузере
        await blobClient.UploadAsync(
            data,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "text/plain; charset=utf-8"
                }
            },
            cancellationToken);

        _logger.LogInformation("Blob {FileName} uploaded to {Uri}", fileName, blobClient.Uri);

        return blobClient.Uri;
    }

    /// <summary>
    /// Скачивает содержимое файла из Azure Blob Storage.
    /// ДЕМОНСТРАЦИЯ: чтение данных из Blob Storage.
    /// </summary>
    public async Task<string?> DownloadBlobAsync(string fileName, CancellationToken cancellationToken = default)
    {
        await EnsureInfrastructureAsync(cancellationToken);

        var blobClient = _blobContainerClient.GetBlobClient(fileName);

        // Проверяем существование файла перед скачиванием
        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            return null;
        }

        // Скачиваем содержимое блоба и конвертируем в строку
        var content = await blobClient.DownloadContentAsync(cancellationToken);
        return content.Value.Content.ToString();
    }

    /// <summary>
    /// Добавляет сообщение в Azure Queue Storage.
    /// ДЕМОНСТРАЦИЯ: использование очередей для асинхронной обработки и развязывания компонентов системы.
    /// Паттерн "producer-consumer" - один компонент создает задачи, другие их обрабатывают.
    /// </summary>
    public async Task EnqueueMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        await EnsureInfrastructureAsync(cancellationToken);

        // Добавляем сообщение в очередь
        // Оно будет автоматически обработано функцией с QueueTrigger
        await _queueClient.SendMessageAsync(message, cancellationToken);

        _logger.LogInformation("Queue message added: {Message}", message);
    }

    /// <summary>
    /// Сохраняет запись в Azure Table Storage.
    /// ДЕМОНСТРАЦИЯ: работа с NoSQL хранилищем для структурированных данных.
    /// Table Storage - это key-value хранилище с быстрым доступом по ключам.
    /// </summary>
    public async Task SaveTableRecordAsync(TableRecordRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureInfrastructureAsync(cancellationToken);

        // Создаем сущность таблицы
        // PartitionKey используется для распределения данных и группировки
        // RowKey - уникальный идентификатор записи внутри партиции
        var entity = new DemoTableEntity
        {
            PartitionKey = request.PartitionKey!,
            RowKey = request.RowKey!,
            Payload = request.Payload ?? string.Empty
        };

        // UpsertEntity - операция "вставить или обновить"
        // Если запись существует - обновляем, если нет - создаем
        await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);

        _logger.LogInformation(
            "Table entity saved. PartitionKey: {PartitionKey}, RowKey: {RowKey}",
            entity.PartitionKey,
            entity.RowKey);
    }

    /// <summary>
    /// Получает запись из Azure Table Storage по ключам.
    /// ДЕМОНСТРАЦИЯ: чтение данных из Table Storage - очень быстрая операция O(1).
    /// </summary>
    public async Task<DemoTableEntity?> GetTableRecordAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
    {
        await EnsureInfrastructureAsync(cancellationToken);

        // Получаем сущность по первичному ключу (PartitionKey + RowKey)
        NullableResponse<DemoTableEntity> response = await _tableClient.GetEntityIfExistsAsync<DemoTableEntity>(
            partitionKey,
            rowKey,
            cancellationToken: cancellationToken);

        return response.HasValue ? response.Value : null;
    }
}