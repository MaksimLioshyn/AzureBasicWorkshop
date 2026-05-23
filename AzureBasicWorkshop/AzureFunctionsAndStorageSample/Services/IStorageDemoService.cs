using AzureFunctionsAndStorageSample.Models;

namespace AzureFunctionsAndStorageSample.Services;

/// <summary>
/// Интерфейс сервиса для демонстрации работы с Azure Storage.
/// Инкапсулирует операции с Blob Storage, Queue Storage и Table Storage.
/// </summary>
public interface IStorageDemoService
{
    /// <summary>
    /// Создает всю необходимую инфраструктуру Azure Storage:
    /// - контейнер для блобов
    /// - очередь для сообщений
    /// - таблицу для записей
    /// </summary>
    Task EnsureInfrastructureAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Загружает текстовый файл в Azure Blob Storage.
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    /// <param name="content">Содержимое файла</param>
    /// <returns>URI загруженного блоба</returns>
    Task<Uri> UploadBlobAsync(string fileName, string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Скачивает содержимое файла из Azure Blob Storage.
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    /// <returns>Содержимое файла или null, если файл не найден</returns>
    Task<string?> DownloadBlobAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет сообщение в Azure Queue Storage для асинхронной обработки.
    /// </summary>
    Task EnqueueMessageAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет запись в Azure Table Storage (NoSQL база данных).
    /// </summary>
    Task SaveTableRecordAsync(TableRecordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает запись из Azure Table Storage по ключам.
    /// </summary>
    /// <param name="partitionKey">Ключ партиции (логическая группировка)</param>
    /// <param name="rowKey">Уникальный ключ строки внутри партиции</param>
    /// <returns>Сущность таблицы или null, если не найдена</returns>
    Task<DemoTableEntity?> GetTableRecordAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default);
}