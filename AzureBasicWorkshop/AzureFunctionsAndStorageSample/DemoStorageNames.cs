namespace AzureFunctionsAndStorageSample;

/// <summary>
/// Централизованное хранилище имен для Azure Storage ресурсов.
/// Все функции и сервисы используют эти константы для согласованности.
/// </summary>
public static class DemoStorageNames
{
    /// <summary>
    /// Имя контейнера для Azure Blob Storage.
    /// В этом контейнере хранятся все загруженные файлы (блобы).
    /// </summary>
    public const string BlobContainerName = "demo-blobs";

    /// <summary>
    /// Паттерн пути для BlobTrigger.
    /// {name} - это placeholder, который автоматически заменяется на имя загруженного файла.
    /// </summary>
    public const string BlobTriggerPath = BlobContainerName + "/{name}";

    /// <summary>
    /// Имя очереди для Azure Queue Storage.
    /// Используется для асинхронной обработки сообщений.
    /// </summary>
    public const string QueueName = "demo-messages";

    /// <summary>
    /// Имя таблицы для Azure Table Storage.
    /// Используется для хранения структурированных NoSQL данных.
    /// </summary>
    public const string TableName = "DemoRecords";
}